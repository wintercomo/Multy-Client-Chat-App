using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyClasses
{
    public class CacheItem
    {
        byte[] requestBytes;
        private readonly byte[] tmpBytes;
        DateTime timeSaved;
        public byte[] RequestBytes {
            get
            {
                return this.requestBytes;
            }
            set
            {
                global::System.Console.WriteLine("UPDAETING REQUEST BYTES " + value);
                this.requestBytes = value;
            }
        }
        public DateTime TimeSaved { get => timeSaved; set => timeSaved = value; }
        public byte[] TmpBytes { get => this.requestBytes;}

        public CacheItem(byte[] requestBytes)
        {
            this.RequestBytes = requestBytes;
            this.timeSaved = DateTime.Now;
        }

    }
}
