using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Microsoft.SharePoint;
using System.Net.Mail;
using MailManager.Models;

namespace MailManager
{
    public class MailManager
    {
        public static bool SendMailFromMessageQueue(
            SPListItem item,
            MailMessage message,
            bool isTestMode = true,
            Models.MailServiceProviders provider=Models.MailServiceProviders.Default)
        {
            if (isTestMode)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"<li>{0}: {1}</li>", "do", message.To.ToString());
                message.To.Clear();
                message.To.Add(new MailAddress(item["colNadawca"].ToString()));

                if (!string.IsNullOrEmpty(message.CC.ToString()))
                {
                    sb.AppendFormat(@"<li>{0}: {1}</li>", "kopia do", message.CC.ToString());
                    message.CC.Clear();
                }

                if (!string.IsNullOrEmpty(message.Bcc.ToString()))
                {
                    sb.AppendFormat(@"<li>{0}: {1}</li>", "kopia do", message.Bcc.ToString());
                    message.Bcc.Clear();
                }

                //wstawia kontrolny ciąg znaków
                string body = string.Format(@"{1}<blockquote style='background-color: #FFFFFF'><ul>{0}</ul></blockquote>",
                    sb.ToString(),
                    message.Body);

                message.Body = body;

                message.Subject = String.Format(":TEST {0}", message.Subject);
            }
            else
            {
                //dodaje sygnarurę wiadomości
                string msgIndex = string.Format(@"<blockquote style='font-size: x-small; color: #808080'>#{0}.{1}</blockquote>",
                    item.ID.ToString(),
                    item["_ZadanieId"] != null ? item["_ZadanieId"].ToString() : string.Empty);

                StringBuilder sb = new StringBuilder(message.Body);
                sb.Replace(@"</body></html>", msgIndex + @"</body></html>");
                message.Body = sb.ToString();
            }

            // add attachements
            for (int attachmentIndex = 0; attachmentIndex < item.Attachments.Count; attachmentIndex++)
            {
                string url = item.Attachments.UrlPrefix + item.Attachments[attachmentIndex];
                SPFile file = item.ParentList.ParentWeb.GetFile(url);
                message.Attachments.Add(new Attachment(file.OpenBinaryStream(), file.Name));
            }

            //return SendMail(item.Web, message, Models.MailServiceProviders.Default);
            return SendMail(item.Web, message, Models.MailServiceProviders.MailGun);
        }

        public static bool SendMail(
            SPWeb web,
            MailMessage message,
            Models.MailServiceProviders provider = Models.MailServiceProviders.Default)
        {
            bool result = false;

            try
            {
                switch (provider)
                {
                    case MailServiceProviders.MailGun:
                        result = Models.MailGun.SendMail(message);
                        break;
                    case MailServiceProviders.ElasticEmail:
                        break;
                    case MailServiceProviders.SendGrid:
                        break;
                    case MailServiceProviders.TurboSMTP:
                        break;
                    default:
                        //wysyłka przez SharePoint
                        result = Models.LocalSMTP.SendMail(web, message);
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogEvent_Exception(web.Name, ex.ToString());

                return false;
            }
        }
    }
}


