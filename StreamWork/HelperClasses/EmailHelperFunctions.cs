using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using StreamWork.DataModels;

namespace StreamWork.HelperClasses
{
    public class EmailHelperFunctions
    {
        readonly string _streamworkEmailID = "streamworktutor@gmail.com";
        private readonly string _streamworkEmailPassword = "STREAMW0RK3R!";

        public async Task<bool> SendOutPasswordRecoveryEmail(UserLogin userLogin, string recoveryLink)
        {
            using (StreamReader streamReader = new StreamReader("EmailTemplates/PasswordRecoveryEmailTemplate.html"))
            {
                string reader = streamReader.ReadToEnd();
                reader = reader.Replace("{NAMEOFUSER}", userLogin.Name.Split('|')[0]);
                reader = reader.Replace("{RESETLINK}", recoveryLink);
                await SendEmailToAnyEmailAsync(_streamworkEmailID, userLogin.EmailAddress, null, "Change Password", reader, null);
            }

            return true;
        }

        public async Task<bool> SendOutStudentGreetingEmail(UserLogin userLogin, string recoveryLink)
        {
            using (StreamReader streamReader = new StreamReader("EmailTemplates/AutomatedEmailTemplate.html"))
            {
                string reader = streamReader.ReadToEnd();
                reader = reader.Replace("{NAMEOFUSER}", userLogin.Name.Split('|')[0]);
                reader = reader.Replace("{RESETLINK}", recoveryLink);
                await SendEmailToAnyEmailAsync(_streamworkEmailID, userLogin.EmailAddress, null, "Change Password", reader, null);
            }

            return true;
        }

        public async Task<bool> SendOutTutorGreetingEmail(UserLogin userLogin, string recoveryLink)
        {
            using (StreamReader streamReader = new StreamReader("EmailTemplates/AutomatedEmailTemplate.html"))
            {
                string reader = streamReader.ReadToEnd();
                reader = reader.Replace("{NAMEOFUSER}", userLogin.Name.Split('|')[0]);
                reader = reader.Replace("{RESETLINK}", recoveryLink);
                await SendEmailToAnyEmailAsync(_streamworkEmailID, userLogin.EmailAddress, null, "Change Password", reader, null);
            }

            return true;
        }

        public async Task<bool> SendOutEmailToStreamWorkTeam(UserLogin login)
        {
            using (StreamReader streamReader = new StreamReader("EmailTemplates/AutomatedEmailTemplate.html"))
            {
                string reader = streamReader.ReadToEnd();
                reader = reader.Replace("{NAMEOFUSER}", "The StreamWork Team");
                reader = reader.Replace("{INTRODUCTION}", "It looks like we got a sign up from a " + login.ProfileType + "!");
                reader = reader.Replace("{BODY}", "Name: " + login.Name.Replace('|', ' ') + " - College/University: " + login.College);
                reader = reader.Replace("{CLOSING}", "GET SHIT DONE!!!"  );
                await SendEmailToAnyEmailAsync(_streamworkEmailID, _streamworkEmailID, null, "New Student Sign Up", reader, null);
            }

            return true;
        }

        public async Task<bool> SendOutEmailToStreamWorkTeam(string firstName, string lastName, string email, List<Attachment> attachments)
        {
            using (StreamReader streamReader = new StreamReader("EmailTemplates/AutomatedEmailTemplate.html"))
            {
                string reader = streamReader.ReadToEnd();
                reader = reader.Replace("{NAMEOFUSER}", "The StreamWork Team");
                reader = reader.Replace("{INTRODUCTION}", "It looks like we got a sign up from a tutor!");
                reader = reader.Replace("{BODY}", "Name: " + firstName + " " + lastName  + " - Email: " + email );
                reader = reader.Replace("{CLOSING}", "GET SHIT DONE!!!");
                await SendEmailToAnyEmailAsync(_streamworkEmailID, _streamworkEmailID, null, "New Tutor SignUp", reader, attachments);
            }

            return true;
        }

        //sends to any email from streamworktutor@gmail.com provided the 'from' 'to' 'subject' 'body' & 'attachments' (if needed)
        public async Task SendEmailToAnyEmailAsync(string from, string to, string[] multipleAddresses, string subject, string body, List<Attachment> attachments)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(_streamworkEmailID, _streamworkEmailPassword),
                EnableSsl = true
            };

            MailMessage message = new MailMessage
            {
                Subject = subject
            };

            if (multipleAddresses == null) message.To.Add(to);
            else
            {
                foreach (var address in multipleAddresses)
                {
                    message.To.Add(address);
                }
            }

            message.Body = body;
            message.From = new MailAddress(from);
            message.IsBodyHtml = true;

            if (attachments != null)
            {
                foreach (var attachement in attachments)
                    message.Attachments.Add(attachement);
            }

            await client.SendMailAsync(message);
        }
    }
}
