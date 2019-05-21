using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SouthernApi.Interfaces;
using SouthernApi.Model;
using SouthernApi.Model.Request;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SouthernApi.Helpers
{
    public class ExcelHelper : IExcelHelper
    {
        IJsonHelper jsonHelper;

        public ExcelHelper()
        {
            jsonHelper = new JsonHelper();
        }

        public ICellStyle CreateHeaderStyle(IWorkbook workBook)
        {
            var cellStyle = (HSSFCellStyle)workBook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.LightGreen.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.WrapText = false;
            cellStyle.IsLocked = true;
            cellStyle.DataFormat = workBook.CreateDataFormat().GetFormat("text");
            return cellStyle;
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
                    if(!string.IsNullOrEmpty(content))
                        cell.SetCellValue(content);
                    index++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateHeaderRow(IWorkbook workBook, List<ItemAttributes> settings, IRow row, ICellStyle headerStyle)
        {
            try
            {
                int index = 0;
                foreach (var item in settings)
                {
                    var cell = row.CreateCell(index);
                    cell.SetCellValue(item.HeaderText);
                    cell.CellStyle = headerStyle;
                    index++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] LoadExcel(string filePath)
        {
            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<byte[]> GetExcelData(Stream inputSteram)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await inputSteram.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
