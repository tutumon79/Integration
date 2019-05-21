using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SouthernApi.Helpers;
using SouthernApi.Interfaces;
using SouthernApi.Model;
using SouthernApi.Model.Request;
using SouthernApi.Model.Response;
using SouthernApi.Service;

namespace SouthernApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        public IItemDataService itemDataService;
        public ItemsController(IItemDataService itemDataService)
        {
            this.itemDataService = itemDataService;
        }

        [HttpGet("data")]
        public string Item()
        {
            return "Hello";
        }

        // POST api/values
        [HttpPost("import")]
        public async Task<PIMImportResponse> ImportFile([FromBody]ItemRequest item)
        {
            try
            {
                var headers = HttpContext.Request.Headers["apiKey"];
                Debug.WriteLine("Start Controller : " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
                var response = await itemDataService.ProcessItems(item);
                Debug.WriteLine("End Controller : " + DateTime.Now.ToString("hh.mm.ss.ffffff"));
                return response;
            }
            catch(Exception ex)
            {
                //TODO: Do all Logging here.
                //Find Mongo Integration.
                throw ex;
            }
        }

        [HttpGet("status")]
        public JobStatusResponse JobStatus(string jobId)
        {
            try
            {
                return itemDataService.GetJobStatus(jobId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
