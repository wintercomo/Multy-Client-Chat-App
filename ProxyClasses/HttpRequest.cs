using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ProxyClasses
{
    public class HttpRequest
    {
        public string type; // { PROXY, REQUEST, RESPONSE, MESSAGE };
        public string method;
        public string body;
        Dictionary<string, string> headers = new Dictionary<string, string>();
        
        public HttpRequest(string requestString)
        {
            SeperateProtocolElements(requestString);
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
        override
        public string ToString()
        {
            string objectString = this.method + "\r\n";
            foreach (KeyValuePair<string, string> entry in headers)
            {
                objectString += $"{entry.Key} : {entry.Value}\r\n";
            }
            objectString += this.body;
            return objectString;
        }
    }
}
