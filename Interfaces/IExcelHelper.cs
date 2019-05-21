using NPOI.SS.UserModel;
using SouthernApi.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SouthernApi.Interfaces
{
    public interface IExcelHelper
    {
        ICellStyle CreateHeaderStyle(IWorkbook workBook);
        void CreateHeaderRow(IWorkbook workBook, List<ItemAttributes> settings, IRow row, ICellStyle headerStyle);
        byte[] LoadExcel(string filePath);
        Task<byte[]> GetExcelData(Stream inputSteram);
    }
}
