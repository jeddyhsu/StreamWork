using MailKit.Net.Smtp;
using MimeKit;
using StreamWork.DataModels;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;
using MailKit.Security;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using StreamWork.HelperMethods;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace StreamWork.Services
{
    public class EmailService
    {
        private readonly StorageService storage;
        private readonly EmailTemplateService templates;
        private readonly CookieService cookies;

        private static readonly string name = "StreamWork";
        private static readonly string streamworkEmailAddress = "hey@streamwork.live";
        private static readonly string serviceAccountEmail = "eat-my-ass-google@streamwork-286021.iam.gserviceaccount.com";
        private static readonly string[] scopes = new[] { "https://mail.google.com/" };
        private static readonly string privateKeyPassword = "notasecret"; // Standard password for all Google-issued private keys
        private readonly string certificatePath;
        private static readonly string gmailSmtp = "smtp.gmail.com";
        private static readonly int gmailPort = 587;

        public EmailService(StorageService storage, EmailTemplateService templates, CookieService cookies, IWebHostEnvironment environment) {
            this.storage = storage;
            this.templates = templates;
            this.cookies = cookies;

            certificatePath = Path.Combine(Directory.GetParent(environment.WebRootPath).FullName, "Config", "streamwork-286021-a06875f20a26.p12"); // HACK Completely fixable, but I really don't want to touch this system anymore.
        }

        public async Task SendTemplateToUser(string templateName, Profile user, IFormFileCollection attachments)
        {
            MimeMessage message = new MimeMessage();
            string[] userNames = user.Name.Split('|');
            message.To.Add(new MailboxAddress($"{userNames[0]} {userNames[1]}", user.EmailAddress));

            await SendTemplateToAddress(templateName, user, attachments, message);
        }

        public async Task SendTemplateToStreamwork(string templateName, Profile user, IFormFileCollection attachments)
        {
            MimeMessage message = new MimeMessage();
            message.To.Add(new MailboxAddress("StreamWork", streamworkEmailAddress));

            await SendTemplateToAddress(templateName, user, attachments, message);
        }

        // Internal, to reduce repetition in code
        // Attachments are IFormFiles for simplicity. Maybe change this to something more accessible if necessary in the future?
        private async Task SendTemplateToAddress(string templateName, Profile user, IFormFileCollection attachments, MimeMessage message)
        {
            EmailTemplate template = templates.GetTemplate(templateName);

            message.From.Add(new MailboxAddress(name, streamworkEmailAddress));
            message.Subject = template.BuildSubject(user);

            BodyBuilder bodyBuilder = new BodyBuilder
            {
                TextBody = template.BuildBody(user)
            };

            foreach (IFormFile attachment in attachments)
            {
                byte[] fileBytes;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    attachment.CopyTo(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
            }
            message.Body = bodyBuilder.ToMessageBody();

            await SendEmail(message);
        }

        public async Task SendForgotPassword(Profile user)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(name, streamworkEmailAddress));
            message.To.Add(MailboxAddress.Parse(user.EmailAddress));
            message.Subject = "Recover Your Password";
            message.Body = new TextPart("html")
            {
                Text = ConvertToTemplateString($"Hey there {user.Name.Split('|')[0]},", $"Your password recovery code is: {user.ChangePasswordKey}. If you did not request to change your password, please ignore this email.")
            };

            await SendEmail(message);
        }

        public async Task SendEmailVerification(string userEmailAddress, string verificationCode)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(name, streamworkEmailAddress));
            message.To.Add(MailboxAddress.Parse(userEmailAddress));
            message.Subject = "Verify Your Email Address";
            message.Body = new TextPart("html")
            {
                Text = ConvertToTemplateString($"Thanks for using StreamWork! ", $"Type this code into the sign up form to verify your email: {verificationCode}")
            };

            await SendEmail(message);
        }

        public async Task NotifyAllFollowers(Profile user)
        {
            await Task.Factory.StartNew(async () =>
            {
                // Can't get channel externally, since it's out of date once the stream has started and info is updated.
                Channel channel = await storage.Get<Channel>(SQLQueries.GetUserChannelWithUsername, user.Username);

                // Start both tasks
                var userFollowsTask = await storage.GetList<Follow>(SQLQueries.GetAllFollowersWithId, user.Id);
                var topicFollowsTask = await storage.GetList<TopicFollow>(SQLQueries.GetTopicFollowsBySubject, channel.StreamSubject);

                // Remove overlapping followers
                IEnumerable<TopicFollow> topicFollows = from TopicFollow topicFollow
                                                        in topicFollowsTask
                                                        where userFollowsTask.FirstOrDefault(userFollow => topicFollow.Follower == userFollow.FollowerUsername) == null
                                                        select topicFollow;


                await Task.Factory.StartNew(async () =>
                {
                    foreach (var userFollow in userFollowsTask)
                    {
                        Profile userFollower = await storage.Get<Profile>(SQLQueries.GetUserWithUsername, userFollow.FollowerUsername);
                        if (userFollower != null && userFollower.NotificationSubscribe == "True")
                        {
                            MimeMessage message = new MimeMessage();

                            string url = cookies.host + "/Stream/Live/" + user.Username;
                            message.From.Add(new MailboxAddress(name, streamworkEmailAddress));
                            message.To.Add(MailboxAddress.Parse(userFollower.EmailAddress));
                            message.Subject = $"{user.Username} is now streaming \"{channel.StreamTitle}\" in {channel.StreamSubject}!";
                            message.Body = new TextPart("html")
                            {
                                Text = ConvertToTemplateString($"Hey there {userFollower.Name.Split('|')[0]},", $"A StreamTutor you follow, {user.Username}, is now live-streaming \"{channel.StreamTitle}\" in {channel.StreamSubject}.\n\n Tune in <a href={url}>here!</a>")
                            };

                            await SendEmail(message);
                        }
                    }
                });

                await Task.Factory.StartNew(async () =>
                {
                    foreach (var topicFollow in topicFollows)
                    {
                        Profile userFollower = await storage.Get<Profile>(SQLQueries.GetUserWithUsername, topicFollow.Follower);
                        if (userFollower != null && userFollower.NotificationSubscribe == "True")
                        {
                            MimeMessage message = new MimeMessage();

                            string url = cookies.host + "/Stream/Live/" + user.Username;
                            message.From.Add(new MailboxAddress(name, streamworkEmailAddress));
                            message.To.Add(MailboxAddress.Parse(userFollower.EmailAddress));
                            message.Subject = $"{user.Username} is now streaming \"{channel.StreamTitle}\" in {channel.StreamSubject}!";
                            message.Body = new TextPart("html")
                            {
                                Text = ConvertToTemplateString($"Hey there {userFollower.Name.Split('|')[0]},", $"{user.Username} is now live-streaming \"{channel.StreamTitle}\" in a topic you follow, {channel.StreamSubject}.\n\n Tune in <a href={url}>here!</a>")
                            };

                            await SendEmail(message);
                        }
                    }
                });
            });
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

        private string ConvertToTemplateString(string header, string description)
        {
            string reader = "";

            using (StreamReader streamReader = new StreamReader("EmailTemplates/StreamWorkEmail.html"))
            {
                reader = streamReader.ReadToEnd();
                reader = reader.Replace("{HEADER}", header);
                reader = reader.Replace("{DESCRIPTION}", description);
            }

            return reader;
        }
    }
}
