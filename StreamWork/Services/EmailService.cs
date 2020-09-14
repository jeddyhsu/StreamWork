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
using StreamWork.HelperMethods;
using System.Linq;

namespace StreamWork.Services
{
    public class EmailService
    {
        private readonly StorageService storage;
        private readonly EmailTemplateService templates;

        private static readonly string name = "StreamWork";
        private static readonly string streamworkEmailAddress = "hey@streamwork.live";
        private static readonly string serviceAccountEmail = "eat-my-ass-google@streamwork-286021.iam.gserviceaccount.com";
        private static readonly string[] scopes = new[] { "https://mail.google.com/" };
        private static readonly string privateKeyPassword = "notasecret"; // Standard password for all Google-issued private keys
        private readonly string certificatePath;
        private static readonly string gmailSmtp = "smtp.gmail.com";
        private static readonly int gmailPort = 587;

        public EmailService(StorageService storage, EmailTemplateService templates, IWebHostEnvironment environment) {
            this.storage = storage;
            this.templates = templates;

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
            EmailTemplate template = templates.GetTemplate(templateName);

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

        public async Task NotifyAllFollowers(Profile user)
        {
            // Can't get channel externally, since it's out of date once the stream has started and info is updated.
            Channel channel = await storage.Get<Channel>(SQLQueries.GetUserChannelWithUsername, user.Username);

            // Start both tasks
            Task<List<Follow>> userFollowsTask = storage.GetList<Follow>(SQLQueries.GetAllFollowersWithId, user.Id);
            Task<List<TopicFollow>> topicFollowsTask = storage.GetList<TopicFollow>(SQLQueries.GetTopicFollowsBySubject, channel.StreamSubject);

            await Task.WhenAll(userFollowsTask, topicFollowsTask); // Wait for both to complete

            // Remove overlapping followers
            IEnumerable<TopicFollow> topicFollows = from TopicFollow topicFollow
                                                    in topicFollowsTask.Result.AsParallel()
                                                    where userFollowsTask.Result.AsParallel().FirstOrDefault(userFollow => topicFollow.Follower == userFollow.FollowerUsername) == null
                                                    select topicFollow;

            Parallel.Invoke(
                () => userFollowsTask.Result.AsParallel().ForAll(async userFollow => {
                    Profile userFollower = await storage.Get<Profile>(SQLQueries.GetUserWithUsername, userFollow.FollowerUsername);
                    if (userFollower != null && userFollower.NotificationSubscribe == "True")
                    {
                        MimeMessage message = new MimeMessage();

                        message.From.Add(new MailboxAddress(name, streamworkEmailAddress));
                        message.To.Add(MailboxAddress.Parse(userFollower.EmailAddress));
                        message.Subject = $"{user.Username} is now streaming \"{channel.StreamTitle}\" in {channel.StreamSubject}!";
                        message.Body = new TextPart("plain")
                        {
                            Text = $"A StreamTutor you follow, {user.Username}, is now streaming \"{channel.StreamTitle}\" in {channel.StreamSubject}.\n\nYou can unsubscribe from these emails in your user settings."
                        };

                        await SendEmail(message);
                    }
                }),
                () => topicFollows.AsParallel().ForAll(async topicFollow =>
                {
                    Profile userFollower = await storage.Get<Profile>(SQLQueries.GetUserWithUsername, topicFollow.Follower);
                    if (userFollower != null && userFollower.NotificationSubscribe == "True")
                    {
                        MimeMessage message = new MimeMessage();

                        message.From.Add(new MailboxAddress(name, streamworkEmailAddress));
                        message.To.Add(MailboxAddress.Parse(userFollower.EmailAddress));
                        message.Subject = $"{user.Username} is now streaming \"{channel.StreamTitle}\" in {channel.StreamSubject}!";
                        message.Body = new TextPart("plain")
                        {
                            Text = $"{user.Username} is now streaming \"{channel.StreamTitle}\" in a topic you follow, {channel.StreamSubject}.\n\nYou can unsubscribe from these emails in your user settings."
                        };

                        await SendEmail(message);
                    }
                })
            );
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
