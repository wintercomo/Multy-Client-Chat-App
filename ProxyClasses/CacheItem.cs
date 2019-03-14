using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyClasses
{
    public class CacheItem
    {
        byte[] requestBytes;
        DateTime timeSaved;
        public byte[] RequestBytes { get => requestBytes; set => requestBytes = value; }
        public DateTime TimeSaved { get => timeSaved; set => timeSaved = value; }

        public CacheItem(byte[] requestBytes)
        {
            this.RequestBytes = requestBytes;
            this.timeSaved = DateTime.Now;
        }

    }
}
