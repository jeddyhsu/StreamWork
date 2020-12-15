using System.Collections;
using StreamWork.DataModels;

namespace StreamWork.Services
{
    public class EmailTemplate
    {
        public delegate string Builder(Profiles user);

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
                        return $"Name: {user.Name}\nEmailAddress: {user.EmailAddress}\nUsername: {user.Username}\nCollege: {user.College}";
                    }
                )},
                {"tutorSignUp", new EmailTemplate(
                    user =>
                    {
                        return $"Tutor Application: {user.Username}";
                    },
                    user =>
                    {
                        return $"Name: {user.Name}\nEmailAddress: {user.EmailAddress}\nPayPalAddress: {user.PayPalAddress}\nUsername: {user.Username}\nCollege: {user.College}";
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
