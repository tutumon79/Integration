using Microsoft.AspNetCore.Http;
using SouthernApi.Model.Request;
using SouthernApi.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SouthernApi.Interfaces
{
    public interface IAssetService
    {
        Task<ImageRequest[]> DownloadImagesFromRequestAsync(HttpRequest httpRequest);
        Task<UrlRequest[]> DownloadCDNImagesAsync(SupplierAssetUpload urlList);
        Task<ImportResponse[]> SendImagesToPIMAsync(UrlRequest[] urlArray);
        Task<string> PostFileToMediaManager(string fileId);
        Task<List<MediaAssetResponse>> UploadFilesToMediaManager(ImportResponse[] fileArray);
    }
}
