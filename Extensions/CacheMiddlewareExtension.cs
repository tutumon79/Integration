using Microsoft.AspNetCore.Builder;
using SouthernApi.Helpers;

namespace SouthernApi.Extensions
{
    public static class CacheMiddlewareExtension
    {
        public static IApplicationBuilder UseCacheMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CacheHelper>();
        }
    }
}
