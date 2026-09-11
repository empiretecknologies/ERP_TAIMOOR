using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Empire_ERP.Controllers
{
    public class ReportController : Controller
    {

        [HttpPost]
        [Consumes("application/json")]
        public IActionResult GeneratePDF([FromBody] PDFRequest request)
        {
            BaseColor themeColor = new BaseColor(0x05, 0x5A, 0x87);

            MemoryStream stream = new MemoryStream();
            Rectangle pageSize = request.IsLandscape ? PageSize.A4.Rotate() : PageSize.A4;

            Document document = new Document(pageSize, 25f, 25f, 65f, 40f);
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.CloseStream = false;

            string fontPathRegular = Path.Combine(Environment.CurrentDirectory, "wwwroot\\fonts\\TCM_____.TTF");
            BaseFont bfRegular = BaseFont.CreateFont(fontPathRegular, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font defaultFont = new Font(bfRegular, 9, Font.NORMAL, BaseColor.BLACK);

            Font GetFont(float size, int style, BaseColor color)
            {
                return new Font(defaultFont.BaseFont, size, style, color);
            }

            writer.PageEvent = new PdfPageHeader
            {
                CompanyName = request.CompanyName,
                ReportName = request.ReportName,
                FromDate = request.From,
                ToDate = request.To,
                PrintedDate = DateTime.Now.ToString("dd-MM-yyyy"),
                PrintedTime = DateTime.Now.ToString("hh:mm tt"),
                DefaultFont = defaultFont
            };

            document.Open();

            int gridCount = 0;
            foreach (var gridReq in request.Grids)
            {
                if (gridReq.GridData == null || gridReq.GridData.Count == 0)
                    continue;

                var flatList = gridReq.GridData.ToObject<List<Dictionary<string, object>>>();

                if (flatList.Count == 0)
                    continue;

                // Build from every row's keys so a missing group field on row 1
                // cannot leave later group columns as DBNull on that row.
                DataTable dt = BuildGridDataTable(flatList, gridReq.GroupColumnsCap);

                if (dt == null || dt.Rows.Count == 0)
                    continue;

                FillBlankNestedGroupValues(dt, gridReq.GroupColumnsCap);
                SortDataTableByGroups(ref dt, gridReq.GroupColumnsCap);

                var visibleCols = dt.Columns
                    .Cast<DataColumn>()
                    .Where(c => !gridReq.GroupColumnsCap.Contains(c.ColumnName))
                    .ToList();

                GridReport grid = new GridReport
                {
                    Data = dt,
                    ReportName = gridReq.GridTitle,
                    GroupColumnsCap = gridReq.GroupColumnsCap,
                    VisibleColumns = visibleCols
                };

                AddGridToPDF(document, grid, themeColor, defaultFont, request.lastDate, request.lastAmount, gridCount);
                gridCount++;
            }

            document.Close();
            stream.Position = 0;

            return File(stream, "application/pdf", "FullReport.pdf");

        }

        void AddMultiGroupedTableList(Document document, DataTable dt, List<string> groupColumns, List<DataColumn> visibleColumns, BaseColor themeColor, Font defaultFont, DateTime? lastDate, decimal? lastAmount)
        {
            if (dt == null || dt.Rows.Count == 0) return;
            if (groupColumns == null) groupColumns = new List<string>();

            Font GetFont(float size, int style, BaseColor color)
            {
                return new Font(defaultFont.BaseFont, size, style, color);
            }


            Dictionary<string, float> columnWidths = new Dictionary<string, float>()
            {
                { "Date", 55f },
                { "Due Date", 60f },
                { "Transaction #", 150f },
                { "Doc", 30f },
                { "Sale Rate", 60f },

                { "Chq No", 80f },
                { "Chq Date", 70f },
                { "Description", 240f },
                { "Debit", 60f },
                { "Credit", 60f },
                { "Balance", 70f },
                { "Color", 75f },
                { "Size", 75f },
                { "Current Stock", 70f },
                //{ "Sale Rate", 60f },
                { "Qty", 55f },
                { "Disc %", 70f },
                { "Amount", 65f },
                { "Net Amount", 65f },
            };

            float[] widths = new float[visibleColumns.Count];
            for (int i = 0; i < visibleColumns.Count; i++)
            {
                string colName = visibleColumns[i].ColumnName;
                widths[i] = columnWidths.ContainsKey(colName) ? columnWidths[colName] : 80f;
            }

            PdfPTable table = new PdfPTable(visibleColumns.Count);
            table.WidthPercentage = 100;
            table.SetWidths(widths);
            table.SplitRows = true;

            foreach (var col in visibleColumns)
            {
                string formatted = Regex.Replace(col.ColumnName, "([a-z])([A-Z])", "$1 $2");
                if (!string.IsNullOrEmpty(formatted))
                    formatted = char.ToUpper(formatted[0]) + formatted.Substring(1);

                BaseFont bf = BaseFont.CreateFont(@"C:\Windows\Fonts\arialbd.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                Font customBoldFont = new Font(bf, 7, Font.NORMAL, themeColor);

                PdfPCell h = new PdfPCell(new Phrase(formatted, customBoldFont));
                h.HorizontalAlignment = Element.ALIGN_CENTER;
                h.Padding = 4;
                h.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                h.BorderColor = new BaseColor(220, 220, 220);
                h.BorderWidthTop = 0.8f;
                h.BorderWidthBottom = 0.8f;

                table.AddCell(h);
            }
            table.HeaderRows = 1;

            List<string> lastGroupValues = new List<string>();
            List<Dictionary<string, decimal>> groupTotals = new List<Dictionary<string, decimal>>();

            foreach (var col in groupColumns)
            {
                lastGroupValues.Add("");
                Dictionary<string, decimal> totals = new Dictionary<string, decimal>();
                foreach (var c in visibleColumns)
                    if (IsNumericColumn(c.ColumnName)) totals[c.ColumnName] = 0;
                groupTotals.Add(totals);
            }

            Dictionary<string, decimal> grandTotals = new Dictionary<string, decimal>();
            foreach (var c in visibleColumns)
                if (IsNumericColumn(c.ColumnName))
                    grandTotals[c.ColumnName] = 0;
            foreach (DataRow r in dt.Rows)
            {
                for (int level = 0; level < groupColumns.Count; level++)
                {
                    string currentValue = r[groupColumns[level]]?.ToString() ?? "";


                    if (!string.IsNullOrWhiteSpace(currentValue) && currentValue != lastGroupValues[level])
                    {

                        if (!string.IsNullOrEmpty(lastGroupValues[level]))
                        {
                            AddTotals(table, visibleColumns, groupTotals[level], GetFont, lastDate, lastAmount);
                        }
                        bool hasChildGroup = level < groupColumns.Count - 1;
                        AddGroupHeader(table, groupColumns[level], currentValue, visibleColumns.Count, GetFont, level, hasChildGroup);


                        for (int resetLevel = level; resetLevel < groupColumns.Count; resetLevel++)
                        {
                            foreach (var key in groupTotals[resetLevel].Keys.ToList())
                                groupTotals[resetLevel][key] = 0;
                        }


                        lastGroupValues[level] = currentValue;
                        for (int i = level + 1; i < groupColumns.Count; i++)
                            lastGroupValues[i] = "";
                    }
                }


                for (int colIndex = 0; colIndex < visibleColumns.Count; colIndex++)
                {
                    var col = visibleColumns[colIndex];
                    string val = r[col.ColumnName]?.ToString() ?? "";
                    PdfPCell cell;
                    bool isTransactionColumn = col.ColumnName == "Transaction #";
                    if (IsNumericColumn(col.ColumnName))
                    {
                        decimal num = 0;
                        if (IsNumericValue(val))
                            num = Convert.ToDecimal(val.Replace(",", ""));


                        for (int level = 0; level < groupColumns.Count; level++)
                            groupTotals[level][col.ColumnName] += num;
                        grandTotals[col.ColumnName] += num;

                        string displayValue = string.IsNullOrWhiteSpace(val) || val == "0" ? "" : string.Format("{0:N0}", num);
                        cell = new PdfPCell(new Phrase(displayValue, GetFont(7, Font.NORMAL, isTransactionColumn ? themeColor : BaseColor.BLACK)))
                        { HorizontalAlignment = Element.ALIGN_RIGHT };
                    }
                    //else
                    //{
                    //    string newVal = val == "1900-01-01" ? "" : val;
                    //    if (col.ColumnName.Contains("Date") && DateTime.TryParse(newVal, out DateTime parsedDate))
                    //        newVal = parsedDate.ToString("dd-MM-yy");

                    //    cell = new PdfPCell(new Phrase(newVal, GetFont(7, Font.NORMAL, isTransactionColumn ? themeColor : BaseColor.BLACK)))
                    //    { HorizontalAlignment = Element.ALIGN_CENTER };
                    //}
                    else
                    {
                        string newVal = val == "1900-01-01" ? "" : val;
                        if (col.ColumnName.Contains("Date") && DateTime.TryParse(newVal, out DateTime parsedDate))
                            newVal = parsedDate.ToString("dd-MM-yy");

                        if (col.ColumnName.Equals("doc", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(newVal))
                        {
                            //try
                            //{
                            //    string fullImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newVal.TrimStart('\\', '/'));

                            //    if (System.IO.File.Exists(fullImagePath))
                            //    {
                            //        using (var originalImage = System.Drawing.Image.FromFile(fullImagePath))
                            //        {

                            //        }
                            //        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(fullImagePath);

                            //        img.ScaleToFit(40f, 40f);

                            //        cell = new PdfPCell(img)
                            //        {
                            //            HorizontalAlignment = Element.ALIGN_CENTER,
                            //            VerticalAlignment = Element.ALIGN_MIDDLE
                            //        };
                            //    }
                            //    else
                            //    {
                            //        cell = new PdfPCell(new Phrase(newVal, GetFont(7, Font.NORMAL, isTransactionColumn ? themeColor : BaseColor.BLACK)))
                            //        { HorizontalAlignment = Element.ALIGN_CENTER };
                            //    }
                            //}
                            //catch (Exception)
                            //{
                            //    cell = new PdfPCell(new Phrase(newVal, GetFont(7, Font.NORMAL, isTransactionColumn ? themeColor : BaseColor.BLACK)))
                            //    { HorizontalAlignment = Element.ALIGN_CENTER };
                            //}
                            try
                            {
                                string fullImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newVal.TrimStart('\\', '/'));

                                if (System.IO.File.Exists(fullImagePath))
                                {
                                    using (var originalImage = System.Drawing.Image.FromFile(fullImagePath))
                                    {
                                        // 1. Resolution thoda barha diya taake blur na ho (120x120 pixels)
                                        int targetWidth = 120;
                                        int targetHeight = 120;

                                        using (var resizedImage = new System.Drawing.Bitmap(targetWidth, targetHeight))
                                        {
                                            using (var graphics = System.Drawing.Graphics.FromImage(resizedImage))
                                            {
                                                // 2. High Quality settings taake pixels na phatein
                                                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                                                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                                                graphics.DrawImage(originalImage, 0, 0, targetWidth, targetHeight);
                                            }

                                            // 3. Encoder settings
                                            var jpgEncoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);
                                            var myEncoder = System.Drawing.Imaging.Encoder.Quality;
                                            var myEncoderParameters = new System.Drawing.Imaging.EncoderParameters(1);

                                            // Quality ko 45% se barha kar 80% kar diya (Isse image bilkul clear hojayegi)
                                            var myEncoderParameter = new System.Drawing.Imaging.EncoderParameter(myEncoder, 80L);
                                            myEncoderParameters.Param[0] = myEncoderParameter;

                                            using (var ms = new MemoryStream())
                                            {
                                                resizedImage.Save(ms, jpgEncoder, myEncoderParameters);

                                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ms.ToArray());

                                                // iTextSharp cell me fit karne ke liye dobara 40f scale kar dein
                                                img.ScaleToFit(40f, 40f);

                                                cell = new PdfPCell(img)
                                                {
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE
                                                };
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    cell = new PdfPCell(new Phrase(newVal, GetFont(7, Font.NORMAL, isTransactionColumn ? themeColor : BaseColor.BLACK)))
                                    { HorizontalAlignment = Element.ALIGN_CENTER };
                                }
                            }
                            catch (Exception)
                            {
                                cell = new PdfPCell(new Phrase(newVal, GetFont(7, Font.NORMAL, isTransactionColumn ? themeColor : BaseColor.BLACK)))
                                { HorizontalAlignment = Element.ALIGN_CENTER };
                            }
                        }
                        else
                        {
                            cell = new PdfPCell(new Phrase(newVal, GetFont(7, Font.NORMAL, isTransactionColumn ? themeColor : BaseColor.BLACK)))
                            { HorizontalAlignment = Element.ALIGN_CENTER };
                        }
                    }

                    if (colIndex == 0) cell.Border &= ~Rectangle.LEFT_BORDER;
                    if (colIndex == visibleColumns.Count - 1) cell.Border &= ~Rectangle.RIGHT_BORDER;

                    cell.BorderColor = new BaseColor(220, 220, 220);
                    cell.Padding = 2;

                    table.AddCell(cell);
                }
            }
            for (int level = groupColumns.Count - 1; level >= 0; level--)
            {
                if (!string.IsNullOrEmpty(lastGroupValues[level]))
                {
                    AddTotals(table, visibleColumns, groupTotals[level], GetFont, lastDate, lastAmount);
                }
            }
            AddTotals(table, visibleColumns, grandTotals, GetFont, lastDate, lastAmount);
            document.Add(table);
        }

        void AddGridToPDF(Document document, GridReport grid, BaseColor themeColor, Font defaultFont, DateTime? lastDate, decimal? lastAmount, int gridCount)
        {
            if (grid.Data == null || grid.Data.Rows.Count == 0) return;

            if (gridCount == 1)
            {
                // 1️⃣ Add header table
                PdfPTable headerTable = new PdfPTable(3);
                headerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                headerTable.LockedWidth = true;

                headerTable.SetWidths(new float[] { 60f, 20f, 20f });

                BaseFont bf = BaseFont.CreateFont(@"C:\Windows\Fonts\arialbd.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                Font customBoldFont = new Font(bf, 6.5f, Font.NORMAL, BaseColor.WHITE);

                PdfPCell titleCell = new PdfPCell(new Phrase($"{grid.ReportName}", customBoldFont))
                { BackgroundColor = themeColor, Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5 };
                headerTable.AddCell(titleCell);

                PdfPCell lastDateCell = new PdfPCell(new Phrase("Last Date : " + lastDate?.ToString("dd-MM-yyyy"), customBoldFont))
                { BackgroundColor = themeColor, Border = Rectangle.LEFT_BORDER, BorderColor = new BaseColor(220, 220, 220), Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER };
                headerTable.AddCell(lastDateCell);

                string formattedAmount = decimal.TryParse(lastAmount?.ToString(), out decimal amt) ? amt.ToString("N0") : lastAmount?.ToString();
                PdfPCell lastAmountCell = new PdfPCell(new Phrase("Last Amount : " + formattedAmount, customBoldFont))
                { BackgroundColor = themeColor, Border = Rectangle.LEFT_BORDER, BorderColor = new BaseColor(220, 220, 220), Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER };
                headerTable.AddCell(lastAmountCell);

                headerTable.SpacingBefore = 3f;
                headerTable.KeepTogether = true;
                headerTable.SplitRows = false;
                document.Add(headerTable);
            }

            if (grid.GroupColumnsCap != null && grid.GroupColumnsCap.Count > 0)
            {
                GenerateDynamicMultiLevelGroupedPDF(
                    document: document,
                    dt: grid.Data,
                    groupColumns: grid.GroupColumnsCap,
                    visibleColumns: grid.VisibleColumns,
                    themeColor: themeColor,
                    defaultFont: defaultFont,
                    lastDate: lastDate,
                    lastAmount: lastAmount
                );
            }
            else
            {
                AddMultiGroupedTableList(document, grid.Data, grid.GroupColumnsCap ?? new List<string>(), grid.VisibleColumns, themeColor, defaultFont, lastDate, lastAmount);
            }
        }

        bool IsNumericColumn(string col)
        {
            return new[] { "Amount", "Debit", "Credit", "Balance", "Total", "Qty","Due Year", "Net Amount", "Disc Amt", "Disc %", "Remaining", "Advance",
                         "Tax Amt " ,"Bill Amount", "Recieved Amt ", "Tax" ,"Commission Value" , "Commission %","Discount","Quantity","Due Month","Due Days","0 To 15",
                "16 To 30","31 To 45","46 To 60","61 To 90","91 To 120","120 Plus","Balance"}
                   .Any(x => col.ToLower().Contains(x.ToLower()));
        }


        bool IsNumericValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            value = value.Replace(",", "").Trim();
            return decimal.TryParse(value, out _);
        }

        void AddGroupHeader(PdfPTable table, string columnName, string value, int colSpan, Func<float, int, BaseColor, Font> GetFont, int level, bool hasChildGroup)
        {
            BaseFont bf = BaseFont.CreateFont(
                @"C:\Windows\Fonts\arialbd.ttf",
                BaseFont.IDENTITY_H,
                BaseFont.EMBEDDED
            );

            float fontSize = level switch
            {
                0 => 8f, // Act Name
                1 => 7f, // Party Name
                _ => 6f  // Account Name or deeper
            };

            Font font = new Font(bf, fontSize, Font.NORMAL, BaseColor.BLACK);

            PdfPCell cell = new PdfPCell(
                new Phrase($"{columnName} : {value}", font)
            );

            cell.Colspan = colSpan;
            cell.PaddingTop = 5;
            cell.PaddingBottom = 5;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;

            cell.Border = Rectangle.NO_BORDER;

            table.AddCell(cell);
        }

        void AddTotals(PdfPTable table, List<DataColumn> cols, Dictionary<string, decimal> totals, Func<float, int, BaseColor, Font> GetFont, DateTime? lastDate = null, decimal? lastAmount = null, bool IslastDate = false)
        {
            decimal debitTotal = totals.ContainsKey("Debit") ? totals["Debit"] : 0;
            decimal creditTotal = totals.ContainsKey("Credit") ? totals["Credit"] : 0;
            decimal balance = debitTotal - creditTotal;

            HashSet<string> totalColumns = IslastDate
                ? new HashSet<string> { "Debit", "Credit", "Amount", "Due Amount", "NetAmount", "Qty", "Disc Amt", "Disc %" , "Remaining", "Advance", "Tax Amt" ,
                    "Bill Amount", "Recieved Amt ", "Tax", "Commission Value", "Commission %", "Discount" ,"Total Amount","0 To 15",
                "16 To 30","31 To 45","46 To 60","61 To 90","91 To 120","120 Plus"}
                : new HashSet<string> { "Debit", "Credit", "Amount", "Due Amount", "Description", "Net Amount", "Qty", "Disc Amt", "Disc %" , "Remaining",
                    "Advance","Tax %", "Tax Amt", "Bill Amount", "Recieved Amt","Commission Value", "Commission %","Discount","Quantity","Total Amount","0 To 15",
                "16 To 30","31 To 45","46 To 60","61 To 90","91 To 120","120 Plus","Balance" };

            BaseFont bf = BaseFont.CreateFont(
                @"C:\Windows\Fonts\arialbd.ttf",
                BaseFont.IDENTITY_H,
                BaseFont.EMBEDDED);

            Font customBoldFont = new Font(bf, 6.5f, Font.NORMAL, BaseColor.BLACK);
            string[] months = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
            for (int i = 0; i < cols.Count; i++)
            {
                var col = cols[i];
                PdfPCell cell;
                bool isMonthColumn = months.Any(m => col.ColumnName.StartsWith(m, StringComparison.OrdinalIgnoreCase));
                if ((totals.ContainsKey(col.ColumnName) && totalColumns.Contains(col.ColumnName)) || isMonthColumn)
                {
                    decimal totalValue = 0;
                    if (totals.ContainsKey(col.ColumnName))
                    {
                        totalValue = totals[col.ColumnName];
                    }
                    else
                    {
                        totalValue = 0;
                    }
                    string val;
                    BaseColor textColor = BaseColor.BLACK;

                    if (totalValue == 0)
                    {
                        val = "";
                    }
                    else if (totalValue < 0)
                    {
                        // Negative value: show in round brackets and red color
                        val = $"({string.Format("{0:N0}", Math.Abs(totalValue))})";
                        textColor = BaseColor.RED;
                    }
                    else
                    {
                        // Positive value: normal format
                        val = string.Format("{0:N0}", totalValue);
                    }

                    Font totalFont = new Font(bf, 6.5f, Font.NORMAL, textColor);
                    cell = new PdfPCell(new Phrase(val, totalFont));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                }
                else
                {
                    cell = new PdfPCell(new Phrase("", customBoldFont));
                }

                cell.Padding = 4;
                cell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                cell.BorderColor = new BaseColor(220, 220, 220);
                cell.BackgroundColor = BaseColor.WHITE;

                if (i == 0)
                    cell.Border &= ~Rectangle.LEFT_BORDER;

                if (i == cols.Count - 1)
                    cell.Border &= ~Rectangle.RIGHT_BORDER;

                table.AddCell(cell);
            }

        }

        void GenerateDynamicMultiLevelGroupedPDF(
            Document document,
            DataTable dt,
            List<string> groupColumns,
            List<DataColumn> visibleColumns,
            BaseColor themeColor,
            Font defaultFont,
            DateTime? lastDate = null,
            decimal? lastAmount = null)
        {
            if (dt == null || dt.Rows.Count == 0) return;
            if (groupColumns == null || groupColumns.Count == 0) return;
            if (visibleColumns == null || visibleColumns.Count == 0) return;

            // Font helper function
            Font GetFont(float size, int style, BaseColor color)
            {
                return new Font(defaultFont.BaseFont, size, style, color);
            }

            // Define column widths
            Dictionary<string, float> columnWidths = new Dictionary<string, float>()
            {
                { "Date", 55f },
                { "Due Date", 60f },
                { "Transaction #", 150f },
                { "Doc", 80f },
                { "Bill Type", 80f },
                { "Chq No", 80f },
                { "Chq Date", 70f },
                { "Description", 240f },
                { "Debit", 60f },
                { "Credit", 60f },
                { "Balance", 70f },
                { "Amount", 65f },
                { "Due Amount", 100f },
                { "Due Year", 80f },
                { "Due Month", 90f },
                { "Due Days", 80f },
                { "Color", 75f },
                { "Size", 75f },
                { "Qty", 55f },
                { "Disc %", 70f },
                { "Net Amount", 65f },
            };

            // Calculate column widths
            float[] widths = new float[visibleColumns.Count];
            for (int i = 0; i < visibleColumns.Count; i++)
            {
                string colName = visibleColumns[i].ColumnName;
                widths[i] = columnWidths.ContainsKey(colName) ? columnWidths[colName] : 80f;
            }

            // Create main table
            PdfPTable table = new PdfPTable(visibleColumns.Count);
            table.WidthPercentage = 100;
            table.SetWidths(widths);
            table.SplitRows = true;

            List<string> currentGroupValues = new List<string>();
            for (int i = 0; i < groupColumns.Count; i++)
                currentGroupValues.Add("");

            List<Dictionary<string, decimal>> groupLevelTotals = new List<Dictionary<string, decimal>>();
            Dictionary<string, decimal> grandTotals = new Dictionary<string, decimal>();
            var monthPattern = @"\b(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec|January|February|March|April|June|July|August|September|October|November|December)\b";

            for (int i = 0; i < groupColumns.Count; i++)
            {
                Dictionary<string, decimal> levelTotals = new Dictionary<string, decimal>();
                    foreach (var col in visibleColumns)
                {
                    if (IsNumericColumn(col.ColumnName) || Regex.IsMatch(col.ColumnName, monthPattern, RegexOptions.IgnoreCase))
                        levelTotals[col.ColumnName] = 0;
                }
                groupLevelTotals.Add(levelTotals);
            }

            // Initialize grand totals
            foreach (var col in visibleColumns)
            {
                if (IsNumericColumn(col.ColumnName) || Regex.IsMatch(col.ColumnName, monthPattern, RegexOptions.IgnoreCase))
                    grandTotals[col.ColumnName] = 0;
            }

            BaseColor lightGrayBorder = new BaseColor(220, 220, 220);
            BaseFont bfBold = BaseFont.CreateFont(
                @"C:\Windows\Fonts\arialbd.ttf",
                BaseFont.IDENTITY_H,
                BaseFont.EMBEDDED
            );

            // Loop through all data rows
            foreach (DataRow row in dt.Rows)
            {
                List<string> rowGroupValues = new List<string>(groupColumns.Count);
                for (int i = 0; i < groupColumns.Count; i++)
                    rowGroupValues.Add(GetGroupCellValue(row, groupColumns[i]));

                // First level whose value differs from the open group.
                // Headers for this level AND every deeper level must be written
                // before the data row, otherwise the first child row is printed
                // under the parent (e.g. Model's first row under Client).
                int changeLevel = -1;
                for (int level = 0; level < groupColumns.Count; level++)
                {
                    if (!string.Equals(rowGroupValues[level], currentGroupValues[level], StringComparison.Ordinal))
                    {
                        changeLevel = level;
                        break;
                    }
                }

                if (changeLevel >= 0)
                {
                    for (int level = groupColumns.Count - 1; level >= changeLevel; level--)
                    {
                        if (!string.IsNullOrEmpty(currentGroupValues[level]))
                            AddTotals(table, visibleColumns, groupLevelTotals[level], GetFont, lastDate, lastAmount);
                    }

                    for (int level = changeLevel; level < groupColumns.Count; level++)
                    {
                        string groupColumn = groupColumns[level];
                        string currentGroupValue = rowGroupValues[level];
                        currentGroupValues[level] = currentGroupValue;

                        foreach (var key in groupLevelTotals[level].Keys.ToList())
                            groupLevelTotals[level][key] = 0;

                        if (string.IsNullOrEmpty(currentGroupValue))
                            continue;

                        if (level == 0)
                        {
                            Font firstGroupFont = new Font(bfBold, 7.5f, Font.NORMAL, BaseColor.BLACK);
                            PdfPCell firstGroupCell = new PdfPCell(new Phrase($"{groupColumn} : {currentGroupValue}", firstGroupFont));
                            firstGroupCell.Colspan = visibleColumns.Count;
                            firstGroupCell.PaddingTop = 5;
                            firstGroupCell.PaddingBottom = 5;
                            firstGroupCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            firstGroupCell.BackgroundColor = BaseColor.WHITE;

                            bool hasChildGroups = groupColumns.Count > 1 && dt.AsEnumerable()
                                .Any(r => GetGroupCellValue(r, groupColumn) == currentGroupValue &&
                                          !string.IsNullOrWhiteSpace(GetGroupCellValue(r, groupColumns[1])));

                            if (hasChildGroups)
                            {
                                firstGroupCell.Border = Rectangle.BOTTOM_BORDER;
                                firstGroupCell.BorderWidthBottom = 0.8f;
                                firstGroupCell.BorderColorBottom = lightGrayBorder;
                            }
                            else
                            {
                                firstGroupCell.Border = Rectangle.NO_BORDER;
                            }

                            table.AddCell(firstGroupCell);

                            Font headerFont = new Font(bfBold, 7, Font.NORMAL, themeColor);
                            foreach (var col in visibleColumns)
                            {
                                string formatted = Regex.Replace(col.ColumnName, "([a-z])([A-Z])", "$1 $2");
                                if (!string.IsNullOrEmpty(formatted))
                                    formatted = char.ToUpper(formatted[0]) + formatted.Substring(1);

                                PdfPCell headerCell = new PdfPCell(new Phrase(formatted, headerFont));

                                string[] rightAlignColumns = { "Debit", "Credit", "Amount", "Net Amount", "Balance", "Qty", "Due Amount", "Due Year", "Due Month", "Due Days" };
                                if (rightAlignColumns.Contains(formatted))
                                    headerCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                else
                                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;

                                headerCell.Padding = 4;
                                headerCell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                                headerCell.BorderColor = lightGrayBorder;
                                headerCell.BorderWidthTop = 0.8f;
                                headerCell.BorderWidthBottom = 0.8f;
                                headerCell.BackgroundColor = BaseColor.WHITE;

                                table.AddCell(headerCell);
                            }
                        }
                        else
                        {
                            Font subGroupFont = new Font(bfBold, 7f, Font.NORMAL, BaseColor.BLACK);
                            PdfPCell subGroupCell = new PdfPCell(new Phrase($"{groupColumn} : {currentGroupValue}", subGroupFont));
                            subGroupCell.Colspan = visibleColumns.Count;
                            subGroupCell.PaddingTop = 4;
                            subGroupCell.PaddingBottom = 4;
                            subGroupCell.HorizontalAlignment = Element.ALIGN_LEFT;
                            subGroupCell.BackgroundColor = BaseColor.WHITE;
                            subGroupCell.Border = Rectangle.NO_BORDER;

                            table.AddCell(subGroupCell);
                        }
                    }
                }

                // Add data row cells
                for (int colIndex = 0; colIndex < visibleColumns.Count; colIndex++)
                {
                    var col = visibleColumns[colIndex];
                    string val = row[col.ColumnName]?.ToString() ?? "";
                    PdfPCell cell;
                    bool isTransactionColumn = col.ColumnName == "Transaction #";

                    if (IsNumericColumn(col.ColumnName) || Regex.IsMatch(col.ColumnName, monthPattern, RegexOptions.IgnoreCase))
                    {
                        decimal num = 0;
                        if (IsNumericValue(val))
                            num = Convert.ToDecimal(val.Replace(",", ""));

                        // Add to totals for all group levels
                        for (int level = 0; level < groupColumns.Count; level++)
                        {
                            if (groupLevelTotals[level].ContainsKey(col.ColumnName))
                                groupLevelTotals[level][col.ColumnName] += num;
                        }
                        grandTotals[col.ColumnName] += num;

                        // Format negative values with round brackets and red color
                        string displayValue;
                        BaseColor textColor;

                        if (string.IsNullOrWhiteSpace(val) || val == "0" || num == 0)
                        {
                            displayValue = "";
                            textColor = isTransactionColumn ? themeColor : BaseColor.BLACK;
                        }
                        else if (num < 0)
                        {
                            // Negative value: show in round brackets and red color
                            displayValue = $"({string.Format("{0:N0}", Math.Abs(num))})";
                            textColor = BaseColor.RED;
                        }
                        else
                        {
                            // Positive value: normal format
                            displayValue = string.Format("{0:N0}", num);
                            textColor = isTransactionColumn ? themeColor : BaseColor.BLACK;
                        }

                        cell = new PdfPCell(new Phrase(displayValue,
                            GetFont(7, Font.NORMAL, textColor)))
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT
                        };
                    }
                    else
                    {
                        string newVal = val == "1900-01-01" ? "" : val;
                        if (col.ColumnName.Contains("Date") && DateTime.TryParse(newVal, out DateTime parsedDate))
                            newVal = parsedDate.ToString("dd-MM-yy");

                        cell = new PdfPCell(new Phrase(newVal,
                            GetFont(7, Font.NORMAL, isTransactionColumn ? themeColor : BaseColor.BLACK)))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                    }

                    // Remove left border from first column and right border from last column
                    if (colIndex == 0) cell.Border &= ~Rectangle.LEFT_BORDER;
                    if (colIndex == visibleColumns.Count - 1) cell.Border &= ~Rectangle.RIGHT_BORDER;

                    cell.BorderColor = lightGrayBorder;
                    cell.Padding = 2;
                    cell.BackgroundColor = BaseColor.WHITE;

                    table.AddCell(cell);
                }
            }

            for (int level = groupColumns.Count - 1; level >= 0; level--)
            {
                if (!string.IsNullOrEmpty(currentGroupValues[level]))
                {
                    AddTotals(table, visibleColumns, groupLevelTotals[level], GetFont, lastDate, lastAmount);
                }
            }

            bool hasGrandTotals = grandTotals.Values.Any(v => v != 0);
            if (hasGrandTotals)
            {
                AddTotals(table, visibleColumns, grandTotals, GetFont, lastDate, lastAmount);
            }

            document.Add(table);
        }

        void FillBlankNestedGroupValues(DataTable dt, List<string> groupColumns)
        {
            if (dt == null || dt.Rows.Count == 0 || groupColumns == null || groupColumns.Count == 0)
                return;

            foreach (var col in groupColumns)
            {
                if (!string.IsNullOrEmpty(col) && !dt.Columns.Contains(col))
                    dt.Columns.Add(col, typeof(string));
            }

            int n = dt.Rows.Count;
            int gCount = groupColumns.Count;
            string[][] values = new string[n][];

            for (int i = 0; i < n; i++)
            {
                values[i] = new string[gCount];
                for (int g = 0; g < gCount; g++)
                    values[i][g] = GetGroupCellValue(dt.Rows[i], groupColumns[g]);
            }

            // Outer-to-inner: an empty child key (e.g. Model on the first row of a Client)
            // inherits the nearest non-empty value that still shares the same parents.
            // That stops the first detail row from rendering under the parent header.
            for (int g = 0; g < gCount; g++)
            {
                for (int i = n - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrEmpty(values[i][g]))
                        continue;

                    // Copy only from a following row. The PDF is shifted by one: the group
                    // key arrives on the next detail row, so the current row must inherit it.
                    string found = null;
                    for (int j = i + 1; j < n; j++)
                    {
                        bool sameParents = true;
                        for (int a = 0; a < g; a++)
                        {
                            if (!string.Equals(values[j][a], values[i][a], StringComparison.Ordinal))
                            {
                                sameParents = false;
                                break;
                            }
                        }
                        if (!sameParents)
                            break;
                        if (!string.IsNullOrEmpty(values[j][g]))
                        {
                            found = values[j][g];
                            break;
                        }
                    }

                    if (found != null)
                    {
                        values[i][g] = found;
                        dt.Rows[i][groupColumns[g]] = found;
                    }
                }
            }
        }

        DataTable BuildGridDataTable(List<Dictionary<string, object>> rows, List<string> groupColumns)
        {
            DataTable dt = new DataTable();
            if (rows == null || rows.Count == 0)
                return dt;

            foreach (var row in rows)
            {
                if (row == null) continue;
                foreach (var key in row.Keys)
                {
                    if (!string.IsNullOrEmpty(key) && !dt.Columns.Contains(key))
                        dt.Columns.Add(key, typeof(string));
                }
            }

            if (groupColumns != null)
            {
                foreach (var g in groupColumns)
                {
                    if (!string.IsNullOrEmpty(g) && !dt.Columns.Contains(g))
                        dt.Columns.Add(g, typeof(string));
                }
            }

            foreach (var row in rows)
            {
                DataRow dr = dt.NewRow();
                if (row != null)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (row.TryGetValue(col.ColumnName, out var val))
                        {
                            dr[col.ColumnName] = ConvertGridCellToString(val);
                            continue;
                        }

                        string matchKey = row.Keys.FirstOrDefault(k =>
                            string.Equals(k, col.ColumnName, StringComparison.OrdinalIgnoreCase));
                        dr[col.ColumnName] = matchKey != null
                            ? ConvertGridCellToString(row[matchKey])
                            : "";
                    }
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        string GetGroupCellValue(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || string.IsNullOrEmpty(columnName) || !row.Table.Columns.Contains(columnName))
                return "";

            object v = row[columnName];
            if (v == null || v == DBNull.Value)
                return "";

            return Convert.ToString(v) ?? "";
        }

        string ConvertGridCellToString(object val)
        {
            if (val == null || val == DBNull.Value)
                return "";

            if (val is JValue jv)
            {
                if (jv.Type == JTokenType.Null || jv.Type == JTokenType.Undefined)
                    return "";
                val = jv.Value;
                if (val == null)
                    return "";
            }

            if (val is DateTime dateVal)
                return dateVal.ToString("yyyy-MM-dd");

            return Convert.ToString(val) ?? "";
        }

        void SortDataTableByGroups(ref DataTable dt, List<string> groupColumns)
        {
            if (dt == null || dt.Rows.Count == 0 || groupColumns == null || groupColumns.Count == 0)
                return;

            foreach (var col in groupColumns)
            {
                if (!dt.Columns.Contains(col))
                    dt.Columns.Add(col, typeof(string));
            }

            // Keep case-sensitive group keys contiguous while preserving first-appearance order
            // (matches DevExtreme grid grouping; DataView sort is case-insensitive by default and
            // can interleave hol-2218 / HOL-2218 rows from raw dataSource order).
            var rows = dt.AsEnumerable().ToList();
            var groupOrder = new Dictionary<string, int>(StringComparer.Ordinal);
            int nextOrder = 0;

            string MakeKey(DataRow r) =>
                string.Join("\u001f", groupColumns.Select(c => GetGroupCellValue(r, c)));

            foreach (var r in rows)
            {
                string key = MakeKey(r);
                if (!groupOrder.ContainsKey(key))
                    groupOrder[key] = nextOrder++;
            }

            var sortedRows = rows
                .Select((r, idx) => new { Row = r, Index = idx })
                .OrderBy(x => groupOrder[MakeKey(x.Row)])
                .ThenBy(x => x.Index)
                .Select(x => x.Row);

            dt = sortedRows.Any() ? sortedRows.CopyToDataTable() : dt.Clone();
        }
        private System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            System.Drawing.Imaging.ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            foreach (System.Drawing.Imaging.ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


    }

    //image work 
 
    public class GridReport
    {
        public DataTable Data { get; set; }
        public string ReportName { get; set; }
        public List<string> GroupColumnsCap { get; set; } = new List<string>();
        public List<DataColumn> VisibleColumns { get; set; } = new List<DataColumn>();
    }

    public class PdfPageHeader : PdfPageEventHelper
    {
        public string CompanyName { get; set; }
        public string ReportName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string PrintedDate { get; set; }
        public string PrintedTime { get; set; }
        public Font DefaultFont { get; set; }

        PdfTemplate totalPages;

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            totalPages = writer.DirectContent.CreateTemplate(50, 50);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            BaseColor themeColor = new BaseColor(0x05, 0x5A, 0x87);

            Font GetFont(float size, int style, BaseColor color)
            {
                return new Font(DefaultFont.BaseFont, size, style, color);
            }

            PdfPTable header = new PdfPTable(3);
            header.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            header.SetWidths(new float[] { 33, 33, 33 });

            PdfPCell companyCell = new PdfPCell(new Phrase(CompanyName, GetFont(13, Font.BOLD, themeColor)));
            companyCell.Colspan = 3;
            companyCell.Border = Rectangle.NO_BORDER;
            companyCell.HorizontalAlignment = Element.ALIGN_LEFT;
            header.AddCell(companyCell);

            PdfPCell reportCell = new PdfPCell(new Phrase(ReportName, GetFont(9, Font.BOLD, themeColor)));
            reportCell.Colspan = 3;
            reportCell.Border = Rectangle.NO_BORDER;
            reportCell.HorizontalAlignment = Element.ALIGN_LEFT;
            header.AddCell(reportCell);

            PdfPCell spacer = new PdfPCell(new Phrase(""));
            spacer.Colspan = 3;
            spacer.Border = Rectangle.NO_BORDER;
            spacer.FixedHeight = 3f;
            header.AddCell(spacer);

            PdfPCell bg1 = new PdfPCell(new Phrase("From : " + FromDate, GetFont(8, Font.NORMAL, BaseColor.WHITE)));
            bg1.BackgroundColor = themeColor;
            bg1.Border = Rectangle.NO_BORDER;
            header.AddCell(bg1);

            PdfPCell bg2 = new PdfPCell(new Phrase("To : " + ToDate, GetFont(8, Font.NORMAL, BaseColor.WHITE)));
            bg2.BackgroundColor = themeColor;
            bg2.Border = Rectangle.NO_BORDER;
            header.AddCell(bg2);

            PdfPCell bg3 = new PdfPCell(new Phrase("", GetFont(8, Font.NORMAL, BaseColor.WHITE)));
            bg3.BackgroundColor = themeColor;
            bg3.Border = Rectangle.NO_BORDER;
            header.AddCell(bg3);

            // Empty line for space before grey line
            PdfPCell spacer2 = new PdfPCell(new Phrase(""));
            spacer2.Colspan = 3;
            spacer2.BackgroundColor = themeColor;
            spacer2.Border = Rectangle.NO_BORDER;
            spacer2.FixedHeight = 3f;
            header.AddCell(spacer2);

            // Line between rows
            PdfPCell lineCell = new PdfPCell(new Phrase(""));
            lineCell.Colspan = 3;
            lineCell.Border = Rectangle.BOTTOM_BORDER;
            lineCell.BorderColorBottom = new BaseColor(200, 200, 200); // light gray
            lineCell.BorderWidthBottom = 0.5f;
            lineCell.PaddingBottom = 2; // Reduced padding to minimize space
            header.AddCell(lineCell);

            PdfPCell pd1 = new PdfPCell(new Phrase("Printed Date : " + PrintedDate, GetFont(8, Font.NORMAL, BaseColor.WHITE))) { BackgroundColor = themeColor, Border = Rectangle.NO_BORDER };
            PdfPCell pd2 = new PdfPCell(new Phrase("Time : " + PrintedTime, GetFont(8, Font.NORMAL, BaseColor.WHITE))) { BackgroundColor = themeColor, Border = Rectangle.NO_BORDER };
            PdfPCell pageCell = new PdfPCell()
            {
                BackgroundColor = themeColor,
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                CellEvent = new PageNumberCellEvent(
                    totalPages,
                    GetFont(8, Font.NORMAL, BaseColor.WHITE),
                    writer.PageNumber   // 👈 CURRENT PAGE injected here
                )
            };
            header.AddCell(pd1);
            header.AddCell(pd2);
            header.AddCell(pageCell);

            header.AddCell(spacer);

            // Position header closer to top (reduced from -10 to -5)
            header.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - 5, writer.DirectContent);
        }
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            totalPages.BeginText();
            totalPages.SetFontAndSize(DefaultFont.BaseFont, 8);
            totalPages.SetColorFill(BaseColor.WHITE);
            totalPages.SetTextMatrix(0, 0);

            totalPages.ShowText((writer.PageNumber).ToString());

            totalPages.EndText();
        }


    }

    public class PageNumberCellEvent : IPdfPCellEvent
    {
        private PdfTemplate totalPages;
        private Font font;
        private int currentPage;

        public PageNumberCellEvent(PdfTemplate totalPages, Font font, int currentPage)
        {
            this.totalPages = totalPages;
            this.font = font;
            this.currentPage = currentPage;
        }

        public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
        {
            PdfContentByte cb = canvases[PdfPTable.TEXTCANVAS];

            float baseY = position.Bottom + 2;

            float rightEdge = position.Right - 8;

            cb.AddTemplate(totalPages, rightEdge - 20, baseY);

            ColumnText.ShowTextAligned(
                cb,
                Element.ALIGN_RIGHT,
                new Phrase("of ", font),
                rightEdge - 24,
                baseY,
                0
            );

            ColumnText.ShowTextAligned(
                cb,
                Element.ALIGN_RIGHT,
                new Phrase($"Page : {currentPage} ", font),
                rightEdge - 34,
                baseY,
                0
            );
        }
    }

    public class GridRequest
    {
        //public object GridData { get; set; }
        public JArray GridData { get; set; } = new JArray();

        public string GridTitle { get; set; }
        public List<string> GroupColumnsCap { get; set; } = new();
    }

    public class PDFRequest
    {
        public List<GridRequest> Grids { get; set; } = new();
        public bool IsLandscape { get; set; }
        public List<string>? GroupColumns1 { get; set; }
        public List<string>? GroupColumns2 { get; set; }
        public Dictionary<string, decimal>? Totals1 { get; set; }
        public Dictionary<string, decimal>? Totals2 { get; set; }
        public string? CompanyName { get; set; }
        public string? ReportName { get; set; }
        public DateTime? lastDate { get; set; }
        public decimal? lastAmount { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Size { get; set; }
    }
}
