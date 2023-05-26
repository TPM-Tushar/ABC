using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ECDataUI.Common
{
    public class CacheManager
    {
        static int CacheExpiryTime = Convert.ToInt32(ConfigurationManager.AppSettings["CacheTimeOut"]);
        /// <summary>
        /// Adding data or model to store System
        /// </summary>
        /// <param name="key">Need to set some key with while storing data to system. 
        /// This key will help to retrieve the same information</param>
        /// <param name="o">Data or Model</param>
        public void Add(string key, object o)
        {
            HttpRuntime.Cache.Insert(key, o, null,
                DateTime.Now.AddMinutes(CacheExpiryTime),//in minutes
                System.Web.Caching.Cache.NoSlidingExpiration);
        }
        /// <summary>
        /// Clear or release data from system
        /// </summary>
        /// <param name="key"></param>
        public void Clear(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
        /// <summary>
        /// Check Model/Data is already stored or not in system
        /// </summary>
        /// <param name="key">Your pre defined key while storing data or model to system</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return HttpRuntime.Cache[key] != null;
        }
        /// <summary>
        /// Fetching/retrieve data from Cached Memory. 
        /// Note it return type is object that's why you need to deserialize it before use.
        /// </summary>
        /// <param name="key">Your pre defined key while storing data or model to system</param>
        /// <returns>Model or data as object data type</returns>
        public object Get(string key)
        {
            try
            {
                return HttpRuntime.Cache[key];
            }
            catch
            {
                return null;
            }
        }

    }
}