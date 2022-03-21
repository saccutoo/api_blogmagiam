using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Templates.API
{
    public static class BookMark
    {
        public static ResponseFile<MemoryStream> ReplaceBookMarkWord(string param,string base64,string fileName)
        {
            ResponseFile<MemoryStream> response = new ResponseFile<MemoryStream>();
            try
            {
                var dynamic = JsonConvert.DeserializeObject<dynamic>(param);

                byte[] bytes = Convert.FromBase64String(base64);
                MemoryStream memoryStream = new MemoryStream(bytes);

                //using (var fileStream = new FileStream("C:\\Users\\vinhtq1\\" + fileName, FileMode.Open, FileAccess.Read))
                //    fileStream.CopyTo(memoryStream);

                using (var document = WordprocessingDocument.Open(memoryStream, true))
                {
                    document.ChangeDocumentType(WordprocessingDocumentType.Document); // change from template to document

                    var body = document.MainDocumentPart.Document.Body;

                    IDictionary<String, BookmarkStart> bookmarkMap = new Dictionary<String, BookmarkStart>();

                    foreach (BookmarkStart bookmarkStart in document.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                    {
                        if (bookmarkStart.Name.ToString().IndexOf("BM_") > -1)
                        {
                            //bookmarkMap[bookmarkStart.Name] = bookmarkStart;
                            Run bookmarkText = bookmarkStart.NextSibling<Run>();
                            if (bookmarkText != null)
                            {
                                if (bookmarkStart.Name.ToString().IndexOf("BM_TABLE") > -1)
                                {
                                    var table = bookmarkStart.Parent.Parent.Parent.Parent;
                                    if (table.GetType() == typeof(DocumentFormat.OpenXml.Wordprocessing.Table))
                                    {
                                        if (dynamic["DATA_TABLE"][bookmarkStart.Name.ToString()] != null)
                                        {
                                            Table myTable = (Table)table;

                                            IEnumerable<TableRow> tableRows = myTable.Descendants<TableRow>();
                                            List<TableRow> newRows = new List<TableRow>();
                                            foreach (var item in tableRows)
                                            {
                                                newRows.Add(item);

                                            }
                                            foreach (var item in newRows)
                                            {
                                                myTable.RemoveChild(item);
                                            }
                                            if (newRows != null && newRows.Count() > 0)
                                            {
                                                int indexRow = 0;
                                                foreach (var row in newRows)
                                                {
                                                    if (indexRow == 0)
                                                    {
                                                        indexRow++;
                                                        myTable.AppendChild(row);
                                                        continue;
                                                    }
                                                    else
                                                    {
                                                        if (indexRow == 1)
                                                        {
                                                            //For list data
                                                            foreach (var item in dynamic["DATA_TABLE"][bookmarkStart.Name.ToString()])
                                                            {
                                                                if (item != null)
                                                                {
                                                                    var newRow = row.CloneNode(true);
                                                                    IEnumerable<TableCell> tableCells = newRow.Descendants<TableCell>();
                                                                    if (tableCells != null && tableCells.Count() > 0)
                                                                    {
                                                                        foreach (var cell in tableCells)
                                                                        {
                                                                            if (cell.InnerText.ToUpper().IndexOf("CALC") > -1)
                                                                            {
                                                                                var calc = cell.InnerText.Replace("CALC<", "").Replace(">", "");
                                                                                foreach (var data in item)
                                                                                {
                                                                                    var value = "0";
                                                                                    if (data.Value != null)
                                                                                    {
                                                                                        value = data.Value.ToString();
                                                                                    }
                                                                                    string columnName = data.Name;
                                                                                    calc = calc.Replace(columnName, value);
                                                                                }
                                                                                var result = new DataTable().Compute(calc, null);

                                                                                Paragraph p = cell.Elements<Paragraph>().First();
                                                                                var rFirst = p.Elements<Run>().First().CloneNode(true);
                                                                                // Find the first run in the paragraph.
                                                                                List<Run> newRuns = new List<Run>();
                                                                                IEnumerable<Run> runs = p.Descendants<Run>();

                                                                                foreach (var r in runs)
                                                                                {
                                                                                    newRuns.Add(r);
                                                                                }
                                                                                foreach (var r in newRuns)
                                                                                {
                                                                                    p.RemoveChild(r);
                                                                                }

                                                                                // Set the text for the run.
                                                                                Text t = rFirst.Elements<Text>().First();
                                                                                t.Text = result == null ? "0" : result.ToString();
                                                                                p.AppendChild(rFirst);
                                                                            }
                                                                            else
                                                                            {
                                                                                if (item[cell.InnerText] != null)
                                                                                {
                                                                                    var value = item[cell.InnerText].Value.ToString();
                                                                                    // Find the first paragraph in the table cell.
                                                                                    Paragraph p = cell.Elements<Paragraph>().First();

                                                                                    // Find the first run in the paragraph.
                                                                                    Run r = p.Elements<Run>().First();

                                                                                    // Set the text for the run.
                                                                                    Text t = r.Elements<Text>().First();
                                                                                    t.Text = value;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    myTable.AppendChild(newRow);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //Tính tổng
                                                            Dictionary<string, string> dicValues = new Dictionary<string, string>();
                                                            var newRow = row.CloneNode(true);
                                                            IEnumerable<TableCell> tableCells = newRow.Descendants<TableCell>();

                                                            if (tableCells != null && tableCells.Count() > 0)
                                                            {
                                                                foreach (var cell in tableCells)
                                                                {

                                                                    if (cell.InnerText.ToUpper().IndexOf("CALC") > -1)
                                                                    {
                                                                        var calc = cell.InnerText.Replace("CALC<", "").Replace(">", "");
                                                                        var splitCalc = calc.Replace("+", " ").Replace("-", " ").Replace("*", " ").Replace("/", " ").Split(" ");
                                                                        foreach (var iSplit in splitCalc)
                                                                        {
                                                                            if (iSplit.ToUpper().IndexOf("SUM_") > -1
                                                                               || iSplit.ToUpper().IndexOf("MINUS_") > -1
                                                                               || iSplit.ToUpper().IndexOf("MULTIPLY_") > -1
                                                                               || iSplit.ToUpper().IndexOf("DIVISION_") > -1)
                                                                            {
                                                                                try
                                                                                {
                                                                                    var dicValue = dicValues[iSplit] as string;
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    var resultString = string.Empty;
                                                                                    #region SUM
                                                                                    if (iSplit.ToUpper().IndexOf("SUM_") > -1)
                                                                                    {
                                                                                        var sumColumn = iSplit.Replace("SUM_", "");
                                                                                        string sumCalc = string.Empty;
                                                                                        int indexS = 0;
                                                                                        foreach (var item in dynamic["DATA_TABLE"][bookmarkStart.Name.ToString()])
                                                                                        {
                                                                                            if (item[sumColumn] != null)
                                                                                            {
                                                                                                if (indexS == 0)
                                                                                                {
                                                                                                    sumCalc = item[sumColumn].Value.ToString();
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    sumCalc += "+" + item[sumColumn].Value.ToString();
                                                                                                }
                                                                                                indexS++;
                                                                                            }
                                                                                        }
                                                                                        if (!string.IsNullOrEmpty(sumCalc))
                                                                                        {
                                                                                            var result = new DataTable().Compute(sumCalc, null);
                                                                                            resultString = result.ToString();
                                                                                        }

                                                                                    }
                                                                                    #endregion

                                                                                    #region MINUS
                                                                                    if (iSplit.ToUpper().IndexOf("MINUS_") > -1)
                                                                                    {

                                                                                        var minusColumn = iSplit.Replace("MINUS_", "");
                                                                                        string minusCalc = string.Empty;
                                                                                        int indexS = 0;
                                                                                        foreach (var item in dynamic["DATA_TABLE"][bookmarkStart.Name.ToString()])
                                                                                        {
                                                                                            if (item[minusColumn] != null)
                                                                                            {
                                                                                                if (indexS == 0)
                                                                                                {
                                                                                                    minusCalc = item[minusColumn].Value.ToString();
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    minusCalc += "-" + item[minusColumn].Value.ToString();
                                                                                                }
                                                                                                indexS++;
                                                                                            }
                                                                                        }

                                                                                        if (!string.IsNullOrEmpty(minusCalc))
                                                                                        {
                                                                                            var result = new DataTable().Compute(minusCalc, null);
                                                                                            resultString = result.ToString();
                                                                                        }

                                                                                    }
                                                                                    #endregion

                                                                                    #region MULTIPLY
                                                                                    if (iSplit.ToUpper().IndexOf("MULTIPLY_") > -1)
                                                                                    {

                                                                                        var multiplyColumn = iSplit.Replace("MULTIPLY_", "");
                                                                                        string multiplyCalc = string.Empty;
                                                                                        int indexS = 0;
                                                                                        foreach (var item in dynamic["DATA_TABLE"][bookmarkStart.Name.ToString()])
                                                                                        {
                                                                                            if (item[multiplyColumn] != null)
                                                                                            {
                                                                                                if (indexS == 0)
                                                                                                {
                                                                                                    multiplyCalc = item[multiplyColumn].Value.ToString();
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    multiplyCalc += "*" + item[multiplyColumn].Value.ToString();
                                                                                                }
                                                                                                indexS++;
                                                                                            }
                                                                                        }

                                                                                        if (!string.IsNullOrEmpty(multiplyCalc))
                                                                                        {
                                                                                            var result = new DataTable().Compute(multiplyCalc, null);
                                                                                            resultString = result.ToString();
                                                                                        }

                                                                                    }
                                                                                    #endregion

                                                                                    #region DIVISION_
                                                                                    if (iSplit.ToUpper().IndexOf("DIVISION_") > -1)
                                                                                    {
                                                                                        var divisionColumn = iSplit.Replace("DIVISION_", "");
                                                                                        string divisionCalc = string.Empty;
                                                                                        int indexS = 0;
                                                                                        foreach (var item in dynamic["DATA_TABLE"][bookmarkStart.Name.ToString()])
                                                                                        {
                                                                                            if (item[divisionColumn] != null)
                                                                                            {
                                                                                                if (indexS == 0)
                                                                                                {
                                                                                                    divisionCalc = item[divisionColumn].Value.ToString();
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    divisionCalc += "/" + item[divisionColumn].Value.ToString();
                                                                                                }
                                                                                                indexS++;
                                                                                            }
                                                                                        }

                                                                                        if (!string.IsNullOrEmpty(divisionCalc))
                                                                                        {
                                                                                            var result = new DataTable().Compute(divisionCalc, null);
                                                                                            double round = 0;
                                                                                            if (result != null)
                                                                                            {
                                                                                                round = Convert.ToDouble(result);
                                                                                            }
                                                                                            resultString = Math.Round(round, 4).ToString();
                                                                                        }

                                                                                    }
                                                                                    #endregion

                                                                                    dicValues.Add(iSplit, string.IsNullOrEmpty(resultString) ? "0" : resultString.ToString());
                                                                                }

                                                                            }
                                                                        }

                                                                        foreach (var item in dicValues)
                                                                        {
                                                                            calc = calc.Replace(item.Key, item.Value);
                                                                        }
                                                                        var resultNew = new DataTable().Compute(calc, null);

                                                                        Paragraph p = cell.Elements<Paragraph>().First();
                                                                        var rFirst = p.Elements<Run>().First().CloneNode(true);
                                                                        // Find the first run in the paragraph.
                                                                        List<Run> newRuns = new List<Run>();
                                                                        IEnumerable<Run> runs = p.Descendants<Run>();

                                                                        foreach (var r in runs)
                                                                        {
                                                                            newRuns.Add(r);
                                                                        }
                                                                        foreach (var r in newRuns)
                                                                        {
                                                                            p.RemoveChild(r);
                                                                        }

                                                                        // Set the text for the run.
                                                                        Text t = rFirst.Elements<Text>().First();
                                                                        t.Text = resultNew == null ? "0" : resultNew.ToString();
                                                                        p.AppendChild(rFirst);
                                                                    }
                                                                    else if (cell.InnerText.ToUpper().IndexOf("SUM_") > -1
                                                                       || cell.InnerText.ToUpper().IndexOf("MINUS_") > -1
                                                                       || cell.InnerText.ToUpper().IndexOf("MULTIPLY_") > -1
                                                                       || cell.InnerText.ToUpper().IndexOf("DIVISION_") > -1)
                                                                    {

                                                                        var resultString = string.Empty;
                                                                        #region SUM
                                                                        if (cell.InnerText.ToUpper().IndexOf("SUM_") > -1)
                                                                        {
                                                                            var sumColumn = cell.InnerText.Replace("SUM_", "");
                                                                            string sumCalc = string.Empty;
                                                                            int indexS = 0;
                                                                            foreach (var item in dynamic["DATA_TABLE"][bookmarkStart.Name.ToString()])
                                                                            {
                                                                                if (item[sumColumn] != null)
                                                                                {
                                                                                    if (indexS == 0)
                                                                                    {
                                                                                        sumCalc = item[sumColumn].Value.ToString();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        sumCalc += "+" + item[sumColumn].Value.ToString();
                                                                                    }
                                                                                    indexS++;
                                                                                }
                                                                            }
                                                                            if (!string.IsNullOrEmpty(sumCalc))
                                                                            {
                                                                                var result = new DataTable().Compute(sumCalc, null);
                                                                                resultString = result.ToString();
                                                                            }

                                                                        }
                                                                        #endregion

                                                                        #region MINUS
                                                                        if (cell.InnerText.ToUpper().IndexOf("MINUS_") > -1)
                                                                        {

                                                                            var minusColumn = cell.InnerText.Replace("MINUS_", "");
                                                                            string minusCalc = string.Empty;
                                                                            int indexS = 0;
                                                                            foreach (var item in dynamic["DATA_TABLE"][bookmarkStart.Name.ToString()])
                                                                            {
                                                                                if (item[minusColumn] != null)
                                                                                {
                                                                                    if (indexS == 0)
                                                                                    {
                                                                                        minusCalc = item[minusColumn].Value.ToString();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        minusCalc += "-" + item[minusColumn].Value.ToString();
                                                                                    }
                                                                                    indexS++;
                                                                                }
                                                                            }

                                                                            if (!string.IsNullOrEmpty(minusCalc))
                                                                            {
                                                                                var result = new DataTable().Compute(minusCalc, null);
                                                                                resultString = result.ToString();
                                                                            }

                                                                        }
                                                                        #endregion

                                                                        #region MULTIPLY
                                                                        if (cell.InnerText.ToUpper().IndexOf("MULTIPLY_") > -1)
                                                                        {

                                                                            var multiplyColumn = cell.InnerText.Replace("MULTIPLY_", "");
                                                                            string multiplyCalc = string.Empty;
                                                                            int indexS = 0;
                                                                            foreach (var item in dynamic["DATA_TABLE"][bookmarkStart.Name.ToString()])
                                                                            {
                                                                                if (item[multiplyColumn] != null)
                                                                                {
                                                                                    if (indexS == 0)
                                                                                    {
                                                                                        multiplyCalc = item[multiplyColumn].Value.ToString();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        multiplyCalc += "*" + item[multiplyColumn].Value.ToString();
                                                                                    }
                                                                                    indexS++;
                                                                                }
                                                                            }

                                                                            if (!string.IsNullOrEmpty(multiplyCalc))
                                                                            {
                                                                                var result = new DataTable().Compute(multiplyCalc, null);
                                                                                resultString = result.ToString();
                                                                            }

                                                                        }
                                                                        #endregion

                                                                        #region DIVISION_
                                                                        if (cell.InnerText.ToUpper().IndexOf("DIVISION_") > -1)
                                                                        {

                                                                            var divisionColumn = cell.InnerText.Replace("DIVISION_", "");
                                                                            string divisionCalc = string.Empty;
                                                                            int indexS = 0;
                                                                            foreach (var item in dynamic["DATA_TABLE"][bookmarkStart.Name.ToString()])
                                                                            {
                                                                                if (item[divisionColumn] != null)
                                                                                {
                                                                                    if (indexS == 0)
                                                                                    {
                                                                                        divisionCalc = item[divisionColumn].Value.ToString();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        divisionCalc += "/" + item[divisionColumn].Value.ToString();
                                                                                    }
                                                                                    indexS++;
                                                                                }
                                                                            }

                                                                            if (!string.IsNullOrEmpty(divisionCalc))
                                                                            {
                                                                                var result = new DataTable().Compute(divisionCalc, null);
                                                                                double round = 0;
                                                                                if (result != null)
                                                                                {
                                                                                    round = Convert.ToDouble(result);
                                                                                }
                                                                                resultString = Math.Round(round, 4).ToString();
                                                                            }

                                                                        }
                                                                        #endregion

                                                                        dicValues.Add(cell.InnerText, string.IsNullOrEmpty(resultString) ? "0" : resultString.ToString());
                                                                        if (!string.IsNullOrEmpty(resultString))
                                                                        {
                                                                            Paragraph p = cell.Elements<Paragraph>().First();
                                                                            var rFirst = p.Elements<Run>().First().CloneNode(true);
                                                                            // Find the first run in the paragraph.
                                                                            List<Run> newRuns = new List<Run>();
                                                                            IEnumerable<Run> runs = p.Descendants<Run>();

                                                                            foreach (var r in runs)
                                                                            {
                                                                                newRuns.Add(r);
                                                                            }
                                                                            foreach (var r in newRuns)
                                                                            {
                                                                                p.RemoveChild(r);
                                                                            }

                                                                            // Set the text for the run.
                                                                            Text t = rFirst.Elements<Text>().First();
                                                                            t.Text = string.IsNullOrEmpty(resultString) ? "0" : resultString;
                                                                            p.AppendChild(rFirst);
                                                                        }

                                                                    }
                                                                }
                                                                myTable.AppendChild(newRow);
                                                            }
                                                        }
                                                    }
                                                    indexRow++;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (dynamic[bookmarkStart.Name.ToString()] != null)
                                    {
                                        bookmarkText.GetFirstChild<Text>().Text = dynamic[bookmarkStart.Name.ToString()].ToString();
                                    }
                                }
                            }
                        }
                    }
                    document.MainDocumentPart.Document.Save();
                    document.Close();
                }
                memoryStream.Position = 0; //let's rewind it
                response = new ResponseFile<MemoryStream>(memoryStream, fileName, "application/msword");
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode= StatusCode.Fail;
            }
            return response;

        }
    }
}
