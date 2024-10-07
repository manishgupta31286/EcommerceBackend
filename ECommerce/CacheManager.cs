using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerce
{
    using Microsoft.Extensions.Caching.Memory;
    using System.Collections.Concurrent;

    public class CacheManager(IMemoryCache cache)
    {
        private readonly IMemoryCache _cache = cache;
        private readonly ConcurrentBag<string> _cacheKeys = [];

        public void AddToCache(string key, object value)
        {
            _cache.Set(key, value);
            _cacheKeys.Add(key);
        }

        public void InvalidateAllCaches()
        {
            foreach (var key in _cacheKeys)
            {
                _cache.Remove(key);
            }
            _cacheKeys.Clear();
        }
    }


}