using System.Collections.Generic;

namespace SouthernApi.Model.Response
{
    public class PIMImportResponse
    {
        public string ItemStatus { get; set; }
        public string ComponentStatus { get; set; }
        public int ItemErrorCount { get; set; }
        public int ComponentErrorCount { get; set; } 
        public ScheduledJobResponse[] JobResponse { get; set; }
        public List<PIMErrorResponse> ItemErrors { get; set; }
        public List<PIMErrorResponse> KitComponentErrors { get; set; }
    }

    public class PIMErrorResponse
    {
        public string ItemNumber { get; set; }
        public string ErrorMessage { get; set; }
    }
}
