using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyClasses
{
    public class Cacher
    {
        Dictionary<string, string> knowRequests = new Dictionary<string, string>();
        public void addRequest(string request, string response)
        {
            knowRequests.Add(request, response);
        }
        public string GetKnownResponse(string request)
        {
            return knowRequests[request];
        }
        public bool RequestKnown(string request)
        {
            //check if request is known
            if (knowRequests.ContainsKey(request))
            {
                return true;
            }
            else
            {
            return false;

            }
        }
    }
}
