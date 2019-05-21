using Microsoft.Extensions.Caching.Distributed;
using SouthernApi.Interfaces;
using SouthernApi.Model.Response;
using System;
using System.Collections.Generic;
using SouthernApi.Extensions;

namespace SouthernApi.Model
{
    public class Geography : IComplexBase
    {
        private readonly IDistributedCache cache;
        private readonly DistributedCacheEntryOptions options = null;

        public Geography() { }

        public Geography(IDistributedCache cache)
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
                var regionList = response.Rows.FindAll(x => x.Values[1] == "2");
                ProcessItem(regionList, "Region");
                var subRegionList = response.Rows.FindAll(x => x.Values[1] == "3");
                ProcessItem(subRegionList, "SubRegion");
                var appelationList = response.Rows.FindAll(x => x.Values[1] == "4");
                ProcessItem(appelationList, "Appelation");
                var vineYardList = response.Rows.FindAll(x => x.Values[1] == "5");
                ProcessItem(vineYardList, "Vineyard");
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
