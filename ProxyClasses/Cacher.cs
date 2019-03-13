﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyClasses
{
    public class Cacher
    {
        Dictionary<string, byte[]> knowRequests = new Dictionary<string, byte[]>();
        public void addRequest(string request, byte[] response)
        {
            knowRequests.Add(request, response);
        }
        public byte[] GetKnownResponse(string request)
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
