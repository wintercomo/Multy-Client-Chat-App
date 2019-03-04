using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyClasses
{
    public class Cacher
    {
        Dictionary<string, HttpRequest> knowRequests = new Dictionary<string, HttpRequest>();
        public Cacher()
        {

        }
        public void addRequest(string request, HttpRequest response)
        {
            knowRequests.Add(request, response);
        }
        public HttpRequest GetKnownResponse(string request)
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
