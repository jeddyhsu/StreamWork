using System;
using System.Collections;
using StreamWork.DataModels;

namespace StreamWork.Services
{
    public class EmailTemplate
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

    public class EmailTemplateService
    {
        private readonly Hashtable templates;

        public EmailTemplateService()
        {
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
                )},
            };
        }

        public EmailTemplate GetTemplate(string name)
        {
            return (EmailTemplate)templates[name];
        }
    }
}
