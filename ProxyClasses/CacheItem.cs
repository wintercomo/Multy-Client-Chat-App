using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyClasses
{
    public class CacheItem
    {
        readonly byte[] responseBytes;
        DateTime timeSaved;
        public byte[] ResponseBytes
        {
            get
            {
                return this.responseBytes;
            }
        }
        public DateTime TimeSaved { get => timeSaved; set => timeSaved = value; }

        public CacheItem(byte[] requestBytes)
        {
            this.responseBytes = requestBytes;
            this.timeSaved = DateTime.Now;
        }

    }
}
