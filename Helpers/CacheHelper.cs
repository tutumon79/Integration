using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SouthernApi.Interfaces;
using SouthernApi.Model;
using SouthernApi.Model.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SouthernApi.Extensions;

namespace SouthernApi.Helpers
{
    public class CacheHelper : ICacheHelper
    {
        IDistributedCache cache;
        private readonly HttpClient client;
        private readonly IConfiguration configuration;
        private readonly IHttpHelper httpHelper;
        private string cacheFile = @"C:\Users\Dipu.Divakaran\source\repos\SouthernApi\Schema\cachesettings.json";
        private readonly RequestDelegate next;
        private readonly IJsonHelper jsonHelper;
        private readonly IComplexResponse complexResponse;
        DistributedCacheEntryOptions options = null;

        public CacheHelper(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public CacheHelper(RequestDelegate next, IDistributedCache cache, HttpClient client, IConfiguration configuration, 
            IHttpHelper httpHelper, IJsonHelper jsonHelper, IComplexResponse complexResponse)
        {
            this.next = next;
            this.cache = cache;
            this.client = client;
            this.configuration = configuration;
            this.httpHelper = httpHelper;
            this.jsonHelper = jsonHelper;
            this.complexResponse = complexResponse;

            options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddHours(12)
            };
            InvokeCacheCalls();
        }

        #region Properties

        public List<CacheSettings> CacheSettings { get; set; }

        #endregion

        public async Task Invoke(HttpContext httpContext)
        {
            await next.Invoke(httpContext);
        }

        public async void InvokeCacheCalls()
        {
            LoadSettings();
            await InvokeLOVCallsAsync();
        }

        private void LoadSettings()
        {
            LoadItemSettings();
            CacheSettings =  LoadCacheSettings();
        }

        private void LoadItemSettings()
        {
            var settingsFile = configuration.GetValue<string>("files:itemsSettings");
            var itemSettings = jsonHelper.LoadSettings(@settingsFile);
            var jsonData = JsonConvert.SerializeObject(itemSettings);
            cache.SetString("ItemSettings", jsonData, options);
        }

        public List<CacheSettings> LoadCacheSettings()
        {
            try
            {
                using (var file = File.OpenText(cacheFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    List<CacheSettings> cacheList = (List<CacheSettings>)serializer.Deserialize(file, typeof(List<CacheSettings>));
                    return cacheList;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task InvokeLOVCallsAsync()
        {
            try
            {
                var cacheQuery = new List<Task>();
                foreach (var item in CacheSettings)
                {
                    if(item.Type == "Enum")
                    {
                        if (CheckExistInCache(item.Field))
                            cacheQuery.Add(GetLOVFromPIMAsync(item));
                    }
                    else
                    {
                        if (CheckExistInCache(item.Field))
                            cacheQuery.Add(GetComplexDataFromPIMAsync(item));
                    }
                }
                
                Task[] downloadTasks = cacheQuery.ToArray();
                await Task.WhenAll(downloadTasks);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        ///Get LOV from PIM for Specific Key
        public async Task GetLOVFromPIMAsync(CacheSettings cacheSettings)
        {
            try
            {
                string url = configuration.GetValue<string>("pim:cacheItemUrl") + cacheSettings.Key;
                var cacheResponse = await httpHelper.GetResponseAsync<LabelLOVResponse>(url);
                var response = ParseLOVResponse(cacheSettings.Field, cacheResponse);
                await cache.SetAsync<List<LOVItem>>(cacheSettings.Field, response, options);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private List<LOVItem> ParseLOVResponse(string field, dynamic data)
        {
            try
            {
                if(data is LabelLOVResponse)
                {
                    var result = (LabelLOVResponse)data;
                    return result.Entires.Select(x => new LOVItem { Id = x.Key, Item =  x.Label }).ToList();
                }
                else if (data is ProducerLOVResponse)
                {
                    var result = (ProducerLOVResponse)data;
                    return result.Entires.Select(x => new LOVItem { Item = x.Label }).ToList();
                }
                return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task GetUnstructuredLOVFromPIMAsync(CacheSettings cacheSettings)
        {
            try
            {
                string url = configuration.GetValue<string>("pim:cacheItemUrl") + cacheSettings.Key;
                var cacheResponse = await httpHelper.GetResponseAsync<ProducerLOVResponse>(url);
                var response = ParseLOVResponse(cacheSettings.Field, cacheResponse);
                await cache.SetAsync<List<LOVItem>>(cacheSettings.Field, response, options);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task GetComplexDataFromPIMAsync(CacheSettings cacheSettings)
        {
            try
            {
                string url = configuration.GetValue<string>("pim:complexUrl") + cacheSettings.Key + configuration.GetValue<string>("pim:complexUrlFields");
                var cacheResponse = await httpHelper.GetResponseAsync<LOVComplexResponse>(url);
                ParseComplexResponse(cacheResponse, cacheSettings.Field);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ParseComplexResponse(LOVComplexResponse response, string field)
        {
            try
            {
                var complex = complexResponse.CreateObject(field);
                complex.ProcessComplexResponse(response);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check the existnace of an item in Cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool CheckExistInCache(string key)
        {
            try
            {
                var result = cache.GetString(key);
                return string.IsNullOrEmpty(result);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get a generic type from Cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<T> GetCache<T>(string key) where T : class
        {
            try
            {
                var result = cache.GetAsync<T>(key);
                //return JsonConvert.DeserializeObject<T>(result);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
