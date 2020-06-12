using System;
namespace StreamWork.TutorObjects
{
    public class Section
    {
        public string SectionTitle;
        public string SectionDescription;

        public Section(string sectionTitle, string sectionDescription)
        {
            SectionTitle = sectionTitle;
            SectionDescription = sectionDescription;
        }
    }
}
