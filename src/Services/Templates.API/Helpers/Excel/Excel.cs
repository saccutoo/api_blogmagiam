using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Templates.API.Excel
{
    public static class Excel
    {
        public static ResponseFile<MemoryStream> ReplaceDataExcel(string param, string base64,string fileName)
        {
            ResponseFile<MemoryStream> response = new ResponseFile<MemoryStream>();
            try
            {
                var dynamic = JsonConvert.DeserializeObject<dynamic>(param);               
                byte[] bytes = Convert.FromBase64String(base64);
                MemoryStream memoryStream = new MemoryStream(bytes);

                using (var workbook = SpreadsheetDocument.Open(memoryStream, true))
                {
                    Sheet sheet = workbook.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                    var sharedStringTable = workbook.WorkbookPart.SharedStringTablePart.SharedStringTable;
                    Worksheet worksheet = (workbook.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                    SheetData sheetData = worksheet.WorksheetPart.Worksheet.GetFirstChild<SheetData>();
                    WorkbookPart workbookPart = workbook.WorkbookPart;

                    IEnumerable<Row> rows = sheetData.Descendants<Row>();
                    var tables = worksheet.WorksheetPart.TableDefinitionParts.ToList();

                    if (rows != null && rows.Count() > 0)
                    {
                        foreach (var row in rows)
                        {
                            IEnumerable<Cell> cells = row.Descendants<Cell>();
                            foreach (var cell in cells)
                            {
                                if (cell != null && cell.DataType != null)
                                {
                                    if (cell.DataType != null)
                                    {
                                        var element = sharedStringTable.ChildElements[int.Parse(cell.InnerText)];
                                        if (!string.IsNullOrEmpty(element.InnerText))
                                        {

                                            if (element.InnerText.IndexOf("BM_") > -1)
                                            {
                                                if (element.InnerText.IndexOf("BM_TABLE") > -1)
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    if (dynamic[element.InnerText]!=null)
                                                    {
                                                        element.InnerXml = element.InnerXml.Replace(element.InnerText, dynamic[element.InnerText].ToString());
                                                        sharedStringTable.Save();
                                                    }
                                                    else
                                                    {
                                                        element.InnerXml = element.InnerXml.Replace(element.InnerText, "");
                                                        sharedStringTable.Save();
                                                    }
                                                   
                                                }

                                            }
                                        }

                                    }
                                }
                            }

                        }
                    }

                    if (rows != null && rows.Count() > 0)
                    {
                        if (tables != null && tables.Count() > 0)
                        {
                            foreach (var table in tables)
                            {
                                Table tableClone = (Table)table.Table;
                                var tableName = tableClone.DisplayName;
                                Row rowClone = new Row();
                                bool rowTable = false;
                                if (dynamic["DATA_TABLE"][tableName] != null)
                                {
                                    foreach (var row in rows)
                                    {                                       
                                        IEnumerable<Cell> cells = row.Descendants<Cell>();
                                        if (cells != null && cells.Count() > 0)
                                        {
                                            foreach (var cell in cells)
                                            {
                                                if (cell != null && cell.DataType != null)
                                                {
                                                    if (cell.DataType != null)
                                                    {
                                                        var element = sharedStringTable.ChildElements[int.Parse(cell.InnerText)];
                                                        if (!string.IsNullOrEmpty(element.InnerText))
                                                        {

                                                            if (element.InnerText.IndexOf("BM_") > -1)
                                                            {
                                                                if (element.InnerText.IndexOf(tableName) > -1)
                                                                {
                                                                    rowTable = true;
                                                                    rowClone = (Row)row.Clone();
                                                                    sheetData.RemoveChild(row);
                                                                    sharedStringTable.Save();
                                                                    workbook.Save();
                                                                    break;
                                                                }

                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                        if (rowTable)
                                        {
                                            break;
                                        }
                                    }
                                    if (rowTable)
                                    {
                                        uint index = 1;
                                        foreach (var data in dynamic["DATA_TABLE"][tableName])
                                        {
                                            if (data != null)
                                            {
                                                string position = (rowClone.RowIndex + index).ToString();

                                                Row newRow = new Row();
                                                newRow.RowIndex = rowClone.RowIndex + index;
                                                IEnumerable<Cell> cells = rowClone.Descendants<Cell>();

                                                foreach (var datItem in data)
                                                {
                                                    var value = "";
                                                    if (datItem.Value != null)
                                                    {
                                                        value = datItem.Value.ToString();
                                                    }
                                                    string columnName = datItem.Name;
                                                    foreach (var item in cells)
                                                    {
                                                        if (item.DataType != null)
                                                        {
                                                            var element = sharedStringTable.ChildElements[int.Parse(item.InnerText)];
                                                            if (element.InnerText.ToString().IndexOf(columnName) > -1)
                                                            {
                                                                Cell cell = new Cell();
                                                                CellValue cellValue = new CellValue();
                                                                cellValue.Text = value;
                                                                cell.CellReference = item.CellReference.Value.ToString().Substring(0, 1) + position;
                                                                cell.Append(cellValue);
                                                                newRow.Append(cell);
                                                            }
                                                        }
                                                    }
                                                }
                                                sheetData.Append(newRow);
                                                workbook.Save();
                                                index++;
                                            }
                                        }
                                    }
                                }                             
                            }
                        }
                    }
                    sharedStringTable.Save();
                    workbook.Save();                   
                }
                memoryStream.Position = 0; //let's rewind it
                memoryStream.Seek(0, SeekOrigin.Begin);
                response = new ResponseFile<MemoryStream>(memoryStream, fileName, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.Fail;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
