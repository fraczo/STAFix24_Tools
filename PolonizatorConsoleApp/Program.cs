using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace PolonizatorConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SPSite site = new SPSite("http://spf2010/sites/animus"))
            {
                Polonizator.Polonizator.Update_SiteColumnsDisplayName(site);
            }
        }
    }
}
