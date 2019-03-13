﻿using ProxyServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ProxyClasses
{
    public class HttpRequest : INotifyPropertyChanged
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


        public HttpRequest(string type, ProxySettings settings)
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
                string sm = "";
                foreach (KeyValuePair<string, string> entry in headers)
                {
                    if (entry.Key == "User-Agent" && !settings.LogCLientInfo)
                    {
                        // dont collect the host header if settings say so
                    }
                    else
                    {
                        sm += $"{entry.Key+ entry.Value}\r\n";
                    }
                }
                return sm; 
            }
        }
        public string Body
        {
            get { return this.body; }
        }
        public string HttpString
        {
            get
            {
                //UpdateHeader("Accept-Encoding", ": *");
                UpdateHeader("Connection", ": Close");
                if (settings.AllowChangeHeaders)
                {
                    // Remove server and user headers (From assigment discription)
                    headers.Remove("User-Agent");
                    headers.Remove("Server");
                }
                return $"{Method}\r\n{Headers}\r\n{Body}";
            }
        }
        
        public bool CheckIfModifiedContent()
        {
            if (headers.ContainsKey("Last-Modified"))
            {
                DateTime localDate = DateTime.Now;
                DateTime lastModified = DateTime.Parse(headers["Last-Modified"].Substring(2));
                return localDate > lastModified;
            }
            return false;
        }
        //TODO edit this function so the type is used to display different data
        public string LogItemInfo
        {
            get {
                if (type == "MESSAGE")
                {
                    return logItemInfo;
                }
                if (settings.LogRequestHeaders)
                {
                    return $"{Method}\r\n{Headers}\r\n{Body}";
                }
                    return $"{Method}\r\n{Body}";
            }
            set
            {
                SeperateProtocolElements(value);
                this.logItemInfo = value;
                this.NotifyPropertyChanged("logItemInfo");
            }
        }


        // Get the method/headers and body and save them seperately for later use
        private void SeperateProtocolElements(string value)
        {
            bool reachedBody = false;
            string[] result = Regex.Split(value, "\r\n|\r|\n");
            for (int i = 0; i < result.Length; i++)
            {
                if (i == 0)
                {
                    this.method = result[i];
                }
                else if (i > 0 && !reachedBody)
                {
                    if (result[i] == "")
                    {
                        reachedBody = true;
                    }
                    else
                    {
                        SaveHeader(result, i);
                    }
                }
                else
                {
                    this.body += result[i];
                }
            }
        }
        public void UpdateHeader(string headerType, string header)
        {
            if (headers.ContainsKey(headerType))
            {
                headers.Remove(headerType);
            }
            headers.Add(headerType, header);
        }

        private void SaveHeader(string[] result, int i)
        {
            int index = result[i].IndexOf(':');
            if (index != -1)
            {
                string headerType = result[i].Substring(0, index);
                string header = result[i].Substring(index);
                UpdateHeader(headerType, header);
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
