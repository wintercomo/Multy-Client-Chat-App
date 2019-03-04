using System;
using System.ComponentModel;

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
        private string headers;

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
                this.method = value.Substring(0, 5);
                this.headers = value.Substring(5);
                this.logItemInfo = method + headers;
                this.NotifyPropertyChanged("logItemInfo");
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
