using System.Collections;
using System.Collections.Generic;

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

        
    }
}
