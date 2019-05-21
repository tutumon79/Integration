using Microsoft.AspNetCore.Http;
using SouthernApi.Model.Request;
using System.Threading.Tasks;

namespace SouthernApi.Interfaces
{
    public interface IImageHelper
    {
        Task<ImageRequest> DownloadImageAsync(IFormFile file);
        bool CheckIfImageFile(IFormFile file);
    }
}
