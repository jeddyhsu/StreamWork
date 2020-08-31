using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StreamWork.HelperMethods
{
    public static class MiscHelperMethods
    {
        public static string GetCorrespondingSubjectThumbnail(string subject)
        {
            Hashtable table = new Hashtable
            {
                { "Mathematics", "/images/ChatAssets/Math.png" },
                { "Science", "/images/ChatAssets/Science.png" },
                { "Business", "/images/ChatAssets/Business.png" },
                { "Engineering", "/images/ChatAssets/Engineering.png" },
                { "Law", "/images/ChatAssets/Law.png" },
                { "Art", "/images/ChatAssets/Art.png" },
                { "Humanities", "/images/ChatAssets/Humanities.png" },
                { "Other", "/images/ChatAssets/Other.png" },
                { "Others", "/images/ChatAssets/Other.png" }
            };

            return (string)table[subject];
        }

        public static string FormatQueryString(List<string> list)
        {
            if (list.Count == 0) return null;
            string ids = "";
            foreach (string id in list) ids += "'" + id + "'" + ",";
            ids = ids.Remove(0, 1);
            ids = ids.Remove(ids.Length - 2, 2);

            return ids;
        }

        public static string URLIFY(string message)
        {
            var regex = new Regex(@"<a [^>]*?>(?<text>.*?)</a>", RegexOptions.Singleline);

            if (!regex.Match(message).Success)
            {
                string pattern = "(https?://([^ ]+))";
                string replacement = "<a target=\"_blank\" href=\"$1\">$1</a>";
                Regex rgx = new Regex(pattern);
                string result = rgx.Replace(message, replacement);

                return result;
            }

            return message;
        }

        public static string RemoveAllStyleTags(string html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var x = doc.DocumentNode.Descendants();

            foreach(var t in x)
            {
                if(t.Attributes.Contains("style"))
                    t.Attributes["style"].Remove();
            }

            return doc.DocumentNode.InnerHtml;
        }

        public static string GetRandomColor()
        {
            var random = new Random();
            var list = new List<string> { "#D9534F", "#F0AD4E", "#56C0E0", "#5CB85C", "#1C7CD5", "#8B4FD9" };
            int index = random.Next(list.Count);
            return list[index];
        }

        public static string GetCorrespondingStreamColor(string subject)
        {
            Hashtable table = new Hashtable
            {
                { "Mathematics", "#AEE8FE" },
                { "Science", "#A29CFE" },
                { "Business", "#46A86E" },
                { "Engineering", "#74B9FF" },
                { "Law", "#F0AD4E" },
                { "Art", "#F8C5DC" },
                { "Humanities", "#FF7775" },
                { "Other", "#FECA6E" }
            };

            return (string)table[subject];
        }
    }
}
