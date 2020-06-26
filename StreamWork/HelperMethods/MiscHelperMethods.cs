using System;
using System.Collections;
using System.Collections.Generic;

namespace StreamWork.HelperMethods
{
    public static class MiscHelperMethods
    {
        public static string GetCorrespondingSubjectThumbnail(string subject)
        {
            Hashtable table = new Hashtable();
            table.Add("Mathematics", "/images/ChatAssets/Math.png");
            table.Add("Science", "/images/ChatAssets/Science.png");
            table.Add("Business", "/images/ChatAssets/Business.png");
            table.Add("Engineering", "/images/ChatAssets/Engineering.png");
            table.Add("Law", "/images/ChatAssets/Law.png");
            table.Add("Art", "/images/ChatAssets/Art.png");
            table.Add("Humanities", "/images/ChatAssets/Humanities.png");
            table.Add("Other", "/images/ChatAssets/Other.png");


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
    }
}
