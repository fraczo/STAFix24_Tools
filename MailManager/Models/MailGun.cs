using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using System.Net.Mail;
using System.IO;

namespace MailManager.Models
{
    class MailGun
    {
        /// <summary>
        /// https://documentation.mailgun.com/api-sending.html#examples
        /// </summary>

        internal static bool SendMail(MailMessage message)
        {
            Logger.LogEvent_Event("MailGun.SendMail.Initialized", message.Subject);

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                    new HttpBasicAuthenticator("api", "key-ad78eba14d918a02b4fa128a0c3e70f1");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                 "stafix24.pl", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";

            // podmiana adresu wysyłającego
            
            //request.AddParameter("from", Tools.Format_SenderEmailAdress(message.From.Address, message.From.DisplayName));
            request.AddParameter("from", Tools.Format_SenderEmailAdress("noreply@stafix24.pl", message.From.DisplayName));

            if (message.ReplyTo == null)
            {
                message.ReplyTo = message.From;
            }


            foreach (MailAddress ma in message.To)
            {
                request.AddParameter("to", ma.Address);
            }

            foreach (MailAddress ma in message.CC)
            {
                request.AddParameter("cc", ma.Address);
            }

            foreach (MailAddress ma in message.Bcc)
            {
                request.AddParameter("bcc", ma.Address);
            }

            request.AddParameter("subject", message.Subject);

            // treść

            if (message.IsBodyHtml)
            {
                request.AddParameter("html", message.Body);
            }
            else
            {
                request.AddParameter("text", message.Body);
            }

            foreach (Attachment att in message.Attachments)
            {
                byte[] buffer = new byte[att.ContentStream.Length];
                request.AddFile("attachment", buffer, att.Name);

                //zapis pliku na dysk
                //FileStream file = new FileStream(@"c:\temp\" + att.Name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                //file.Write(buffer, 0, buffer.Length);
                //file.Dispose();
            }

            // disable/enable track
            request.AddParameter("o:tracking", false);

            // set delivery time
            //ten układ parametrów działa dla naszej strefy czasowej
            //request.AddParameter("o:deliverytime", "Sun, 15 Nov 2015 20:28:00 +0100");

            // tag the message
            request.AddParameter("o:tag", "Animus");

            if (message.ReplyTo != null)
            {
                request.AddParameter("h:Reply-To", message.ReplyTo.Address);
            }

            request.Method = Method.POST;

            IRestResponse response = client.Execute(request);

            Logger.LogEvent_Event("response", response.StatusCode.ToString());

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
    }
}
