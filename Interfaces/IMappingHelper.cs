using SouthernApi.Model.Request;

namespace SouthernApi.Interfaces
{
    public interface IMappingHelper
    {
        UrlRequest[] ConvertImageRequestToUrlRequest(ImageRequest[] imageRequests);
    }
}
