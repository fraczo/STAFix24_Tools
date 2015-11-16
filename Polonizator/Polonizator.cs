using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Xml;

namespace Polonizator
{
    public class Polonizator
    {
        public static void Update_SiteColumnsDisplayName(SPSite site)
        {
            using (SPWeb web = site.RootWeb)
            {

                for (int i = 0; i < web.Fields.Count; i++)
                {
                    SPField field = web.Fields[i];

                    if (field.Group.Equals("Kolumny niestandardowe")
                        && field.Hidden != true)
                    {
                        string intName = field.InternalName;

                        if (field.Title.Equals(intName))
                        {
                            // aktualizuj tylko jeżeli nazwa nie była modyfikowana
                            string dispName = Create_DisplayName(intName);
                            XmlDocument xml = new XmlDocument();
                            xml.LoadXml(field.SchemaXml);
                            XmlAttribute attr = xml.SelectSingleNode("/Field").Attributes["DisplayName"];
                            attr.Value = dispName;
                            field.SchemaXml = xml.InnerXml;


                            field.PushChangesToLists = true;
                            field.Update();
                        }
                    }
                }
            }
        }

        private static string Create_DisplayName(string s)
        {
            RemoveLeadingText(ref s, "col");
            RemoveLeadingText(ref s, "sel");
            RemoveLeadingText(ref s, "enum");

            CleanupText(ref s);

            return s;
        }

        private static void CleanupText(ref string s)
        {
            s = s.Replace("_", " ");
            s = s.Replace("  ", " ");

            string s1 = string.Empty;
            bool isFirstChar = true;

            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                string c = chars[i].ToString();
                string c1 = string.Empty;
                if (i < chars.Length - 1)
                {
                    c1 = chars[i + 1].ToString();
                }

                if (c.ToUpperInvariant() == c)
                {
                    if (!string.IsNullOrEmpty(c.Trim()) && (isFirstChar || c1.ToUpperInvariant() == c1))
                    {
                        //jeżeli pierwszy znak lub kolejny znak też jest dużą literą to nie modyfikuj
                        s1 = s1 + c;
                        isFirstChar = false;
                    }
                    else
                    {
                        s1 = s1 + " " + c.ToLowerInvariant();
                    }
                }
                else
                {
                    s1 = s1 + c;
                }
            }

            s1 = s1.Trim();
            s1 = s1.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ");
            s1 = s1.Replace(" vat", " VAT");
            s1 = s1.Replace(" zus", " ZUS");
            s1 = s1.Replace(" pd", " PD");

            s = s1;
        }

        private static void RemoveLeadingText(ref string s, string pattern)
        {
            if (s.StartsWith(pattern))
            {
                s = s.Substring(pattern.Length, s.Length - pattern.Length);
            }
        }
    }
}
