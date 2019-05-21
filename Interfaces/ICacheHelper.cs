using SouthernApi.Model;
using SouthernApi.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SouthernApi.Interfaces
{
    public interface ICacheHelper
    {
        List<CacheSettings> LoadCacheSettings();
        Task GetLOVFromPIMAsync(CacheSettings cacheSettings);
        bool CheckExistInCache(string key);
        Task<T> GetCache<T>(string key) where T : class;
    }
}
