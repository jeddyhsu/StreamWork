﻿using System;
using System.Threading;
using System.Threading.Tasks;
using StreamWork.Core;
using StreamWork.StreamHoster;

namespace StreamWork.Threads
{
    public class ThreadClassHelperFunctions
    {
        public bool CheckIfUserChannelIsLive(string channelKey) //channel keys is [0] and rss id is [1]
        {
            try
            {
                var response = DataStore.CallAPI<StreamHosterEndpoint>("https://a.streamhoster.com/v1/papi/media/stream/stat/realtime-stream?targetcustomerid=" + channelKey, "NjBjZDBjYzlkNTNlOGViZDc3YWYyZGE2ZDNhN2EyZjQ5YWNmODk1YTo=");
                if (response.Data.Length != 0)
                    Console.WriteLine("Live");
                else
                {
                    Console.WriteLine("Not Live");
                    return false;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Error in RunThread: " + ex.Message);
                return true;
            }

            return true;
        }
    }
}
