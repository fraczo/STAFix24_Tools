using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Net.Mail;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SPSite site = new SPSite("http://spf2010/sites/animus"))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists.TryGetList("Wiadomości");
                    SPListItem item = list.Items.Cast<SPListItem>()
                        .FirstOrDefault();
                    if (item!=null)
                    {
                        string from = Get_Text(item, "colNadawca");
                        string to = Get_Text(item, "colOdbiorca");
                        string subject = item.Title;
                        string body = Get_Text(item, "colTrescHTML");
                        string newBody = MailManager.Tools.Get_InnerHTML(body, "HTML");
                        if (!string.IsNullOrEmpty(newBody))
                        {
                            body = newBody;
                        }

                        MailMessage message = new MailMessage(from, to , subject, body);
                        message.IsBodyHtml = true;

                        MailManager.MailManager.SendMailFromMessageQueue(item, message);
                    }
                }
            }
        }

        #region Helpers
        private static string Get_Text(SPListItem item, string col)
        {
            return item[col] != null ? item[col].ToString() : string.Empty;
        } 
        #endregion
    }
}
