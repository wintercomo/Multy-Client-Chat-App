using ProxyServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ProxyClasses
{
    public class LogItem : INotifyPropertyChanged
    {
        public const string REQUEST = "REQUEST";
        public const string PROXY_REQUEST = "PROXY REQUEST";
        public const string RESPONSE = "RESPONSE";
        public const string MESSAGE = "MESSAGE";
        public const string CACHED_RESPONSE = "CACHED RESPONSE";
        private string logItemInfo;
        private string type; // { PROXY, REQUEST, RESPONSE, MESSAGE };
        private string method;
        private string body;
        ProxySettings settings;
        Dictionary<string, string> headers = new Dictionary<string, string>();


        public LogItem(string type, ProxySettings settings)
        {
            this.type = type;
            this.settings = settings;
        }
        public string Method
        {
            get { return this.method; }
        }
        public string Type
        {
            get { return this.type; }
        }
        public Dictionary<string,string> getHeadersList
        {
            get { return this.headers; }
        }
        public string Headers
        {
            get
            {
                if (!settings.LogRequestHeaders)
                {
                    return "";
                }
                string sm = "";
                foreach (KeyValuePair<string, string> entry in headers)
                {
                    sm += $"{entry.Key+ entry.Value}\r\n";
                    // do something with entry.Value or entry.Key
                }
                return sm; 
            }
        }
        public string Body
        {
            get { return this.body; }
        }
        //TODO edit this function so the type is used to display different data
        public string LogItemInfo
        {
            get {
                if (type == "MESSAGE")
                {
                    return logItemInfo;
                }
                return $"{Method}\r\n{Headers}\r\n{Body}";
            }
            set
            {
                if (type == "MESSAGE")
                {
                    this.logItemInfo = value;
                }
                else
                {
                    SeperateProtocolElements(value);
                }
                this.NotifyPropertyChanged("logItemInfo");
            }
        }


        // Get the method/headers and body and save them seperately for later use
        private void SeperateProtocolElements(string value)
        {
            string[] result = Regex.Split(value, "\r\n|\r|\n");
            for (int i = 0; i < result.Length; i++)
            {
                if (i == 0)
                {
                    this.method = result[i];
                }
                else if (settings.LogRequestHeaders && i > 0 && i != result.Length - 1)
                {
                    GetHeaders(result, i);
                }
                else
                {
                    this.body = result[i];
                }
            }
        }

        private void GetHeaders(string[] result, int i)
        {
            int index = result[i].IndexOf(':');
            if (index != -1)
            {
                string headerType = result[i].Substring(0, index);
                string header = result[i].Substring(index);
                headers.Add(headerType, header);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
