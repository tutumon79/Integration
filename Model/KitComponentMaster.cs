using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
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
    public class KitComponentMaster : ISGWSBase
    {
        private ICellStyle headerStyle;
        IJsonHelper jsonHelper;
        IExcelHelper excelHelper;
        private string kitFile = @"C:\Users\Dipu.Divakaran\source\repos\SouthernApi\Schema\kitcomponentsettings.json";

        public KitComponentMaster()
        {
            jsonHelper = new JsonHelper();
            excelHelper = new ExcelHelper();
        }

        public void CreateExcel(SGWSRequestBase request)
        {
            try
            {
                var templateFile = @"C:\\Practice\Kit_Component.xls";
                var newFile = @"C:\\Practice\Components.xls";
                var componentSettings = jsonHelper.LoadSettings(kitFile);
                using (var fs = new FileStream(templateFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (request != null)
                    {
                        var itemRequest = (ItemRequest)request;
                        IWorkbook workBook = new HSSFWorkbook(fs);
                        headerStyle = excelHelper.CreateHeaderStyle(workBook);
                        CreateComponentsSheets(workBook, itemRequest, componentSettings, headerStyle);
                        using (var fileStream = new FileStream(newFile, FileMode.Create, FileAccess.Write))
                        {
                            workBook.Write(fileStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateComponentsSheets(IWorkbook workBook, ItemRequest request, List<ItemAttributes> settings, ICellStyle headerStyle)
        {
            try
            {
                var componentSheet = workBook.GetSheet("KIT_COMPONENT");
                var rowNumber = 1;
                var row = componentSheet.CreateRow(rowNumber);
                foreach (var item in request.Components)
                {
                    row = componentSheet.CreateRow(rowNumber);
                    CreateComponentRow(item, row);
                    rowNumber++;
                }
                componentSheet.AutoSizeColumn(0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateComponentRow(KitComponent component, IRow row)
        {
            try
            {
                var type = component.GetType();
                var properties = type.GetProperties();
                int index = 0;
                foreach (PropertyInfo property in properties)
                {
                    var cell = row.CreateCell(index);
                    string content = property.GetValue(component, null).ToString();
                    if (!string.IsNullOrEmpty(content))
                        cell.SetCellValue(content);
                    index++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
