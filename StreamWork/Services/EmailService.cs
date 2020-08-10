using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using StreamWork.DataModels;
using System.Collections;

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

        public EmailService() {
            templates = new Hashtable
            {
                {"test", new EmailTemplate(user =>
                    {
                        return "Test Email";
                    }, user => {
                        string[] names = user.Name.Split('|');
                        return $"Hello {names[0]} {names[1]}! How are you doing today? You email address is {user.EmailAddress} and your username is {user.Username}!";
                    }
                )}
            };
        }

        public void SendTemplateToUser(string templateName, Profile user)
        {
            MimeMessage message = new MimeMessage();
            
            string[] userNames = user.Name.Split('|');
            EmailTemplate template = (EmailTemplate)templates[templateName];

            message.From.Add(new MailboxAddress("StreamWork", "hey@StreamWork.live"));
            message.To.Add(new MailboxAddress($"{userNames[0]} {userNames[1]}", user.EmailAddress));
            message.Subject = template.BuildSubject(user);
            message.Body = new TextPart("plain")
            {
                Text = template.BuildBody(user)
            };

            using SmtpClient client = new SmtpClient();
            client.Connect("smtp.idk.idk", 10000, false);
            client.Authenticate("idk", "idk");
            client.Send(message);
            client.Disconnect(true);
        }
    }
}
