using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer
{
    public class ProxySettings : INotifyPropertyChanged
    {
        private Int32 portValue;
        private Int32 cacheTimeoutValue;
        private Int32 bufferSizeValue;
        private Boolean checkModifiedContent;
        private Boolean contentFilterOn;
        private Boolean basicAuthOn;
        private Boolean allowChangeHeaders;
        private Boolean logRequestHeaders;
        private Boolean logContentIn;
        private Boolean logContentOut;
        private Boolean logCLientInfo;
        private Boolean serverRunning;

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public Int32 Port
        {
            get { return portValue; }
            set
            {
                if (value != portValue)
                {
                    portValue = value;
                }
            }
        }
        public Int32 CacheTimeout
        {
            get { return cacheTimeoutValue; }
            set
            {
                if (value != cacheTimeoutValue)
                {
                    cacheTimeoutValue = value;
                }
            }
        }
        public Int32 BufferSize
        {
            get { return bufferSizeValue; }
            set
            {
                if (value != bufferSizeValue)
                {
                    bufferSizeValue = value;
                }
            }
        }
        public bool CheckModifiedContent
        {
            get { return checkModifiedContent; }
            set
            {
                if (value != checkModifiedContent)
                {
                    checkModifiedContent = value;
                }
            }
        }
        public bool ContentFilterOn
        {
            get { return contentFilterOn; }
            set
            {
                if (value != contentFilterOn)
                {
                    contentFilterOn = value;
                }
            }
        }
        public bool BasicAuthOn
        {
            get { return basicAuthOn; }
            set
            {
                if (value != basicAuthOn)
                {
                    basicAuthOn = value;
                }
            }
        }
        public bool AllowChangeHeaders
        {
            get { return allowChangeHeaders; }
            set
            {
                if (value != allowChangeHeaders)
                {
                    allowChangeHeaders = value;
                }
            }
        }

        public bool LogRequestHeaders
        {
            get { return logRequestHeaders; }
            set
            {
                if (value != logRequestHeaders)
                {
                    logRequestHeaders = value;
                }
            }
        }
        public bool LogContentIn
        {
            get { return logContentIn; }
            set
            {
                if (value != logContentIn)
                {
                    logContentIn = value;
                }
            }
        }
        public bool LogContentOut
        {
            get { return logContentOut; }
            set
            {
                if (value != logContentOut)
                {
                    logContentOut = value;
                }
            }
        }
        public bool LogCLientInfo
        {
            get { return logCLientInfo; }
            set
            {
                if (value != logCLientInfo)
                {
                    logCLientInfo = value;
                }
            }
        }
        public bool ServerRunning
        {
            get { return serverRunning; }
            set
            {
                if (value != serverRunning)
                {
                    serverRunning = value;
                    this.NotifyPropertyChanged("ServerRunning");

                }
            }
        }
    }
}
