using System;
using Microsoft.AspNetCore.Http;
using SouthernApi.Interfaces;
using SouthernApi.Model.Request;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using SouthernApi.Helpers.Enum;
using System.Linq;
using System.Collections.Generic;

namespace SouthernApi.Helpers
{
    public class ImageHelper : IImageHelper
    {
        public bool CheckIfImageFile(IFormFile file)
        {
            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
            var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
            var extension = ext.ToLower();
            return AllowedFileExtensions.Contains(extension);
        }

        public async Task<ImageRequest> DownloadImageAsync(IFormFile file)
        {
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                var filePath = @"C:\Practice\Images\" + file.FileName + extension;
                using (var bits = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(bits);
                }
                return new ImageRequest { ItemNumber = "", Name = file.FileName, Path = filePath };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
