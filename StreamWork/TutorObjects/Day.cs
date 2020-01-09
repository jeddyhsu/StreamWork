using System;
using System.Collections.Generic;

namespace StreamWork.TutorObjects
{
    public class Day
    {
        public DateTime DayOfWeek { get; set; }
        public List<StreamTask> StreamTaskList { get; set; }

        public Day(DateTime dateTime)
        {
            DayOfWeek = dateTime;
        }
    }
}
