using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerce
{
    using Microsoft.Extensions.Caching.Memory;
    using System.Collections.Concurrent;

    public class CacheManager
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentBag<string> _cacheKeys;

        public CacheManager(IMemoryCache cache)
        {
            _cache = cache;
            _cacheKeys = new ConcurrentBag<string>();
        }

        public void AddToCache(string key, object value)
        {
            _cache.Set(key, value);
            _cacheKeys.Add(key);  // ConcurrentBag is thread-safe
        }

        public void InvalidateAllCaches()
        {
            foreach (var key in _cacheKeys)
            {
                _cache.Remove(key);
            }
            _cacheKeys.Clear();  // You might want to keep the keys for future use
        }
    }


}