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
using System;

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
        private static readonly string emailAddress = "hey@streamwork.live";
        private static readonly string serviceAccountEmail = "streamworkhey@streamwork-284417.iam.gserviceaccount.com";
        private static readonly string[] scopes = new[] { "https://mail.google.com/" };
        private static readonly string privateKeyPassword = "notasecret"; // Standard password for all Google-issued private keys
        private static readonly string certificatePath = Path.Combine(Environment.CurrentDirectory, "Config", "streamwork-284417-96b6d91c8931.p12");
        private static readonly string gmailSmtp = "smtp.gmail.com";
        private static readonly int gmailPort = 587;

        public EmailService() {
            templates = new Hashtable
            {
                {"test", new EmailTemplate(user =>
                    {
                        return "Test Email";
                    }, user => {
                        string[] names = user.Name.Split('|');
                        return $"Hello {names[0]} {names[1]}! How are you doing today? Your email address is {user.EmailAddress} and your username is {user.Username}!";
                    }
                )},
            };
        }

        public async Task SendTemplateToUser(string templateName, Profile user)
        {
            MimeMessage message = new MimeMessage();

            string[] userNames = user.Name.Split('|');
            EmailTemplate template = (EmailTemplate)templates[templateName];

            message.From.Add(new MailboxAddress(name, emailAddress));
            message.To.Add(new MailboxAddress($"{userNames[0]} {userNames[1]}", user.EmailAddress));
            message.Subject = template.BuildSubject(user);
            message.Body = new TextPart("plain")
            {
                Text = template.BuildBody(user)
            };

            await SendEmail(message);
        }

        public async Task SendEmailVerification(string userEmailAddress, string verificationCode)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(name, emailAddress));
            message.To.Add(MailboxAddress.Parse(userEmailAddress));
            message.Subject = "Verify Your Email Address";
            message.Body = new TextPart("plain")
            {
                Text = $"Thank you for signing up for StreamWork! Type this code into the verification page: {verificationCode}"
            };

            await SendEmail(message);
        }

        private async Task SendEmail(MimeMessage message)
        {
            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                Scopes = scopes,
                User = emailAddress
            }.FromCertificate(new X509Certificate2(certificatePath, privateKeyPassword, X509KeyStorageFlags.Exportable)));

            // Token gets put inside credential. At least cross your fucking fingers that it does.
            await credential.RequestAccessTokenAsync(CancellationToken.None);

            using var client = new SmtpClient();
            client.Connect(gmailSmtp, gmailPort);

            var oauth2 = new SaslMechanismOAuth2(emailAddress, credential.Token.AccessToken);
            client.Authenticate(oauth2);

            client.Send(message);
            client.Disconnect(true);
        }
    }
}
