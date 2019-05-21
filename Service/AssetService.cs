using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SouthernApi.Helpers;
using SouthernApi.Interfaces;
using SouthernApi.Model.Request;
using SouthernApi.Model.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SouthernApi.Service
{
    public class AssetService : IAssetService
    {
        public IWebHelper webHelper;
        public IImageHelper imageHelper;
        private static HttpClient httpClient = new HttpClient();
        public AssetService()
        {
            this.webHelper = new WebHelper();
            this.imageHelper = new ImageHelper();
        }

        public AssetService(IWebHelper webHelper, IImageHelper imageHelper)
        {
            this.webHelper = webHelper;
            this.imageHelper = imageHelper;
        }

        /// <summary>
        /// Invoke the DownloadRequestImageAsync method to download the images in parellel from request
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public async Task<ImageRequest[]> DownloadImagesFromRequestAsync(HttpRequest httpRequest)
        {
            try
            {
                IEnumerable<Task<ImageRequest>> postTaskQuery = from item in httpRequest.Form.Files select DownloadRequestImageAsync(item);
                Task<ImageRequest>[] downloadTasks = postTaskQuery.ToArray();
                ImageRequest[] result = await Task.WhenAll(downloadTasks);
                return result;
            }
            catch (Exception ex)
            {
                throw ex; 
            }
        }

        /// <summary>
        /// Download an individual Image to Disk from HttpRequest
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<ImageRequest> DownloadRequestImageAsync(IFormFile file)
        {
            try
            {
                if (imageHelper.CheckIfImageFile(file))
                {
                    return await imageHelper.DownloadImageAsync(file);
                }
                return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UrlRequest[]> DownloadCDNImagesAsync(SupplierAssetUpload supplierAssetUpload)
        {
            try
            {
                List<Task<UrlRequest>> downloadTaskQuery = new List<Task<UrlRequest>>();
                foreach(var imageList in supplierAssetUpload.ImageList)
                {
                    downloadTaskQuery.Add(DownloadImageAsync(imageList.FrontFullBottle));
                    downloadTaskQuery.Add(DownloadImageAsync(imageList.BackFullBottle));
                    downloadTaskQuery.Add(DownloadImageAsync(imageList.FrontLabel));
                    downloadTaskQuery.Add(DownloadImageAsync(imageList.BackLabel));
                }
                Task<UrlRequest>[] downloadTasks = downloadTaskQuery.ToArray();
                Console.WriteLine("Main Task" + "  Started @ " + DateTime.Now);
                UrlRequest[] result = await Task.WhenAll(downloadTasks);
                Console.WriteLine("Main Task" + "  Ends @ " + DateTime.Now);
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task<UrlRequest> DownloadImageAsync(UrlRequest urlRequest)
        {
            try
            {
                string filePath = @"C:\Practice\Images\" + urlRequest.Name;
                //using (HttpClient client = new HttpClient())
                //{
                    Console.WriteLine(urlRequest.Name + "  Started @ " + DateTime.Now);
                    using (HttpResponseMessage response = await httpClient.GetAsync(urlRequest.Url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        {
                            using (Stream streamToWriteTo = File.Open(filePath, FileMode.Create))
                            {
                                await streamToReadFrom.CopyToAsync(streamToWriteTo);
                            }
                        }
                    }
                    urlRequest.Path = filePath;
                    Console.WriteLine(urlRequest.Name + "  Ended @ " + DateTime.Now);
                //}
                return urlRequest;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// First version of image download from Request. Not currently used.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<string> DownloadRequestImagesAsync(HttpRequest request)
        {
            var errorList = new List<string>();
            try
            {
                foreach (var postedFile in request.Form.Files)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Created);

                    if (postedFile != null && postedFile.Length > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            errorList.Add(string.Format("Please Upload image of type .jpg,.gif,.png."));
                        }
                        else if (postedFile.Length > MaxContentLength)
                        {
                            errorList.Add(string.Format("Please Upload a file upto 1 mb"));
                        }
                        else
                        {
                            var filePath = @"C:\Practice\Images" + postedFile.FileName + extension;
                            using(var stream = new FileStream(filePath, FileMode.Create))
                            {
                                postedFile.CopyTo(stream);
                            }
                        }
                    }
                }
                return errorList;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ImportResponse[]> SendImagesToPIMAsync(UrlRequest[] urlArray)
        {
            try
            {
                IEnumerable<Task<ImportResponse>> postTaskQuery = from item in urlArray select PostImageAsync(item);
                Task<ImportResponse>[] downloadTasks = postTaskQuery.ToArray();
                ImportResponse[] result = await Task.WhenAll(downloadTasks);
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ImportResponse> PostImageAsync(UrlRequest urlRequest)
        {
            try
            {
                var responseMessage = new RESTResponse();
                //using (HttpClient client = new HttpClient())
                //{
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic" , Convert.ToBase64String(Encoding.Default.GetBytes("supplier_gallo:Sgws$1")));
                    var streamContent = webHelper.GetStreamForImage(urlRequest.Path);
                    using (HttpResponseMessage response = await httpClient.PostAsync("https://pim-itemregistry-qa2.test.com/rest/V1.0/manage/file?originalFilename=" + urlRequest.Name, streamContent))
                    {
                        var responseStream = await response.Content.ReadAsStreamAsync();
                        using (StreamReader sr = new StreamReader(responseStream))
                        {
                            responseMessage.Body = sr.ReadToEnd();
                        }
                    }
                //}
                return JsonConvert.DeserializeObject<ImportResponse>(responseMessage.Body);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<MediaAssetResponse>> UploadFilesToMediaManager(ImportResponse[] fileArray)
        {
            try
            {
                var mediaAssetResponsese = new List<MediaAssetResponse>();
                IEnumerable<Task<string>> postTaskQuery = from item in fileArray select PostFileToMediaManager(item.Id);
                Task<string>[] downloadTasks = postTaskQuery.ToArray();
                string[] result = await Task.WhenAll(downloadTasks);
                //TODO : Need Refactoring on this part.
                var resultArray = result.ToList();
                resultArray.ForEach(x =>
                {
                    mediaAssetResponsese.Add(new MediaAssetResponse { Id = x });

                });
                return mediaAssetResponsese; ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> PostFileToMediaManager(string fileId)
        {
            try
            {
                var responseMessage = new RESTResponse();
                //using (HttpClient client = new HttpClient())
                //{
                    //httpClient.Timeout = Timeout.InfiniteTimeSpan;
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.Default.GetBytes("supplier_gallo:Sgws$1")));
                    using (HttpResponseMessage response = await httpClient.PostAsync(webHelper.GetPostMediaManaerUrl(fileId), new StringContent("", Encoding.UTF8, "text/plain")))
                    {
                        var responseStream = await response.Content.ReadAsStreamAsync();
                        using (StreamReader sr = new StreamReader(responseStream))
                        {
                            responseMessage.Body = sr.ReadToEnd();
                        }
                    }
                //}
                return Convert.ToString(responseMessage.Body);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
