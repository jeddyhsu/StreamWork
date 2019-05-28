using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamWork.Models
{
    public class UserProfile
    {
        public string FirstName { get; set; } //property
        public string LastName { get; set; }
        public string ChannelId { get; set; }
        public string PictureUrl { get; set; }
    }
}
