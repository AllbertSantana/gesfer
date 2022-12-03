using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Reflection;

namespace backend.Utilities
{
    public static class Export
    {
        public static Task<byte[]> ToSpreadsheet<T>(IEnumerable<T>? items, string name = "Planilha") where T : class
        {
            using var excel = new ExcelPackage();
            var worksheet = excel.Workbook.Worksheets.Add(name);
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            // Header
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            var col = 1;
            foreach (var prop in properties)
                worksheet.Cells[1, col++].Value = prop.Name;

            // Rows
            if (items != null)
            {
                var row = 2;
                foreach (var item in items)
                {
                    col = 1;
                    foreach (var prop in properties)
                        worksheet.Cells[row, col++].Value = prop.GetValue(item);
                    row++;
                }
            }
            
            while(col > 1)
                worksheet.Column(--col).AutoFit();

            return excel.GetAsByteArrayAsync();
        }
    }
}
