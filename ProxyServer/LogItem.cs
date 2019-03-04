using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyServer
{
    class LogItem : INotifyPropertyChanged
    {
        private string logItemInfo;
        public string LogItemInfo
        {
            get { return this.logItemInfo; }
            set
            {
                if (this.logItemInfo != value)
                {
                    this.logItemInfo = value;
                    this.NotifyPropertyChanged("Name");
                }
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
