using Newtonsoft.Json;
using SouthernApi.Interfaces;
using SouthernApi.Model.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SouthernApi.Helpers
{
    public class HttpHelper : IHttpHelper
    {
        private readonly HttpClient httpClient;

        public HttpHelper(HttpClient client)
        {
            this.httpClient = client;
        }

        public async Task<T> GetResponseAsync<T>(string url) where T : class, new()
        {
            try
            {
                var responseMessage = new RESTResponse();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.Default.GetBytes("supplier_gallo:Sgws$1")));
                using (HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (StreamReader sr = new StreamReader(responseStream))
                        {
                            responseMessage.Body = await sr.ReadToEndAsync();
                        }
                    }
                }
                return JsonConvert.DeserializeObject<T>(responseMessage.Body);
            }
            catch(Exception ex)
            {
                throw ex;
            }           
        }

        public async Task<T> PostResponseAsync<T>(string url, HttpContent content) where T : class, new()
        {
            try
            {
                var responseMessage = new RESTResponse();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.Default.GetBytes("supplier_gallo:Sgws$1")));
                
                using (HttpResponseMessage response = await httpClient.PostAsync(url, content))
                {
                    using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (StreamReader sr = new StreamReader(responseStream))
                        {
                            responseMessage.Body = await sr.ReadToEndAsync();
                        }
                    }
                }
                return JsonConvert.DeserializeObject<T>(responseMessage.Body);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
