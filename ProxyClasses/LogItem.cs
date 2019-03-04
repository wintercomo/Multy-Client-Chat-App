using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ProxyClasses
{
    public class LogItem : INotifyPropertyChanged
    {
        public const string REQUEST = "REQUEST";
        public const string RESPONSE = "RESPONSE";
        public const string MESSAGE = "MESSAGE";
        private string logItemInfo;
        private string type; // { PROXY, REQUEST, RESPONSE, MESSAGE };
        private string method;
        private string body;
        Dictionary<string, string> headers = new Dictionary<string, string>();


        public LogItem(string type)
        {

            this.type = type;
        }
        //TODO edit this function so the type is used to display different data
        public string LogItemInfo
        {
            get { return this.logItemInfo; }
            set
            {
                SeperateProtocolElements(value);
                switch (this.type)
                {
                    case MESSAGE:
                        this.logItemInfo = value;
                        break;
                    case REQUEST:
                        this.logItemInfo = $"{this.method}\r\n";
                        foreach (KeyValuePair<string, string> entry in headers)
                        {
                            this.logItemInfo += $"{entry.Key + entry.Value}\r\n";
                            // do something with entry.Value or entry.Key
                        }
                        this.logItemInfo += this.body;
                        break;
                    case RESPONSE:
                        this.logItemInfo = $"{this.method}\r\n";
                        foreach (KeyValuePair<string, string> entry in headers)
                        {
                            this.logItemInfo += $"{entry.Key + entry.Value}\r\n";
                            // do something with entry.Value or entry.Key
                        }
                        this.logItemInfo += this.body;
                        break;
                    default:
                        break;
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
                if (i > 0 && i != result.Length - 1)
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
                string header = result[i].Substring(index + 1);
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
