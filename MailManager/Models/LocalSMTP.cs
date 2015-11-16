using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Microsoft.SharePoint;

namespace MailManager.Models
{
    class LocalSMTP
    {
        internal static bool SendMail(SPWeb web, System.Net.Mail.MailMessage message)
        {
            bool result = false;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPWeb web1 = web.Site.OpenWeb())
                {
                    SmtpClient client = new SmtpClient();
                    client.Host = web1.Site.WebApplication.OutboundMailServiceInstance.Server.Address;

                    try
                    {
                        client.Send(message);
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogEvent_Exception(web.Name, ex.ToString());
                    }
                }
            });

            return result;
            
        }
    }
}
