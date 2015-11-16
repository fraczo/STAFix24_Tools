using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace MailManager
{
    public class Tools
    {
        public static string Get_InnerHTML(string xmlString, string targetNode)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlString);

            XmlNode xn = xml.SelectSingleNode(string.Format(@"descendant-or-self::{0}", targetNode));

            if (xn == null)
            {
                targetNode = targetNode.ToLower();
                xn = xml.SelectSingleNode(string.Format(@"descendant-or-self::{0}", targetNode));
            }

            if (xn == null)
            {
                targetNode = targetNode.ToUpper();
                xn = xml.SelectSingleNode(string.Format(@"descendant-or-self::{0}", targetNode));
            }

            if (xn != null)
            {
                return xn.OuterXml;
            }
            else
            {
                return string.Empty;
            }
        }

        internal static string Format_SenderEmailAdress(string from, string fromName)
        {
            if (string.IsNullOrEmpty(fromName))
            {
                return from.ToString().Trim();
            }
            else
            {
                return string.Format(@"{1} <{0}>",
                    from.ToString().Trim(),
                    fromName.ToString().Trim());
            }
        }

        //// returns the number of milliseconds since Jan 1, 1970 (useful for converting C# dates to JS dates)
        //public static double UnixTicks(this DateTime dt)
        //{
        //    DateTime d1 = new DateTime(1970, 1, 1);
        //    DateTime d2 = dt.ToUniversalTime();
        //    TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
        //    return ts.TotalMilliseconds;
        //}
    }
}
