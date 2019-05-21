using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SouthernApi.Interfaces;
using SouthernApi.Model.Request;
using SouthernApi.Model.Response;
using SouthernApi.Service;

namespace SouthernApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        IAssetService assetService;
        public IObjectFactory excelFactory;
        public IMappingHelper mappingHelper;
        public AssetsController(IAssetService assetService, IObjectFactory excelFactory, IMappingHelper mappingHelper)
        {
            this.assetService = assetService;
            this.excelFactory = excelFactory;
            this.mappingHelper = mappingHelper;
        }

        [HttpPost("uploadCDN")]
        public async Task<List<MediaAssetResponse>> DownloadCDNAssets([FromBody]SupplierAssetUploadRequest urlList)
        {
            var errorList = new List<string>();
            try
            {
                
                UrlRequest[] ImageArray = null; 
                var factory = excelFactory.GetObject("DigitalAsset");
                foreach(var supplierAsset in urlList.AssetList)
                {
                    factory.CreateExcel(supplierAsset);
                    ImageArray = await assetService.DownloadCDNImagesAsync(supplierAsset);
                }
                var pimResponse = await assetService.SendImagesToPIMAsync(ImageArray);
                var uploadFileResponse = await assetService.UploadFilesToMediaManager(pimResponse);
                return uploadFileResponse;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Upload the images send by the client to PIM.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("uploadRequest")]
        public async Task<List<MediaAssetResponse>> UploadImages(IFormFile file)
        {
            try
            {
                var httpRequest = HttpContext.Request;
                var imageReuestArray = await assetService.DownloadImagesFromRequestAsync(httpRequest);
                var urlRequestArray = mappingHelper.ConvertImageRequestToUrlRequest(imageReuestArray);
                var pimResponse = await assetService.SendImagesToPIMAsync(urlRequestArray);
                var uploadFileResponse = await assetService.UploadFilesToMediaManager(pimResponse);
                return uploadFileResponse;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}