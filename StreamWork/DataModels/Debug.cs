using StreamWork.Base;

namespace StreamWork.DataModels
{
    public class Debug : StorageBase //EACH MODEL NEEDS TO INHERIT STORAGE BASE
    {
        //public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }
}
