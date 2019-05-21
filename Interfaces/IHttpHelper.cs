using System.Net.Http;
using System.Threading.Tasks;

namespace SouthernApi.Interfaces
{
    public interface IHttpHelper
    {
        Task<T> GetResponseAsync<T>(string url) where T : class, new();
        Task<T> PostResponseAsync<T>(string url, HttpContent content) where T : class, new();
    }
}
