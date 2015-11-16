using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MailManager.Models
{
    public enum MailServiceProviders
    {
        Default,
        MailGun,
        SendGrid,
        ElasticEmail,
        TurboSMTP
    }
}
