using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SouthernApi.Helpers;
using SouthernApi.Interfaces;
using SouthernApi.Model.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SouthernApi.Model
{
    public class DigitalAsset : ISGWSBase
    {
        IJsonHelper jsonHelper;
        IExcelHelper excelHelper;
        private string assetFile = @"C:\Users\Dipu.Divakaran\source\repos\SouthernApi\Schema\assetsettings.json";
        private ICellStyle headerStyle;

        public DigitalAsset()
        {
            jsonHelper = new JsonHelper();
            excelHelper = new ExcelHelper();
        }

        public DigitalAsset(IJsonHelper jsonHelper, IExcelHelper excelHelper)
        {
            this.jsonHelper = jsonHelper;
            this.excelHelper = excelHelper;
        }

        public void CreateExcel(SGWSRequestBase request)
        {
            try
            {
                var newFile = @"C:\\Practice\Assets.xlsx";
                var itemSettings = jsonHelper.LoadSettings(assetFile);
                using (var fileStream = new FileStream(newFile, FileMode.Create, FileAccess.Write))
                {
                    if (request != null)
                    {
                        var assetRequest = (SupplierAssetUpload)request;
                        IWorkbook workBook = new XSSFWorkbook();
                        headerStyle = excelHelper.CreateHeaderStyle(workBook);
                        CreateAssetSheet(workBook, assetRequest, itemSettings);
                        workBook.Write(fileStream);
                        excelHelper.GetExcelData(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ISheet CreateAssetSheet(IWorkbook workBook, SupplierAssetUpload request, List<ItemAttributes> settings)
        {
            try
            {
                var itemSheet = workBook.CreateSheet("Sheet1");
                var rowNumber = 0;
                var row = itemSheet.CreateRow(rowNumber);
                excelHelper.CreateHeaderRow(workBook, settings, row, headerStyle);
                rowNumber++;
                foreach (var item in request.ImageList)
                {
                    row = itemSheet.CreateRow(rowNumber);
                    CreateItemRow(item, row);
                }
                return itemSheet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CreateItemRow(CDNRequest item, IRow row)
        {
            try
            {
                var type = item.GetType();
                var properties = type.GetProperties();
                int index = 0;
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name != "ItemNumber")
                    {
                        var cell = row.CreateCell(index);
                        cell.SetCellValue(((UrlRequest)(property.GetValue(item, null))).Name.Trim());
                        index++;
                    }
                    else
                    {
                        var cell = row.CreateCell(index);
                        cell.SetCellValue(property.GetValue(item, null).ToString().Trim());
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
