using Newtonsoft.Json;
using SouthernApi.Model.Request;
using System;
using System.IO;
using System.Net;
using System.Text;
using SouthernApi.Model.Response;
using SouthernApi.Interfaces;
using SouthernApi.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using SouthernApi.Factory;
using SouthernApi.Model;

namespace SouthernApi.Service
{
    public class ItemDataService : IItemDataService
    {
        public IExcelHelper excelHelper;
        public IItemDataHelper itemDataHelper;
        public IJsonHelper jsonHelper;
        public IWebHelper webHelper;
        public IHttpHelper httpHelper;
        public IObjectFactory excelFactory;
        public IValidatorFactory validatorFactory;

        public ItemDataService(IExcelHelper excelHelper, IItemDataHelper itemDataHelper, IJsonHelper jsonHelper,
            IWebHelper webHelper, IHttpHelper httpHelper, IObjectFactory excelFactory, IValidatorFactory validatorFactory)
        {
            this.excelHelper = excelHelper;
            this.itemDataHelper = itemDataHelper;
            this.jsonHelper = jsonHelper;
            this.webHelper = webHelper;
            this.httpHelper = httpHelper;
            this.excelFactory = excelFactory;
            this.validatorFactory = validatorFactory;
        }

        public async Task<PIMImportResponse> ProcessItems(ItemRequest item)
        {
            var scheduleJobResponse = new ScheduledJobResponse();
            var pimImportResponse = new PIMImportResponse();
            var originalItemCount = item.Items.Count;
            var originalComponentsCount = item.Components.Count;

            byte[] itemData = null;
            byte[] componentData = null;

            try
            {
                var items = await ValidateItemsAsync(item);
                pimImportResponse = ProcessValidationResponse(item, items, originalItemCount);
                //TODO : Revisit this logic
                var components = await ValidateComponentsAsync(item);
                pimImportResponse = ProcessComponentValidationResponse(item.Components, components, pimImportResponse, originalComponentsCount);
                if (pimImportResponse.ItemErrorCount >= item.Items.Count && pimImportResponse.ComponentErrorCount >= item.Components.Count)
                    return pimImportResponse;
                ///Create Excel file for Items
                if (pimImportResponse.ItemErrorCount < originalItemCount)
                {
                    CreateItemExcel(item);
                    itemData = await File.ReadAllBytesAsync(@"");
                }
                ///Create Excel file for Kit_Components
                if (pimImportResponse.ComponentErrorCount < originalComponentsCount)
                {
                    CreateKitComponentExcel(item);
                    componentData = await File.ReadAllBytesAsync(@"");
                }
              
                
                var importResponse = await ImportFilesAsync(itemData, componentData);
                if (importResponse != null)
                {
                    var scheduleJobRequests = CreateScheduleJobRequest(importResponse);
                    var scheduleJobResponses = await ScheduleJobsAsync(scheduleJobRequests);
                    pimImportResponse.JobResponse = scheduleJobResponses;
                    return pimImportResponse;
                }
                return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private PIMImportResponse ProcessComponentValidationResponse(List<KitComponent> components, List<ValidationResponse> validationResponse, PIMImportResponse pimImportResponse, int originalCount)
        {
            var componentErrors = new List<PIMErrorResponse>();
            var errorCount = 0;
            try
            {
                foreach (var response in validationResponse)
                {
                    foreach (var error in response.Errors)
                    {
                        errorCount++;
                        componentErrors.Add(new PIMErrorResponse { ItemNumber = error.ItemNumber, ErrorMessage = error.ErrorMessage });
                    }
                }
                pimImportResponse.ComponentErrorCount = errorCount;
                pimImportResponse.KitComponentErrors = componentErrors;
                pimImportResponse.ComponentStatus = "Total Components Submited: " + originalCount + " , Failed :  " + errorCount + " , Send to PIM: " + (originalCount - errorCount);
                return pimImportResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private PIMImportResponse ProcessValidationResponse(ItemRequest item, List<ValidationResponse> items, int originalCount)
        {
            var pimImportResponse = new PIMImportResponse();
            var itemErrors = new List<PIMErrorResponse>();
            var errorCount = 0;
            try
            {
                foreach(var response in items)
                {
                    foreach(var error in response.Errors)
                    {
                        errorCount++;
                        itemErrors.Add(new PIMErrorResponse { ItemNumber = error.ItemNumber, ErrorMessage= error.ErrorMessage });
                    }
                }
                pimImportResponse.ItemErrorCount = errorCount;
                pimImportResponse.ItemErrors = itemErrors;
                pimImportResponse.ItemStatus = "Total Items Submited: " + originalCount + " , Failed :  " + errorCount + " , Send to PIM: " + (originalCount - errorCount) ;
                return pimImportResponse;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<ScheduleJobRequest> CreateScheduleJobRequest(ImportResponse[] importResponses)
        {
            List<ScheduleJobRequest> jobRequests = new List<ScheduleJobRequest>();
            try
            {
                foreach(var item in importResponses)
                {
                    if(item.Type == "Item")
                    {
                        jobRequests.Add(itemDataHelper.GenerateScheduleJobRequest(item.Id, "1244", "'LOAD_ITEMS'", "1"));
                    }
                    else
                        jobRequests.Add(itemDataHelper.GenerateScheduleJobRequest(item.Id, "1244", "'LOAD_API_KIT_COMPONENTS'", "0"));
                }
                return jobRequests;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ImportResponse[]> ImportFilesAsync(byte[] itemData, byte[] componentData)
        {
            try
            {
                int arrayLength = (itemData != null && componentData != null) ? 2 : 1;
                Task<ImportResponse>[] downloadTasks = new Task<ImportResponse>[arrayLength];
                if (itemData != null && componentData != null)
                {
                    downloadTasks[0] = InvokeImportFileAsync("application/octet-stream", itemData, "swgsnew.xls", "Item");
                    downloadTasks[1] = InvokeImportFileAsync("application/octet-stream", componentData, "Components.xls", "Component");
                }
                else if(itemData != null)
                    downloadTasks[0] = InvokeImportFileAsync("application/octet-stream", itemData, "swgsnew.xls", "Item");
                else if(componentData != null)
                    downloadTasks[0] = InvokeImportFileAsync("application/octet-stream", componentData, "Components.xls", "Component");
                ImportResponse[] result = await Task.WhenAll(downloadTasks);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ScheduledJobResponse[]> ScheduleJobsAsync(List<ScheduleJobRequest> jobRequests)
        {
            try
            {
                IEnumerable<Task<ScheduledJobResponse>> postTaskQuery = from item in jobRequests select ScheduleJobAsync(item);
                Task<ScheduledJobResponse>[] downloadTasks = postTaskQuery.ToArray();
                ScheduledJobResponse[] result = await Task.WhenAll(downloadTasks);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Move below functions to excelhelper
        /// </summary>
        /// <param name="item"></param>
        private void CreateItemExcel(ItemRequest item)
        {
            try
            {
                var factory = excelFactory.GetObject("ItemMaster");
                factory.CreateExcel(item);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void CreateKitComponentExcel(ItemRequest item)
        {
            try
            {
                var factory = excelFactory.GetObject("KitComponentMaster");
                factory.CreateExcel(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<ValidationResponse>> ValidateItemsAsync(ItemRequest itemRequest)
        {
            try
            {
                var item = validatorFactory.GetObject("ItemValidator");
                var validationResults = await item.ValidateItemsAsync(itemRequest);
                return validationResults;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<ValidationResponse>> ValidateComponentsAsync(ItemRequest itemRequest)
        {
            try
            {
                var item = validatorFactory.GetObject("ComponentValidator");
                var validationResults = await item.ValidateItemsAsync(itemRequest);
                return validationResults;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ScheduledJobResponse ScheduleJob(ScheduleJobRequest scheduleJobRequest)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://pim-itemregistry-qa2.test.com/rest/V1.0/manage/import");
                request.Method = WebRequestMethods.Http.Post;
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("supplier_gallo:Sgws$1"));
                request.ContentType = "application/json";
                request.Accept = "application/json";
                jsonHelper.WriteToRequestStream(scheduleJobRequest, request);
                var responseMessage = new RESTResponse();
                using (var response = request.GetResponse())
                {
                    responseMessage.Status = (int)((HttpWebResponse)response).StatusCode;
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        responseMessage.Body = sr.ReadToEnd();
                    }
                }
                return JsonConvert.DeserializeObject<ScheduledJobResponse>(responseMessage.Body);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JobStatusResponse GetJobStatus(string jobId)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(webHelper.CreateJobStatusQueryUrl(jobId));
                request.Method = WebRequestMethods.Http.Get;
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("supplier_gallo:Sgws$1"));
                request.ContentType = "application/json";
                var responseMessage = new RESTResponse();
                using (var response = request.GetResponse())
                {
                    responseMessage.Status = (int)((HttpWebResponse)response).StatusCode;
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        responseMessage.Body = sr.ReadToEnd();
                    }
                }
                return JsonConvert.DeserializeObject<JobStatusResponse>(responseMessage.Body);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ImportResponse InvokeImportFile(string contentType, byte[] data)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://pim-itemregistry-qa2.test.com/rest/V1.0/manage/file?originalFilename=");
                request.Method = WebRequestMethods.Http.Post;
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("supplier_gallo:Sgws$1"));
                request.ContentType = contentType;
                request.ContentLength = data.Length;

                using (BinaryWriter postStream = new BinaryWriter(request.GetRequestStream()))
                {
                    postStream.Write(data);
                    postStream.Close();
                }

                var responseMessage = new RESTResponse();
                using (var response = request.GetResponse())
                {
                    responseMessage.Status = (int)((HttpWebResponse)response).StatusCode;
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        responseMessage.Body = sr.ReadToEnd();
                    }
                }
                return JsonConvert.DeserializeObject<ImportResponse>(responseMessage.Body);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private async Task<ImportResponse> InvokeImportFileAsync(string contentType, byte[] data, string fileName, string type)
        {
            try
            {
                var url = "https://pim-itemregistry-qa2.test.com/rest/V1.0/manage/file?originalFilename=" + fileName;
                ByteArrayContent byteContent = new ByteArrayContent(data);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await httpHelper.PostResponseAsync<ImportResponse>(url, byteContent);
                response.Type = type;
                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ScheduledJobResponse> ScheduleJobAsync(ScheduleJobRequest scheduleJobRequest)
        {
            try
            {
                var url = "https://pim-itemregistry-qa2.test.com/rest/V1.0/manage/import";
                var data = JsonConvert.SerializeObject(scheduleJobRequest);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var response = await httpHelper.PostResponseAsync<ScheduledJobResponse>(url, content);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
