using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using SouthernApi.Interfaces;
using SouthernApi.Model.Response;
using SouthernApi.Extensions;

namespace SouthernApi.Model
{
    public class Taxonomy : IComplexBase
    {
        private readonly IDistributedCache cache;
        private readonly DistributedCacheEntryOptions options = null;
        public Taxonomy()
        {
        }

        public Taxonomy(IDistributedCache cache)
        {
            this.cache = cache;
            options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(12)
            };
        }

        public void ProcessComplexResponse(LOVComplexResponse response)
        {
            try
            {
                var categoryList = response.Rows.FindAll(x => x.Values[1] == "2");
                ProcessItem(categoryList, "Category");
                var classList = response.Rows.FindAll(x => x.Values[1] == "3");
                ProcessItem(classList, "Class");
                var subClassList = response.Rows.FindAll(x => x.Values[1] == "4");
                ProcessItem(subClassList, "SubClass");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private async void ProcessItem(List<ComplexData> list, string key)
        {
            try
            {
                List<LOVItem> lovList = new List<LOVItem>();
                list.ForEach(x =>
                {
                    lovList.Add(new LOVItem { Id = x.Values[0], ParentId = x.Values[2], Item = x.Values[3] });
                });
                await cache.SetAsync<List<LOVItem>>(key, lovList, options);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
