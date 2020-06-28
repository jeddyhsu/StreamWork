using System;
using System.Collections;

namespace StreamWork.Services
{
    public class SubjectService
    {
        private Hashtable subjectIcons;

        public SubjectService()
        {
            subjectIcons = new Hashtable
            {
                { "Mathematics", "/images/mathematics.svg" },
                { "Humanities", "/images/book.svg" },
                { "Science", "/images/microscope.svg" },
                { "Business", "/images/business-strategy.svg" },
                { "Engineering", "/images/gear.svg" },
                { "Law", "/images/court-gavel.svg" },
                { "Art", "/images/inclilned-paint-brush.svg" },
                { "Other", "/images/eye.svg" }
            };
        }

        public string GetSubjectIcon(string subject)
        {
            if (string.IsNullOrEmpty(subject))
            {
                return null;
            }
            return (string)subjectIcons[subject];
        }
    }
}
