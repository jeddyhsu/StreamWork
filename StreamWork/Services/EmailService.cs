using MailKit.Net.Smtp;
using MimeKit;
using StreamWork.DataModels;
using System.Collections;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;
using MailKit.Security;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;

namespace StreamWork.Services
{
    public class EmailService
    {
        private class EmailTemplate
        {
            public delegate string Builder(Profile user);

            public readonly Builder BuildSubject;
            public readonly Builder BuildBody;

            public EmailTemplate(Builder BuildSubject, Builder BuildBody)
            {
                this.BuildSubject = BuildSubject;
                this.BuildBody = BuildBody; // Email is a strength-intensive job.
            }
        }

        private readonly Hashtable templates;

        //private static readonly string clientId = "41418326736-v947g93sja94pq5pmcn41vo6cj66tgv3.apps.googleusercontent.com";
        //private static readonly string clientSecret = "iG7E63BHxtCMAxwXv2jg1eD8";
        private static readonly string name = "StreamWork";
        private static readonly string streamworkEmailAddress = "hey@streamwork.live";
        private static readonly string serviceAccountEmail = "eat-my-ass-google@streamwork-286021.iam.gserviceaccount.com";
        private static readonly string[] scopes = new[] { "https://mail.google.com/" };
        private static readonly string privateKeyPassword = "notasecret"; // Standard password for all Google-issued private keys
        private readonly string certificatePath;
        private static readonly string gmailSmtp = "smtp.gmail.com";
        private static readonly int gmailPort = 587;

        public EmailService(IWebHostEnvironment environment) {
            templates = new Hashtable
            {
                {"test", new EmailTemplate(
                    user =>
                    {
                        return "Test Email";
                    },
                    user =>
                    {
                        string[] names = user.Name.Split('|');
                        return $"Hello {names[0]} {names[1]}! How are you doing today? Your email address is {user.EmailAddress} and your username is {user.Username}!";
                    }
                )},
                {"studentSignUp", new EmailTemplate(
                    user =>
                    {
                        return $"Student Signed Up: {user.Username}";
                    },
                    user =>
                    {
                        return $"A new student, named {user.Name.Replace('|', ' ')} signed up! Their username is {user.Username}. They go to {user.College}, and you can contact them at {user.EmailAddress}.\nIf these emails get annoying, let Tom know, and he'll turn them off sooner or later.";
                    }
                )},
                {"tutorSignUp", new EmailTemplate(
                    user =>
                    {
                        return $"Tutor Application: {user.Username}";
                    },
                    user =>
                    {
                        return $"We've just recieved a new tutor application from {user.Name.Replace('|', ' ')}, aka {user.Username}. They go to {user.College}, and they are interested in the following topics: (I still need to edit the email system to allow for this). You can contact them at {user.EmailAddress} and pay them at {user.PayPalAddress}. See the attachments for more information.\nAlternatively this is a test account, in which case, you should ignore it.";
                    }
                )}
            };

            certificatePath = Path.Combine(Directory.GetParent(environment.WebRootPath).FullName, "Config", "streamwork-286021-a06875f20a26.p12"); // HACK Completely fixable, but I really don't want to touch this system anymore.
        }

        public async Task SendTemplateToUser(string templateName, Profile user, List<MemoryStream> attachments)
        {
            MimeMessage message = new MimeMessage();
            string[] userNames = user.Name.Split('|');
            message.To.Add(new MailboxAddress($"{userNames[0]} {userNames[1]}", user.EmailAddress));

            await SendTemplateToAddress(templateName, user, attachments, message);
        }

        public async Task SendTemplateToStreamwork(string templateName, Profile user, List<MemoryStream> attachments)
        {
            MimeMessage message = new MimeMessage();
            message.To.Add(new MailboxAddress("StreamWork", streamworkEmailAddress));

            await SendTemplateToAddress(templateName, user, attachments, message);
        }

        // Internal, to reduce repetition in code
        private async Task SendTemplateToAddress(string templateName, Profile user, List<MemoryStream> attachments, MimeMessage message)
        {
            EmailTemplate template = (EmailTemplate)templates[templateName];

            message.From.Add(new MailboxAddress(name, streamworkEmailAddress));
            message.Subject = template.BuildSubject(user);

            BodyBuilder bodyBuilder = new BodyBuilder
            {
                TextBody = template.BuildBody(user)
            };
            foreach (MemoryStream attachment in attachments)
            {
                if (attachment.Length > 0)
                {
                    bodyBuilder.Attachments.Add(MimeEntity.Load(attachment));
                }
            }
            message.Body = bodyBuilder.ToMessageBody();

            await SendEmail(message);
        }

        public async Task SendEmailVerification(string userEmailAddress, string verificationCode)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(name, streamworkEmailAddress));
            message.To.Add(MailboxAddress.Parse(userEmailAddress));
            message.Subject = "Verify Your Email Address";
            message.Body = new TextPart("plain")
            {
                Text = $"Thanks for using StreamWork! Type this code into the sign up form to verify your email: {verificationCode}"
            };

            await SendEmail(message);
        }

        private async Task SendEmail(MimeMessage message)
        {
            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                Scopes = scopes,
                User = streamworkEmailAddress
            }.FromCertificate(new X509Certificate2(certificatePath, privateKeyPassword, X509KeyStorageFlags.MachineKeySet)));

            // Token gets put inside credential. At least cross your fucking fingers that it does.
            await credential.RequestAccessTokenAsync(CancellationToken.None);

            using var client = new SmtpClient();
            client.Connect(gmailSmtp, gmailPort);

            var oauth2 = new SaslMechanismOAuth2(streamworkEmailAddress, credential.Token.AccessToken);
            client.Authenticate(oauth2);

            client.Send(message);
            client.Disconnect(true);
        }
    }
}
