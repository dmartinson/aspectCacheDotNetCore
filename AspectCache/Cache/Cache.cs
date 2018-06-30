using AspectCache.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;


namespace AspectCache.Cache
{
    public class Cache : ICache
    {
        //private static readonly ILogger Logger = LogManager.GetLogger<Cache>();
        private static IDictionary<string, MemoryCacheEntryOptions> configCachePolicies;
        private static MemoryCache memoryCache;
        public Cache()
        {
            memoryCache = new MemoryCache(new MemoryCacheOptions());
            configCachePolicies = new Dictionary<string, MemoryCacheEntryOptions>();


            //var cachePolicySection = ConfigurationManager.GetSection("CacheConfigurationSection") as CacheConfigurationSection;
            var cacheConfigurationSection = CacheConfigurationReader.GetConfiguration();
            if (cacheConfigurationSection == null)
            {
                //Logger.Error($"{ nameof(cachePolicySection) } not found in config file.");
                return;
            }

            if (cacheConfigurationSection.CacheEntryOptionsConfigurations == null)
            {
                //Logger.Error($"{ nameof(cachePolicies) } not found in { nameof(cachePolicySection) } in config file.");
                return;
            }
            foreach (var cacheEntryOptionsConfiguration in cacheConfigurationSection.CacheEntryOptionsConfigurations)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now + TimeSpan.Parse(cacheEntryOptionsConfiguration.AbsoluteExpiration));
                configCachePolicies.Add(cacheEntryOptionsConfiguration.Key, cacheEntryOptions);
            }
        }

        public bool Add(string key, object value, UseCacheAttribute cacheAttribute)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!configCachePolicies.ContainsKey(cacheAttribute.PolicyKey))
            {
                return false;
            }
            var cacheEntryOptions = configCachePolicies[cacheAttribute.PolicyKey];
            if (cacheEntryOptions == null)
            {
                //Logger.Error($"Could not add value { value } to Cache with key: { key } and policy Key: { cacheAttribute.PolicyKey } because policy was not found in config file.");
                return false;
            }

            //Logger.Info($"Add value { value } to Cache with key: { key }, policy Key: { cacheAttribute.PolicyKey } and timeSpanExpiration { cachePolicy.AbsoluteExpiration }.");
            return memoryCache.Set(key, value, cacheEntryOptions) != null;
        }

        public object Get(string key)
        {
            return memoryCache.Get(key);
        }
    }
}
