using Microsoft.Extensions.Configuration;
using NPOI.HSSF.UserModel;
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
    public class ItemMaster : ISGWSBase
    {
        private string itemFile = @"C:\Users\Dipu.Divakaran\source\repos\SouthernApi\Schema\itemsettings.json";
        //private string kitFile = @"C:\Users\Dipu.Divakaran\source\repos\SouthernApi\Schema\kitcomponentsettings.json";
        private ICellStyle headerStyle;
        IJsonHelper jsonHelper;
        IExcelHelper excelHelper;
        IConfiguration configuration;
        public ItemMaster()
        {
            jsonHelper = new JsonHelper();
            excelHelper = new ExcelHelper();
        }

        public void CreateExcel(SGWSRequestBase request)
        {
            try
            {
                var templateFile = @"C:\\Practice\FinalOne.xls";
                var newFile = @"C:\\Practice\swgsnew.xls";
                var itemSettings = jsonHelper.LoadSettings(itemFile);
                using (var fs = new FileStream(templateFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (request != null)
                    {
                        var itemRequest = (ItemRequest)request;
                        IWorkbook workBook = new HSSFWorkbook(fs);
                        headerStyle = excelHelper.CreateHeaderStyle(workBook);
                        CreateItemSheet(workBook, itemRequest, itemSettings);
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

        public ISheet CreateItemSheet(IWorkbook workBook, ItemRequest request, List<ItemAttributes> settings)
        {
            try
            {
                //CreateSheet
                var itemSheet = workBook.GetSheet("PIM_LAYOUT");
                var rowNumber = 1;
                var row = itemSheet.CreateRow(rowNumber);
                foreach (var item in request.Items)
                {
                    row = itemSheet.CreateRow(rowNumber);
                    CreateItemRow(item, row, null);
                    rowNumber++;
                }
                return itemSheet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CreateItemRow(Item item, IRow row, ICellStyle cellStyle)
        {
            try
            {
                var type = item.GetType();
                var properties = type.GetProperties();
                int index = 0;
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name != "ExtendedProperty")
                    {
                        var cell = row.CreateCell(index);
                        cell.SetCellValue(property.GetValue(item, null).ToString());
                        cell.CellStyle = headerStyle;
                        cell.SetCellType(CellType.String);
                        index++;
                    }
                    else
                    {
                        var extendedProperties = property.GetValue(item, null);
                        var propertyList = (List<Object>)extendedProperties;
                        foreach (var extProperty in propertyList)
                        {
                            var prop = (Newtonsoft.Json.Linq.JObject)extProperty;
                            if (prop.First == null)
                            {
                                var cell = row.CreateCell(index);
                                cell.SetCellValue(string.Empty);
                                cell.CellStyle = headerStyle;
                                cell.SetCellType(CellType.String);
                                index++;
                            }
                            else
                            {
                                var propertyName = ((Newtonsoft.Json.Linq.JProperty)prop.First).Name;
                                var propertyValue = Convert.ToString(prop.GetValue(propertyName));
                                var cell = row.CreateCell(index);
                                cell.SetCellValue(propertyValue);
                                cell.CellStyle = headerStyle;
                                cell.SetCellType(CellType.String);
                                index++;
                            }
                        }
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
