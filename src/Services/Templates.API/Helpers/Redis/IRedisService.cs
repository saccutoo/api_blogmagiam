using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Templates.API
{
    public interface IRedisService
    {
        Task<T> GetFromCache<T>(string key) where T : class;

        Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options);

        Task ClearCache(string key);
    }
}
