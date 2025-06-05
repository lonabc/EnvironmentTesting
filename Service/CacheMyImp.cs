using Microsoft.Extensions.Caching.Memory;
using TempModbusProject.Service.IService;

namespace TempModbusProject.Service
{
    public class CacheMyImp : ICacheMy
    {

        private readonly IMemoryCache memCache;
        public CacheMyImp(IMemoryCache memoryCache)
        {
            memCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }


        object ICacheMy.getCacge(string key)
        {
            memCache.TryGetValue(key, out string value);
            var result = value is null ? null : value;
            return result;
        }

        void ICacheMy.setCacge(string key, object value)
        {
            memCache.Set(key, value, TimeSpan.FromMinutes(3600));
        }
    }
}
