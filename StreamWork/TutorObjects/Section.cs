using System;
namespace StreamWork.TutorObjects
{
    public class Section
    {
        public string SectionTitle;
        public string SectionDescription;
        public string Year; //this will only be used for the first section

        public Section(string year)
        {
            Year = year;
        }

        public Section(string sectionTitle, string sectionDescription)
        {
            SectionTitle = sectionTitle;
            SectionDescription = sectionDescription;
        }
    }
}
