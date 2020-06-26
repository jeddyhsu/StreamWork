using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StreamWork.Config;
using StreamWork.Core;

namespace StreamWork.HelperMethods
{
    public class EditProfileMethods
    {
        private readonly HomeMethods _homeMethods = new HomeMethods();
        private readonly TutorMethods _tutorMethods = new TutorMethods();
        private readonly ScheduleMethods _scheduleMethods = new ScheduleMethods();

        

        //private string GetTimeZoneAbbreviation(string zone)
        //{
        //    Hashtable timeZones = new Hashtable
        //    {
        //        { "GMT -08:00", "PST" },
        //        { "GMT -07:00", "MST" },
        //        { "GMT -06:00", "CST" },
        //        { "GMT -05:00", "EST" },
        //        { "GMT -09:00", "Alaska" },
        //        { "GMT -10:00", "Hawaii" },
        //    };

        //    return (string)timeZones[zone];
        //}
    }
}
