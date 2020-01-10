namespace StreamWork.TutorObjects
{
    public class StreamTask
    {
       public StreamTask(string n, string t, string d){
            Name = n;
            TimeOfDay = t;
            Day = d;
       }

       public string Name { get; set; }
       public string TimeOfDay { get; set; }
       public string Day { get; set; }
    }
}
