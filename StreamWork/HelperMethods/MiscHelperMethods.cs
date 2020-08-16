using System;
using System.Collections;
using System.Collections.Generic;
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
                { "Other", "/images/ChatAssets/Other.png" }
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
            string pattern = "(https?://([^ ]+))";
            string replacement = "<a target=\"_blank\" href=\"$1\">$1</a>";
            Regex rgx = new Regex(pattern);
            string result = rgx.Replace(message, replacement);

            return result;
        }

        public static string GetRandomColor()
        {
            var random = new Random();
            var list = new List<string> { "#D9534F", "#F0AD4E", "#56C0E0", "#5CB85C", "#1C7CD5", "#8B4FD9" };
            int index = random.Next(list.Count);
            return list[index];
        }
    }
}
