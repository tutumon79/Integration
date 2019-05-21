using System.Collections.Generic;

namespace SouthernApi.Model.Request
{
    public class SupplierAssetUploadRequest 
    {
        public List<SupplierAssetUpload> AssetList { get; set; }
    }

    public class SupplierAssetUpload : SGWSRequestBase
    {
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public List<CDNRequest> ImageList { get; set; }
    }
}
