using SendGrid;
using SendGrid.Helpers.Mail;
using System;

namespace SaleAndRentingPortalSql.Services
{

    public class EmailSender 
    {
        public static bool SendEmail(string Subject, string MessegeBody, string Title, string Receiver)
        {
            try
            {

                var client = new SendGridClient("SendGridKey");

                var message = new SendGridMessage();
                message.From = new EmailAddress("Noreply@TheGamingTeam.com", "Noreply TheGamingTeam");
                message.AddTo(Receiver);
                message.SetSubject(Subject);
                message.AddSubstitution("[Title]", Title);
                message.TemplateId = "b3bc0351-763e-4229-977e-8a6011521af0";
                message.HtmlContent = MessegeBody;

                var result = client.SendEmailAsync(message).Result;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
