using MailKit.Net.Smtp;
using MimeKit;
using StreamWork.DataModels;
using System.Collections;
using System;
using Google.Apis.Auth.OAuth2;
using System.Threading.Tasks;
using MailKit.Security;
using System.Threading;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;

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
                this.BuildBody = BuildBody;
            }
        }

        private readonly Hashtable templates;

        private static readonly string clientId = "41418326736-m9evhp98usso68cml66knr9oqf6j9h7v.apps.googleusercontent.com";
        private static readonly string clientSecret = "0fHO8pGA9f3zr6zVq7GrseuR";
        private static readonly string name = "StreamWork";
        private static readonly string emailAddress = "hey@streamwork.live";

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
                )}
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

        private async Task SendEmail(MimeMessage message)
        {
            var authCode = new AuthorizationCodeInstalledApp(new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                DataStore = new FileDataStore("CredentialCacheFolder", false),
                Scopes = new[] { "https://mail.google.com/" },
                ClientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                }
            }), new LocalServerCodeReceiver());
            var credential = await authCode.AuthorizeAsync(emailAddress, CancellationToken.None);

            if (authCode.ShouldRequestAuthorizationCode(credential.Token))
            {
                await credential.RefreshTokenAsync(CancellationToken.None);
            }

            using SmtpClient client = new SmtpClient();

            client.Connect("smtp.gmail.com", 587);
            client.Authenticate(new SaslMechanismOAuth2(credential.UserId, credential.Token.AccessToken));
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
