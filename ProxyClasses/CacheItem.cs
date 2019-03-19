using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyClasses
{
    public class CacheItem
    {
        public byte[] responseBytes;
        DateTime timeSaved;
        public byte[] ResponseBytes {
            get
            {
                return this.responseBytes;
            }
            set
            {
                Console.WriteLine($"Changed ResponseBytes[  {responseBytes.Length}] to => {value}");
                this.responseBytes = value;
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
