namespace StreamWork.ProfileObjects
{
    public class Section
    {
        public string SectionTitle;
        public string SectionDescription;
        public string Year; //this will only be used for the first section
        public bool IsOver66Words;

        public Section(string year)
        {
            Year = year;
        }

        public Section(string sectionTitle, string sectionDescription, bool isOver66Words)
        {
            SectionTitle = sectionTitle;
            SectionDescription = sectionDescription;
            IsOver66Words = isOver66Words;
        }
    }
}
