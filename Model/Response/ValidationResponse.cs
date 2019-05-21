
using System.Collections.Generic;

namespace SouthernApi.Model.Response
{
    public class ValidationResponse
    {
        public string SupplierId { get; set; }
        public List<ErrorResponse> Errors { get; set; }
        public List<TaxonomyResponse> TaxonomyList { get; set; }
        public List<GeographyResponse> GeographyList { get; set; }
    }

    public class ErrorResponse
    {
        public string ItemNumber { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class TaxonomyResponse
    {
        public string ItemNumber { get; set; }
        public string TaxonomyId { get; set; }
    }

    public class GeographyResponse
    {
        public string ItemNumber { get; set; }
        public string TaxonomyId { get; set; }
    }
}
