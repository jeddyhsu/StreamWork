using System;
using System.Collections.Generic;
using StreamWork.DataModels;

namespace StreamWork.Services
{
    public class StreamService
    {
        private readonly StorageService storage;

        public StreamService(StorageService storage)
        {
            this.storage = storage;
        }
    }
}
