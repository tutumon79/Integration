using SouthernApi.Interfaces;
using SouthernApi.Model.Request;
using System;

namespace SouthernApi.Helpers
{
    public class MappingHelper : IMappingHelper
    {
        public UrlRequest[] ConvertImageRequestToUrlRequest(ImageRequest[] imageRequests)
        {
            return Array.ConvertAll(imageRequests, new Converter<ImageRequest, UrlRequest>(ImageRequestToUrlRequest));
        }

        private UrlRequest ImageRequestToUrlRequest(ImageRequest imageRequest)
        {
            return new UrlRequest { Name = imageRequest.Name, Path = imageRequest.Path };
        }
    }
}
