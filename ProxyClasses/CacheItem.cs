using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyClasses
{
    public class CacheItem
    {
        byte[] responseBytes;
        private readonly byte[] tmpBytes;
        DateTime timeSaved;
        public byte[] ResponseBytes {
            get
            {
                return this.responseBytes;
            }
            set
            {
                this.responseBytes = value;
            }
        }
        public DateTime TimeSaved { get => timeSaved; set => timeSaved = value; }
        public byte[] TmpBytes { get => this.responseBytes;}

        public CacheItem(byte[] requestBytes)
        {
            this.ResponseBytes = requestBytes;
            this.timeSaved = DateTime.Now;
        }

    }
}
