using Microsoft.Extensions.Caching.Distributed;
using SouthernApi.Interfaces;
using SouthernApi.Model;

namespace SouthernApi.Factory
{
    public class ComplexResponseFactory : IComplexResponse
    {
        IDistributedCache cache;
        public ComplexResponseFactory(IDistributedCache cache)
        {
            this.cache = cache;
        }
        public IComplexBase CreateObject(string type)
        {
            switch (type)
            {
                case "Taxonomy":
                    return new Taxonomy(cache);
                case "Geography":
                    return new Geography(cache);
                default:
                    return new Taxonomy(cache);
            }
        }
    }
}
