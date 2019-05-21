using System.Net.Http;

namespace SouthernApi.Interfaces
{
    public interface IWebHelper
    {
        string CreateJobStatusQueryUrl(string jobId);
        StreamContent GetStreamForImage(string filePath);
        string GetPostMediaManaerUrl(string fileId);
    }
}
