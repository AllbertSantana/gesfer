using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel.DataAnnotations;
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
                worksheet.Cells[1, col++].Value = prop.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? prop.Name;

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

        /*public static Task<byte[]> ToSpreadsheet<T>(T? data, string name = "Planilha") where T : class
        {
            using var excel = new ExcelPackage();
            var worksheet = excel.Workbook.Worksheets.Add(name);

            //if (data is IEnumerable<object> items)
            //    return ToSpreadsheet(items);

            // Header
            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            var col = 1;
            foreach (var propertyName in GetPropertiesNames(typeof(T)))
            {
                worksheet.Cells[1, col++].Value = propertyName;
            }
            // TODO: Maybe flattenning using AutoMap
            // Rows
            if (data != null)
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

            while (col > 1)
                worksheet.Column(--col).AutoFit();

            return excel.GetAsByteArrayAsync();
        }

        private static List<string> GetPropertiesNames(Type type)
        {
            if (typeof(IEnumerable<>).IsAssignableFrom(type))
                type = type.GetGenericArguments()[0];

            if (!type.IsClass)
                return new() { type.Name };

            var names = new List<string>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var property in properties)
            {
                type = property.PropertyType;
                if (typeof(IEnumerable<>).IsAssignableFrom(type))
                    type = type.GetGenericArguments()[0];

                if (type.IsClass)
                {
                    names.AddRange(
                        GetPropertiesNames(type).Select(n => $"{property.Name}.{n}"));
                }
                else
                {
                    names.Add(property.Name);
                }
            }

            return names;
        }

        public static Dictionary<string, List<object?>> Flatten<T>(T obj, string? name = null)
        {
            var dataframe = new Dictionary<string, List<object?>>();

            var type = typeof(T);
            

            if (typeof(IEnumerable<>).IsAssignableFrom(type))
            {
                type = type.GetGenericArguments()[0];
                if (type.IsClass)
                {
                }
                else
                {
                    name ??= type.Name;
                    dataframe[name] = (List<object?>)obj;
                }
            }
            else if (type.IsClass)
            {
            }
            else
            {
                dataframe[property.Name] = new() { property.GetValue(obj) };
            }

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                type = property.PropertyType;
                if (typeof(IEnumerable<>).IsAssignableFrom(type) && type != typeof(string))
                {
                    type = type.GetGenericArguments()[0];
                    if (type.IsClass)
                    {
                        foreach (var pair in Flatten(property.GetValue(obj)))
                        {
                            dataframe[$"{property.Name}.{pair.Key}"] = pair.Value;
                        }
                    }
                    else
                    {
                        dataframe[property.Name] = (property.GetValue(obj) as List<object?>) ?? new();
                    }
                }
                else if (type.IsClass)
                {
                    foreach (var pair in Flatten(property.GetValue(obj)))
                    {
                        dataframe[$"{property.Name}.{pair.Key}"] = pair.Value;
                    }
                }
                else
                {
                    dataframe[property.Name] = new() { property.GetValue(obj) };
                }
            }

            return dataframe;
        }
        */
    }
}
