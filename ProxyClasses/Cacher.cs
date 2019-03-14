﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyClasses
{
    public class Cacher
    {
        Dictionary<string, CacheItem> knowRequests = new Dictionary<string, CacheItem>();
        public void addRequest(string request, byte[] response)
        {
            CacheItem cacheItem = new CacheItem(response);
            knowRequests.Add(request, cacheItem);
        }

        //public bool ContentModified(CacheItem item, Int32 cacheTimeout)
        //{
        //    var diffInSeconds = (DateTime.Now - item.TimeSaved ).TotalSeconds;
        //    var timoutSeconds = TimeSpan.FromMilliseconds(cacheTimeout).TotalSeconds;
        //    if (diffInSeconds > timoutSeconds)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        public CacheItem GetKnownResponse(string request)
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
