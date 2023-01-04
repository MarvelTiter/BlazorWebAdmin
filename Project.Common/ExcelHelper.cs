using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Common
{
    public interface IExcelHelper
    {
        void WriteExcel(string path, object data, string sheetName);
        Task WriteExcelAsync(string path, object data, string sheetName);
    }

    public class ExcelHelper : IExcelHelper
    {
        public ExcelHelper()
        {

        }
        public void WriteExcel(string path, object data, string sheetName)
        {
            try
            {
                InternalWriteExcel(path, data, sheetName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task WriteExcelAsync(string path, object data, string sheetName)
        {
            TaskCompletionSource tcs = new TaskCompletionSource();
            try
            {
                InternalWriteExcel(path, data, sheetName);
                tcs.SetResult();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            return tcs.Task;
        }

        private void InternalWriteExcel(string path, object data, string sheetName)
        {
            using var doc = GetDocument(path);
            Func<object, Row> headerRender;
            Func<object, IEnumerable<Row>> bodyRender;
            if (data is IDataReader)
            {
                headerRender = DataReaderHeaderRender;
                bodyRender = DataReaderBodyRender;
            }
            else if (data is DataTable)
            {
                headerRender = DataTableHeaderRender;
                bodyRender = DataTableBodyRender;
            }
            else
            {
                throw new NotSupportedException($"not supported data type: {data.GetType()}");
            }
            WriteSheet(doc, data, headerRender, bodyRender, sheetName);
        }

        public SpreadsheetDocument GetDocument(string path)
        {
            SpreadsheetDocument doc = null;
            try
            {
                if (File.Exists(path))
                {
                    doc = SpreadsheetDocument.Open(path, true);
                }
                else
                {
                    //创建Workbook, 指定为Excel Workbook (*.xlsx).
                    doc = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
                    //创建WorkbookPart（工作簿）
                    WorkbookPart workbookPart = doc.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();
                    //创建工作表列表
                    workbookPart.Workbook.AppendChild(new Sheets());
                    //构建SharedStringTablePart
                    var shareStringPart = workbookPart.AddNewPart<SharedStringTablePart>();
                    //创建共享字符串表
                    shareStringPart.SharedStringTable = new SharedStringTable();
                    workbookPart.Workbook.Save();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return doc;
        }

        private SpreadsheetDocument WriteSheet(SpreadsheetDocument doc, object data, Func<object, Row> headerRender, Func<object, IEnumerable<Row>> bodyRender, string sheetName)
        {
            // 获取WorkbookPart（工作簿）
            var workBookPart = doc.WorkbookPart;
            var sheets = workBookPart!.Workbook.Sheets;
            //获取SharedStringTablePart
            var sharedStringTable = workBookPart!.SharedStringTablePart;
            //创建WorksheetPart（工作簿中的工作表）
            var worksheetPart = workBookPart!.AddNewPart<WorksheetPart>();
            var newSheetIndex = sheets!.Count() + 1;
            sheetName = sheetName ?? $"sheet{newSheetIndex}";
            //Workbook 下创建Sheets节点, 建立一个子节点Sheet，关联工作表WorksheetPart
            var rid = workBookPart.GetIdOfPart(worksheetPart);
            workBookPart!.Workbook.Sheets!.AppendChild(new Sheet()
            {
                Id = rid,
                SheetId = (uint)newSheetIndex,
                Name = sheetName
            });

            //初始化Worksheet
            InitWorksheet(worksheetPart);

            //创建表头
            CreateHeader(worksheetPart, sharedStringTable!, data, headerRender);

            //创建内容数据
            CreateBody(worksheetPart, data, bodyRender);

            worksheetPart.Worksheet.Save();
            workBookPart.Workbook.Save();

            return doc;
        }

        /// <summary>
        /// 初始化工作表
        /// </summary>
        /// <param name="worksheetPart"></param>
        public void InitWorksheet(WorksheetPart worksheetPart)
        {
            var worksheet = new Worksheet();

            //SheetFormatProperties, 设置默认行高度，宽度， 值类型是Double类型。
            var sheetFormatProperties = new SheetFormatProperties()
            {
                DefaultColumnWidth = 15d,
                DefaultRowHeight = 15d
            };


            //初始化列宽 第一列 5个单位， 第二列~第四列 30个单位
            var columns = new Columns();
            //列，从1开始算起。
            var column1 = new Column
            {
                Min = 1,
                Max = 1,
                Width = 5d,
                CustomWidth = true
            };
            var column2 = new Column
            {
                Min = 2,
                Max = 3,
                Width = 30d,
                CustomWidth = true
            };

            columns.Append(column1, column2);

            // 顺序不能变
            worksheet.Append(new OpenXmlElement[]
            {
                sheetFormatProperties,
                new Columns(),
                new SheetData()
            });
            worksheetPart.Worksheet = worksheet;

        }

        /// <summary>
        /// 创建表头
        /// </summary>
        /// <param name="worksheetPart">WorksheetPart 对象</param>
        /// <param name="shareStringPart">SharedStringTablePart 对象</param>
        public void CreateHeader(WorksheetPart worksheetPart, SharedStringTablePart shareStringPart, object data, Func<object, Row> headerRender)
        {
            //获取Worksheet对象
            var worksheet = worksheetPart.Worksheet;

            //获取表格的数据对象，SheetData
            var sheetData = worksheet.GetFirstChild<SheetData>();

            var row = headerRender.Invoke(data);
            row.RowIndex = 1;
            sheetData!.AppendChild(row);
        }

        public void CreateBody(WorksheetPart worksheetPart, object data, Func<object, IEnumerable<Row>> bodyRender)
        {
            //获取Worksheet对象
            var worksheet = worksheetPart.Worksheet;

            //获取表格的数据对象，SheetData
            var sheetData = worksheet.GetFirstChild<SheetData>();

            var rows = bodyRender.Invoke(data);
            uint startIndex = 2;
            foreach (var r in rows)
            {
                r.RowIndex = startIndex;
                sheetData!.AppendChild(r);
                startIndex++;
            }
        }

        Row DataTableHeaderRender(object data)
        {
            var table = (DataTable)data;
            var row = new Row();
            foreach (DataColumn col in table.Columns)
            {
                var cell = new Cell
                {
                    CellValue = new CellValue(col.ColumnName),
                    DataType = new EnumValue<CellValues>(CellValues.String),
                };
                row.AppendChild(cell);
            }
            return row;
        }

        Row DataReaderHeaderRender(object data)
        {
            var reader = (IDataReader)data;
            var row = new Row();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var cell = new Cell
                {
                    CellValue = new CellValue(reader.GetName(i)),
                    DataType = new EnumValue<CellValues>(CellValues.String),
                };
                row.AppendChild(cell);
            }
            return row;
        }

        IEnumerable<Row> DataTableBodyRender(object data)
        {
            var table = (DataTable)data;
            foreach (DataRow item in table.Rows)
            {
                var row = new Row();
                foreach (DataColumn column in table.Columns)
                {
                    var cell = CreateTypedCell(column.DataType, item[column]);
                    row.AppendChild(cell);
                }
                yield return row;
            }
        }

        IEnumerable<Row> DataReaderBodyRender(object data)
        {
            var reader = (IDataReader)data;
            while (reader.Read())
            {
                var row = new Row();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var cell = CreateTypedCell(reader.GetFieldType(i), reader.GetValue(i));
                    row.AppendChild(cell);
                }
                yield return row;
            }
        }

        private Cell CreateTypedCell(Type type, object value)
        {
            var cell = new Cell();
            if (type == typeof(bool))
            {
                cell.CellValue = new CellValue((bool)value);
                cell.DataType = new EnumValue<CellValues>(CellValues.Boolean);
            }
            else if (type == typeof(DateTime))
            {
                cell.CellValue = new CellValue((DateTime)value);
                cell.DataType = new EnumValue<CellValues>(CellValues.Date);
            }
            else if (type == typeof(DateTimeOffset))
            {
                cell.CellValue = new CellValue((DateTimeOffset)value);
                cell.DataType = new EnumValue<CellValues>(CellValues.Date);
            }
            else if (value.IsNumeric<decimal>(out var v))
            {
                cell.CellValue = new CellValue((decimal)v);
                cell.DataType = new EnumValue<CellValues>(CellValues.Number);
            }
            else if (value.IsNumeric<double>(out var v1))
            {
                cell.CellValue = new CellValue((double)v1);
                cell.DataType = new EnumValue<CellValues>(CellValues.Number);
            }
            else if (value.IsNumeric<int>(out var v2))
            {
                cell.CellValue = new CellValue((int)v2);
                cell.DataType = new EnumValue<CellValues>(CellValues.Number);
            }
            else
            {
                cell.CellValue = new CellValue(value?.ToString() ?? "");
                cell.DataType = new EnumValue<CellValues>(CellValues.String);
            }

            return cell;
        }
    }
}

public static class Ex
{
    public static bool IsNumeric<T>(this object obj, out object value) where T : struct
    {
        value = null;
        if (obj == null)
        {
            return false;
        }
        if (typeof(T) == typeof(decimal))
        {
            if (decimal.TryParse(obj.ToString(), out var d))
            {
                value = d;
                return true;
            }
        }

        if (typeof(T) == typeof(double))
        {
            if (double.TryParse(obj.ToString(), out var d))
            {
                value = d;
                return true;
            }
        }

        if (typeof(T) == typeof(int))
        {
            if (int.TryParse(obj.ToString(), out var d))
            {
                value = d;
                return true;
            }
        }
        return false;
    }
}
