using Empire_ERP.Core.Entities;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using ZXing.OneD;
using ZXing.QrCode;
using Cell = iText.Layout.Element.Cell;
using Table = iText.Layout.Element.Table;

namespace Empire_ERP.Core.Services
{
    public class PdfService
    {
        public static void GeneratePayWiseSlips(SlipViewModel slip, string baseFilePath, string webRootPath, int PRINT_COPIES)
        {
            if (slip == null || slip.Items == null || slip.Items.Count == 0)
                return;

            GeneratePaySlipPdf(slip, baseFilePath, webRootPath, PRINT_COPIES);
        }
        public static void GenerateKotWiseSlips(SlipViewModel slip, string baseFilePath, string webRootPath, int PRINT_COPIES)
        {
            if (slip == null || slip.Items == null || slip.Items.Count == 0)
                return;

            var kotGroups = slip.Items
                .Where(x => x.JobName == "KOT"
                         && !string.IsNullOrWhiteSpace(x.KotPrinter))
                .GroupBy(x => x.KotPrinter)
                .ToList();

            foreach (var group in kotGroups)
            {
                string kotPrinter = group.Key;

                var kotSlip = new SlipViewModel
                {
                    Clogo = slip.Clogo,
                    ReferenceNumber = slip.ReferenceNumber,
                    Date = slip.Date,
                    Time = slip.Time,
                    Waitername = slip.Waitername,
                    TableNum = slip.TableNum,
                    SalesmanName = slip.SalesmanName,
                    QRCode = slip.QRCode,

                    KotPrinter = kotPrinter,
                    Items = group.ToList()
                };

                string folder = System.IO.Path.GetDirectoryName(baseFilePath);
                string fileName = $"KOT_{kotPrinter}_{DateTime.Now:yyyyMMddHHmmssfff}.pdf";
                string filePath = System.IO.Path.Combine(folder, fileName);

                GenerateSlipPdf(kotSlip, filePath, webRootPath, PRINT_COPIES);
            }
        }
        public static void GenerateStickerWiseSlips(SlipViewModel slip, string baseFilePath, string webRootPath, int PRINT_COPIES)
        {
            if (slip == null || slip.Items == null || slip.Items.Count == 0)
                return;

            // Group STICKER items by StickerPrinter
            var stickerGroups = slip.Items
                .Where(x => x.JobName == "STICKER" && !string.IsNullOrWhiteSpace(x.StickerPrinter))
                .GroupBy(x => x.StickerPrinter)
                .ToList();

            foreach (var group in stickerGroups)
            {
                string stickerPrinter = group.Key;

                var stickerSlip = new SlipViewModel
                {
                    StickerPrinter = stickerPrinter,
                    StickerLogo = slip.StickerLogo,
                    Date = slip.Date,
                    Time = slip.Time,
                    CustomerName = slip.CustomerName,
                    FName = slip.FName,
                    Type = slip.Type,
                    Items = group.ToList()
                };

                GenerateStickers(stickerSlip, baseFilePath, webRootPath,PRINT_COPIES);
            }
        }
        public static void GeneratePaySlipPdf(SlipViewModel slip, string filePath, string webRootPath, int PRINT_COPIES )
        {
            if (slip == null)
                throw new ArgumentNullException(nameof(slip));

            try
            {
                // Ensure folder exists
                var folder = System.IO.Path.GetDirectoryName(filePath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                float thermalWidth = 204.9f; // 80mm in points
                float thermalHeight = 1200f; // Dynamic height, will adjust

                // Create PDF writer and document
                using var writer = new PdfWriter(filePath);
                using var pdf = new PdfDocument(writer);
                var pageSize = new PageSize(thermalWidth, thermalHeight);
                var doc = new iText.Layout.Document(pdf, pageSize);
                // Minimal margins for thermal printer
                doc.SetMargins(5, 5, 5, 5);

                var normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // -----------------------------
                // HEADER SECTION (Logo + Store Info)
                // -----------------------------

                // Add logo if exists
                if (!string.IsNullOrEmpty(slip.Clogo))
                {
                    string logoRelativePath = slip.Clogo.TrimStart('/');
                    string logoPath = System.IO.Path.Combine(webRootPath, logoRelativePath);
                    if (File.Exists(logoPath))
                    {
                        var imgData = ImageDataFactory.Create(logoPath);
                        var logo = new iText.Layout.Element.Image(imgData);
                        logo.ScaleToFit(150, 75);
                        logo.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                        doc.Add(logo);
                        doc.Add(new Paragraph(" ").SetMargin(0).SetPadding(0)); // Spacing
                    }
                }

                // Business name (centered)
                if (!string.IsNullOrWhiteSpace(slip.BName))
                {
                    doc.Add(new Paragraph(slip.BName)
                        .SetFont(normalFont)
                        .SetFontSize(11)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMargin(0));
                }

                // Business address (centered)
                if (!string.IsNullOrWhiteSpace(slip.BAddress))
                {
                    doc.Add(new Paragraph(slip.BAddress)
                        .SetFont(normalFont)
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMargin(0));
                }

                // BNTN (if exists and not empty/0)
                if (!string.IsNullOrWhiteSpace(slip.BNTN) && slip.BNTN != "0")
                {
                    doc.Add(new Paragraph($"SNTN: {slip.BNTN}")
                        .SetFont(normalFont)
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMargin(0));
                }

                // CPC_CODE (if exists and not empty/0)
                if (!string.IsNullOrWhiteSpace(slip.CPC_CODE) && slip.CPC_CODE != "0")
                {
                    doc.Add(new Paragraph($"CPC: {slip.CPC_CODE}")
                        .SetFont(normalFont)
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMargin(0));
                }

                // Business phone (centered)
                if (!string.IsNullOrWhiteSpace(slip.BPhone))
                {
                    doc.Add(new Paragraph(slip.BPhone)
                        .SetFont(normalFont)
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(4)
                        .SetMarginTop(0));
                }

                // -----------------------------
                // QR CODE (Reference Number)
                // -----------------------------
                if (!string.IsNullOrEmpty(slip.ReferenceNumber) && slip.QRCode == 1)
                {
                    byte[] qrCodeBytes = GenerateQRCodeBytes(slip.ReferenceNumber);
                    if (qrCodeBytes != null && qrCodeBytes.Length > 0)
                    {
                        var qrCodeImage = ImageDataFactory.Create(qrCodeBytes);
                        var qrImg = new iText.Layout.Element.Image(qrCodeImage);
                        qrImg.ScaleToFit(70, 70);
                        qrImg.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                        doc.Add(qrImg);
                    }
                }

                // -----------------------------
                // BILL INFO SECTION (Left-aligned label and value)
                // -----------------------------
                var infoTable = new Table(UnitValue.CreatePercentArray(new float[] { 40, 60 }))
                    .UseAllAvailableWidth()
                    .SetFontSize(9)
                    .SetMarginTop(4)
                    .SetMarginBottom(4);

                void AddInfoRow(string label, string value)
                {
                    if (string.IsNullOrWhiteSpace(value))
                        return;

                    infoTable.AddCell(new Cell()
                        .Add(new Paragraph(label)
                            .SetFont(boldFont)
                            .SetFontSize(9))
                        .SetBorder(Border.NO_BORDER)
                        .SetPadding(1)
                        .SetTextAlignment(TextAlignment.LEFT));

                    infoTable.AddCell(new Cell()
                        .Add(new Paragraph(value)
                            .SetFont(normalFont)
                            .SetFontSize(9))
                        .SetBorder(Border.NO_BORDER)
                        .SetPadding(1)
                        .SetTextAlignment(TextAlignment.LEFT));
                }

                AddInfoRow("Bill No:", slip.ReferenceNumber ?? "");
                AddInfoRow("Date & Time:", $"{slip.Date ?? ""} {slip.Time ?? ""}".Trim());

                if (!string.IsNullOrWhiteSpace(slip.Type))
                    AddInfoRow("Type:", slip.Type);

                if (!string.IsNullOrWhiteSpace(slip.SalesmanName))
                    AddInfoRow("Cashier:", slip.SalesmanName);

                if (!string.IsNullOrWhiteSpace(slip.CustomerName))
                    AddInfoRow("Name:", slip.CustomerName);

                if (!string.IsNullOrWhiteSpace(slip.ContactNumber))
                    AddInfoRow("Contact #:", slip.ContactNumber);

                if (!string.IsNullOrWhiteSpace(slip.Waitername))
                    AddInfoRow("Waiter:", slip.Waitername);

                if (!string.IsNullOrWhiteSpace(slip.TableNum))
                    AddInfoRow("Table:", slip.TableNum);

                if (!string.IsNullOrWhiteSpace(slip.Remarks))
                    AddInfoRow("Remarks:", slip.Remarks);

                doc.Add(infoTable);

                // -----------------------------
                // SALES RECEIPT TITLE (Bordered Box)
                // -----------------------------
                var receiptHeaderDiv = new Div()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(2)
                    .SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
                receiptHeaderDiv.Add(new Paragraph("SALES RECEIPT")
                    .SetFont(boldFont)
                    .SetFontSize(13)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMargin(0));
                doc.Add(receiptHeaderDiv);
                doc.Add(new Paragraph(" ").SetMargin(2).SetPadding(0)); // Spacing

                // -----------------------------
                // ITEMS TABLE (with conditional Disc % column)
                // -----------------------------
                bool showDiscount = slip.itemDiscount != null && slip.itemDiscount != 0;
                float[] columnWidths = showDiscount 
                    ? new float[] { 30, 12, 18, 12, 28 }  // Description, Qty, Price, Disc %, Amount
                    : new float[] { 40, 15, 20, 25 };     // Description, Qty, Price, Amount

                Table itemsTable = new Table(UnitValue.CreatePercentArray(columnWidths))
                .UseAllAvailableWidth()
                .SetFontSize(9)
                    .SetMarginTop(4)
                    .SetMarginBottom(4);

                void AddHeaderCell(string text)
                {
                    itemsTable.AddHeaderCell(new Cell()
                        .Add(new Paragraph(text)
                            .SetFont(boldFont)
                            .SetFontSize(10))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPadding(2)
                        .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f)));
                }

                AddHeaderCell("Description");
                AddHeaderCell("Qty");
                AddHeaderCell("Price");
                if (showDiscount)
                    AddHeaderCell("Disc %");
                AddHeaderCell("Amount");

                if (slip.Items != null && slip.Items.Any())
                {
                    foreach (var item in slip.Items)
                    {
                        // Check if this is "# Of Items" row
                        bool isOfItemsRow = !string.IsNullOrWhiteSpace(item.Description) && 
                                          (item.Description.Contains("# Of Items") || item.Description.Contains("# Of Item"));

                        // Description row (spans all columns, left-aligned)
                        int colspan = showDiscount ? 5 : 4;
                        var descCell = new Cell(1, colspan)
                            .Add(new Paragraph(item.Description ?? "")
                                .SetFont(boldFont)
                                .SetFontSize(9)
                                .SetMargin(0))
                            .SetTextAlignment(TextAlignment.LEFT)
                            .SetPadding(2);

                        if (isOfItemsRow)
                        {
                            descCell.SetBorderTop(Border.NO_BORDER)
                                    .SetBorderLeft(Border.NO_BORDER)
                                    .SetBorderRight(Border.NO_BORDER)
                                    .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f));
                        }
                        else
                        {
                            descCell.SetBorder(Border.NO_BORDER);
                        }
                        itemsTable.AddCell(descCell);

                        // Data row: BARCODE (in Description column), Qty, Price, Disc % (if shown), Amount
                        var barcodeCell = new Cell()
                            .Add(new Paragraph(item.BARCODE ?? "")
                                .SetFont(normalFont)
                                .SetFontSize(9)
                                .SetMargin(0))
                            .SetTextAlignment(TextAlignment.LEFT)
                            .SetPadding(2);

                        if (isOfItemsRow)
                        {
                            barcodeCell.SetBorderTop(Border.NO_BORDER)
                                      .SetBorderLeft(Border.NO_BORDER)
                                      .SetBorderRight(Border.NO_BORDER)
                                      .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f));
                        }
                        else
                        {
                            barcodeCell.SetBorder(Border.NO_BORDER);
                        }
                        itemsTable.AddCell(barcodeCell);

                        var qtyCell = new Cell()
                            .Add(new Paragraph((item.Quantity ?? 0).ToString())
                                .SetFont(normalFont)
                                .SetFontSize(9)
                                .SetMargin(0))
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetPadding(2);

                        if (isOfItemsRow)
                        {
                            qtyCell.SetBorderTop(Border.NO_BORDER)
                                   .SetBorderLeft(Border.NO_BORDER)
                                   .SetBorderRight(Border.NO_BORDER)
                                   .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f));
                        }
                        else
                        {
                            qtyCell.SetBorder(Border.NO_BORDER);
                        }
                        itemsTable.AddCell(qtyCell);

                        string priceText = (item.Price ?? 0).ToString("N0");
                        var priceCell = new Cell()
                            .Add(new Paragraph(priceText)
                                .SetFont(normalFont)
                                .SetFontSize(9)
                                .SetMargin(0))
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetPadding(2);

                        if (isOfItemsRow)
                        {
                            priceCell.SetBorderTop(Border.NO_BORDER)
                                     .SetBorderLeft(Border.NO_BORDER)
                                     .SetBorderRight(Border.NO_BORDER)
                                     .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f));
                        }
                        else
                        {
                            priceCell.SetBorder(Border.NO_BORDER);
                        }
                        itemsTable.AddCell(priceCell);

                        if (showDiscount)
                        {
                            string discText = (item.Discount ?? 0).ToString("N0");
                            if (discText == "0")
                                discText = "";
                            var discCell = new Cell()
                                .Add(new Paragraph(discText)
                                    .SetFont(normalFont)
                                    .SetFontSize(9)
                                    .SetMargin(0))
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetPadding(2);

                            if (isOfItemsRow)
                            {
                                discCell.SetBorderTop(Border.NO_BORDER)
                                        .SetBorderLeft(Border.NO_BORDER)
                                        .SetBorderRight(Border.NO_BORDER)
                                        .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f));
                            }
                            else
                            {
                                discCell.SetBorder(Border.NO_BORDER);
                            }
                            itemsTable.AddCell(discCell);
                        }

                        string amountText = (item.Amount ?? 0).ToString("N0");
                        var amountCell = new Cell()
                            .Add(new Paragraph(amountText)
                                .SetFont(normalFont)
                                .SetFontSize(9)
                                .SetMargin(0))
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetPadding(2);

                        if (isOfItemsRow)
                        {
                            amountCell.SetBorderTop(Border.NO_BORDER)
                                      .SetBorderLeft(Border.NO_BORDER)
                                      .SetBorderRight(Border.NO_BORDER)
                                      .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f));
                        }
                        else
                        {
                            amountCell.SetBorder(Border.NO_BORDER);
                        }
                        itemsTable.AddCell(amountCell);
                    }
                }

                doc.Add(itemsTable);

                // -----------------------------
                // SUMMARY SECTION
                // -----------------------------
                doc.Add(new LineSeparator(new SolidLine(0.5f)));

                var summaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 60, 40 }))
                    .UseAllAvailableWidth()
                    .SetFontSize(9)
                    .SetMarginTop(2)
                    .SetMarginBottom(2);

                void AddSummaryRow(string label, string value, bool bold = false)
                {
                    summaryTable.AddCell(new Cell()
                        .Add(new Paragraph(label)
                            .SetFont(bold ? boldFont : normalFont)
                            .SetFontSize(9))
                        .SetBorder(Border.NO_BORDER)
                        .SetPadding(1));

                    summaryTable.AddCell(new Cell()
                        .Add(new Paragraph(value)
                            .SetFont(bold ? boldFont : normalFont)
                            .SetFontSize(9))
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetBorder(Border.NO_BORDER)
                        .SetPadding(1));
                }

                // BILL DISC
                if (slip.RoundingDiscount != null && slip.RoundingDiscount != 0)
                {
                    string discLabel = $"BILL DISC :";
                    string discValue = $"{slip.DiscountPercentage ?? 0} %";
                    string discAmount = $"({slip.RoundingDiscount})";
                    AddSummaryRow(discLabel, $"{discValue} {discAmount}");
                }

                // SERVICE CHARGES
                if (slip.SER_CHARGES != null && slip.SER_CHARGES != 0)
                {
                    AddSummaryRow("SERVICE CHARGES :", slip.SER_CHARGES.Value.ToString("N0"));
                }

                // DELIVERY CHARGES
                if (slip.Del_Charges != null && slip.Del_Charges != 0)
                {
                    AddSummaryRow("DELIVERY :", slip.Del_Charges.Value.ToString("N0"));
                }

                // GROSS TOTAL
                if (slip.GrossTotal != null && slip.GrossTotal != 0)
                {
                    AddSummaryRow("GROSS TOTAL :", slip.GrossTotal.Value.ToString("N0"), bold: true);
                    doc.Add(summaryTable);
                    summaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 60, 40 }))
                        .UseAllAvailableWidth()
                        .SetFontSize(9)
                        .SetMarginTop(2)
                        .SetMarginBottom(2);
                }

                // SST TAX (based on PAYTYPE) - Format: "SST :15 %" on left, "143" on right
                if (slip.TAX_CASH != 0 || slip.BANK_TAX != 0 || slip.PARTY_TAX != 0)
                {
                    string taxLabel = "";
                    string taxAmount = "";

                    if (slip.PAYTYPE == "Split")
                    {
                        taxLabel = $"SST :{slip.TAX_CASH ?? 0} %";
                        taxAmount = (slip.CASHTAX_AMT ?? 0).ToString("N0");
                    }
                    else if (slip.PAYTYPE == "Bank")
                    {
                        taxLabel = $"SST :{slip.BANK_TAX ?? 0} %";
                        taxAmount = (slip.BANKTAX_AMT ?? 0).ToString("N0");
                    }
                    else if (slip.PAYTYPE == "Party")
                    {
                        taxLabel = $"SST :{slip.PARTY_TAX ?? 0} %";
                        taxAmount = (slip.PARTYTAX_AMT ?? 0).ToString("N0");
                    }
                    else if (slip.PAYTYPE == "Cash" || slip.PAYTYPE == "Advance")
                    {
                        taxLabel = $"SST :{slip.TAX_CASH ?? 0} %";
                        taxAmount = (slip.CASHTAX_AMT ?? 0).ToString("N0");
                    }

                    if (!string.IsNullOrEmpty(taxLabel))
                    {
                        AddSummaryRow(taxLabel, taxAmount);
                    }
                }

                // SETTLEMENT
                if (slip.Sett != null && slip.Sett != 0)
                {
                    string settValue = slip.Sett < 0 ? $"({slip.Sett})" : slip.Sett.Value.ToString("N0");
                    AddSummaryRow("SETTLEMENT:", settValue);
                }

                // INVOICE VALUE (in bordered box)
                string invoiceValue = !string.IsNullOrWhiteSpace(slip.TotalAmountFormatted)
                    ? slip.TotalAmountFormatted
                    : (slip.Items?.Sum(x => (decimal)(x.Amount ?? 0)) ?? 0m).ToString("N0");

                doc.Add(summaryTable);

                var invoiceValueDiv = new Div()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(3)
                    .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                    .SetMarginTop(5);

                var invoiceTable = new Table(UnitValue.CreatePercentArray(new float[] { 60, 40 }))
                    .UseAllAvailableWidth()
                    .SetFontSize(10);

                invoiceTable.AddCell(new Cell()
                    .Add(new Paragraph("INVOICE VALUE:")
                        .SetFont(boldFont)
                        .SetFontSize(11))
                    .SetBorder(Border.NO_BORDER)
                    .SetPadding(1)
                    .SetTextAlignment(TextAlignment.LEFT));

                invoiceTable.AddCell(new Cell()
                    .Add(new Paragraph(invoiceValue)
                        .SetFont(boldFont)
                        .SetFontSize(12))
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetBorder(Border.NO_BORDER)
                    .SetPadding(1));

                invoiceValueDiv.Add(invoiceTable);
                doc.Add(invoiceValueDiv);

                // -----------------------------
                // PAYMENTS SECTION (only if billStatus == "P")
                // -----------------------------
                if (slip.billStatus == "P")
                {
                    doc.Add(new Paragraph(" ").SetMargin(4).SetPadding(0)); // Spacing

                    var paymentsHeader = new Div()
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPadding(2)
                        .SetBorder(new SolidBorder(ColorConstants.BLACK, 1));
                    paymentsHeader.Add(new Paragraph("PAYMENTS")
                        .SetFont(boldFont)
                        .SetFontSize(13)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMargin(0));
                    doc.Add(paymentsHeader);
                    doc.Add(new Paragraph(" ").SetMargin(2).SetPadding(0)); // Spacing

                    var paymentsTable = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }))
                        .UseAllAvailableWidth()
                        .SetFontSize(9)
                        .SetMarginTop(2)
                        .SetMarginBottom(2);

                    void AddPaymentRow(string label, string value, bool isBold = false)
                    {
                        paymentsTable.AddCell(new Cell()
                            .Add(new Paragraph(label)
                                .SetFont(isBold ? boldFont : normalFont)
                                .SetFontSize(9))
                            .SetBorder(Border.NO_BORDER)
                            .SetPadding(1)
                            .SetTextAlignment(TextAlignment.LEFT));

                        paymentsTable.AddCell(new Cell()
                            .Add(new Paragraph(value)
                                .SetFont(isBold ? boldFont : normalFont)
                                .SetFontSize(9))
                            .SetTextAlignment(TextAlignment.RIGHT)
                            .SetBorder(Border.NO_BORDER)
                            .SetPadding(1));
                    }

                    if (slip.Complete == 1)
                    {
                        // Complete payment logic
                        if (slip.PAYTYPE == "Cash" || slip.PAYTYPE == "Split")
                        {
                            if (slip.CashFormatted != "0" && !string.IsNullOrWhiteSpace(slip.CashFormatted))
                            {
                                AddPaymentRow("CASH:", slip.CashFormatted, isBold: true);
                            }
                            if (slip.CashBack != null && slip.CashBack != 0)
                            {
                                AddPaymentRow("CASH BACK:", slip.CashBack.Value.ToString("N0"));
                            }
                        }

                        if (slip.PAYTYPE == "Bank" || slip.PAYTYPE == "Split")
                        {
                            if (slip.BankFormatted != "0" && !string.IsNullOrWhiteSpace(slip.BankFormatted))
                            {
                                AddPaymentRow("CARD:", slip.BankFormatted, isBold: true);
                            }
                        }

                        if (slip.PAYTYPE == "Party" || slip.PAYTYPE == "Split")
                        {
                            if (slip.PartyFormatted != "0" && !string.IsNullOrWhiteSpace(slip.PartyFormatted))
                            {
                                string partyLabel = $"PARTY ({(!string.IsNullOrWhiteSpace(slip.billmode) ? slip.billmode : "Credit")}):";
                                AddPaymentRow(partyLabel, slip.PartyFormatted, isBold: true);
                            }
                        }

                        if (slip.PAYTYPE == "Advance")
                        {
                            if (slip.TotalAdvance != null && slip.TotalAdvance != 0)
                            {
                                AddPaymentRow("PREVIOUS ADVANCE:", slip.TotalAdvance.Value.ToString("N0"));
                            }

                            string receivedAmount = "0";
                            if (slip.CashFormatted != "0" && !string.IsNullOrWhiteSpace(slip.CashFormatted))
                                receivedAmount = slip.CashFormatted;
                            else if (slip.Bank != null && slip.Bank != 0)
                                receivedAmount = slip.Bank.Value.ToString("N0");

                            AddPaymentRow("RECEIVED AMOUNT:", receivedAmount, isBold: true);

                            if (slip.CashFormatted != "0" && !string.IsNullOrWhiteSpace(slip.CashFormatted))
                            {
                                AddPaymentRow("CASH:", slip.CashFormatted, isBold: true);
                            }
                            if (slip.Bank != null && slip.Bank != 0)
                            {
                                AddPaymentRow("CARD:", slip.Bank.Value.ToString("N0"), isBold: true);
                            }
                        }

                        if ((slip.CashFormatted == "0" || string.IsNullOrWhiteSpace(slip.CashFormatted)) 
                            && (slip.Bank == null || slip.Bank == 0) 
                            && (slip.Party == null || slip.Party == 0) 
                            && slip.PAYTYPE != "Advance")
                        {
                            AddPaymentRow("CASH:", slip.CashFormatted ?? "0", isBold: true);
                            if (slip.CashBack != null && slip.CashBack != 0)
                            {
                                AddPaymentRow("CASH BACK:", slip.CashBack.Value.ToString("N0"));
                            }
                        }
                    }
                    else
                    {
                        // Incomplete payment (Advance/Balance)
                        if (slip.Advance != null && slip.Advance != 0)
                        {
                            AddPaymentRow("ADVANCE CASH:", slip.Advance.Value.ToString("N0"), isBold: true);
                        }
                        if (slip.ADVBANK != null && slip.ADVBANK != 0)
                        {
                            AddPaymentRow("ADVANCE CARD:", slip.ADVBANK.Value.ToString("N0"), isBold: true);
                        }
                        if (slip.Balance != null && slip.Balance != 0)
                        {
                            AddPaymentRow("REMAINING BALANCE:", slip.Balance.Value.ToString("N0"), isBold: true);
                        }
                    }

                    doc.Add(paymentsTable);
                }

                // -----------------------------
                // SRB INVOICE (if applicable)
                // -----------------------------
                if (slip.SRBSTATUS == "Y" && !string.IsNullOrWhiteSpace(slip.SRBInvoiceId))
                {
                    doc.Add(new Paragraph(" ").SetMargin(4).SetPadding(0));
                    doc.Add(new Paragraph("SRB Invoice #:")
                        .SetFont(boldFont)
                        .SetFontSize(9)
                        .SetTextAlignment(TextAlignment.CENTER));
                    doc.Add(new Paragraph(slip.SRBInvoiceId)
                        .SetFont(normalFont)
                        .SetFontSize(8)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(4));

                    // SRB Logo and QR Code side by side in a table
                    var srbTable = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }))
                        .UseAllAvailableWidth()
                        .SetMarginBottom(4);

                    // Left cell - SRB Logo
                    var logoCell = new Cell()
                        .SetBorder(Border.NO_BORDER)
                        .SetPadding(2)
                        .SetTextAlignment(TextAlignment.LEFT);

                    string srbLogoPath = System.IO.Path.Combine(webRootPath, "Client", "Company", "SRBPOS.png");
                    if (File.Exists(srbLogoPath))
                    {
                        var srbImgData = ImageDataFactory.Create(srbLogoPath);
                        var srbLogo = new iText.Layout.Element.Image(srbImgData);
                        srbLogo.ScaleToFit(100, 50);
                        logoCell.Add(srbLogo);
                    }
                    srbTable.AddCell(logoCell);

                    // Right cell - QR Code
                    var qrCell = new Cell()
                        .SetBorder(Border.NO_BORDER)
                        .SetPadding(2)
                        .SetTextAlignment(TextAlignment.RIGHT);

                    byte[] srbQrBytes = GenerateQRCodeBytes(slip.SRBInvoiceId);
                    if (srbQrBytes != null && srbQrBytes.Length > 0)
                    {
                        var srbQrImage = ImageDataFactory.Create(srbQrBytes);
                        var srbQrImg = new iText.Layout.Element.Image(srbQrImage);
                        srbQrImg.ScaleToFit(85, 85);
                        qrCell.Add(srbQrImg);
                    }
                    srbTable.AddCell(qrCell);

                    doc.Add(srbTable);
                }

                // -----------------------------
                // TERMS & CONDITIONS
                // -----------------------------
                if (!string.IsNullOrWhiteSpace(slip.MENUTERMS))
                {
                    doc.Add(new LineSeparator(new SolidLine(0.5f)).SetMarginTop(8));

                    var termsHeader = new Div()
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetPadding(2)
                        .SetBorder(new SolidBorder(ColorConstants.BLACK, 1))
                        .SetMarginTop(5);
                    termsHeader.Add(new Paragraph("TERMS & CONDITIONS")
                        .SetFont(boldFont)
                        .SetFontSize(13)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMargin(0));
                    doc.Add(termsHeader);

                    // Parse MENUTERMS (replace \n with line breaks)
                    string[] termsLines = slip.MENUTERMS.Replace("\r", "").Split('\n');
                    foreach (var line in termsLines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            doc.Add(new Paragraph(line.Trim())
                                .SetFont(normalFont)
                                .SetFontSize(8)
                                .SetTextAlignment(TextAlignment.LEFT)
                                .SetMarginTop(1)
                                .SetMarginBottom(0));
                        }
                    }
                    doc.Add(new Paragraph(" ").SetMargin(2).SetPadding(0)); // Spacing
                }

                // -----------------------------
                // SOCIAL MEDIA LINKS
                // -----------------------------
                bool hasSocialMedia = !string.IsNullOrWhiteSpace(slip.FBLINK) ||
                                     !string.IsNullOrWhiteSpace(slip.INSTALINK) ||
                                     !string.IsNullOrWhiteSpace(slip.WEBLINK) ||
                                     !string.IsNullOrWhiteSpace(slip.TIKTOKLINK) ||
                                     !string.IsNullOrWhiteSpace(slip.YOUTUBELINK);

                if (hasSocialMedia)
                {
                    var socialTable = new Table(UnitValue.CreatePercentArray(new float[] { 20, 80 }))
                        .UseAllAvailableWidth()
                        .SetFontSize(9)
                        .SetMarginTop(2);

                    void AddSocialRow(string label, string link)
                    {
                        if (string.IsNullOrWhiteSpace(link))
                            return;

                        socialTable.AddCell(new Cell()
                            .Add(new Paragraph(label)
                                .SetFont(boldFont)
                                .SetFontSize(9))
                            .SetBorder(Border.NO_BORDER)
                            .SetPadding(1)
                            .SetTextAlignment(TextAlignment.LEFT));

                        socialTable.AddCell(new Cell()
                            .Add(new Paragraph(link)
                                .SetFont(normalFont)
                                .SetFontSize(9))
                            .SetBorder(Border.NO_BORDER)
                            .SetPadding(1)
                            .SetTextAlignment(TextAlignment.LEFT));
                    }

                    AddSocialRow("Facebook:", slip.FBLINK ?? "");
                    AddSocialRow("Instagram:", slip.INSTALINK ?? "");
                    AddSocialRow("Website:", slip.WEBLINK ?? "");
                    AddSocialRow("TikTok:", slip.TIKTOKLINK ?? "");
                    AddSocialRow("YouTube:", slip.YOUTUBELINK ?? "");

                    doc.Add(socialTable);
                }

                // -----------------------------
                // WIFI QR CODE
                // -----------------------------
                if (!string.IsNullOrWhiteSpace(slip.WIFIPASS))
                {
                    doc.Add(new Paragraph(" ").SetMargin(4).SetPadding(0));
                    doc.Add(new Paragraph("Free Wi-Fi")
                        .SetFont(normalFont)
                        .SetFontSize(7)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMargin(0));

                    // Generate WiFi QR Code
                    string wifiConfig = $"WIFI:S:{Uri.EscapeDataString(slip.WIFINAME ?? "")};T:WPA;P:{Uri.EscapeDataString(slip.WIFIPASS)};H:false;";
                    byte[] wifiQrBytes = GenerateQRCodeBytes(wifiConfig);
                    if (wifiQrBytes != null && wifiQrBytes.Length > 0)
                    {
                        var wifiQrImage = ImageDataFactory.Create(wifiQrBytes);
                        var wifiQrImg = new iText.Layout.Element.Image(wifiQrImage);
                        wifiQrImg.ScaleToFit(80, 80);
                        wifiQrImg.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                        doc.Add(wifiQrImg);
                    }

                    doc.Add(new Paragraph($"Password: {slip.WIFIPASS}")
                        .SetFont(normalFont)
                        .SetFontSize(6)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginTop(2));
                }

                // -----------------------------
                // FOOTER (Black bar with website - angled left edge design)
                // -----------------------------
                doc.Add(new LineSeparator(new SolidLine(0.5f)).SetMarginTop(8));

                // -----------------------------
                // FOOTER (Black bar with website - angled left edge design + optional circle)
                // -----------------------------
                // -----------------------------
                // FOOTER (Black bar with website - angled left edge design + optional circle)
                // -----------------------------
                if (!string.IsNullOrWhiteSpace(slip.FWebsite))
                {
                    var pdfDoc = doc.GetPdfDocument();
                    var page = pdfDoc.GetLastPage();
                    var pageSizes = page.GetPageSize();

                    float pageWidth = pageSizes.GetWidth();

                    // ===== Footer Dimensions =====
                    float footerHeight = 22f;
                    float slantWidth = 18f;

                    var pdfCanvas = new PdfCanvas(page);
                    var canvas = new Canvas(pdfCanvas, pageSizes);

                    // ===== Slanted Black Footer =====
                    pdfCanvas.SaveState();
                    pdfCanvas.SetFillColor(ColorConstants.BLACK);

                    pdfCanvas.MoveTo(0, 0);                         // bottom-left
                    pdfCanvas.LineTo(slantWidth, footerHeight);     // slanted edge
                    pdfCanvas.LineTo(pageWidth, footerHeight);      // top-right
                    pdfCanvas.LineTo(pageWidth, 0);                 // bottom-right
                    //pdfCanvas.ClosePathFill();

                    pdfCanvas.RestoreState();

                    // ===== Footer Text =====
                    var footerText = new Paragraph(slip.FWebsite)
                        .SetFont(boldFont)
                        .SetFontSize(11)
                        .SetFontColor(ColorConstants.WHITE)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        .SetMargin(0);

                    var footerDiv = new Div()
                        .SetHeight(footerHeight)
                        .SetFixedPosition(0, 0, pageWidth)
                        .SetPaddingLeft(10)
                        .Add(footerText);

                    canvas.Add(footerDiv);
                    canvas.Close();
                }




                doc.Close();

                PrintPdfToThermalSticker(filePath, slip.P_PRINTER, $"{webRootPath}\\Client\\SumatraPDF-3.5.2-64\\SumatraPDF.exe",PRINT_COPIES);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error generating PDF slip", ex);
            }
        }
        public static void GenerateSlipPdf(SlipViewModel slip, string filePath, string webRootPath,int PRINT_COPIES)
        {
            if (slip == null)
                throw new ArgumentNullException(nameof(slip));

            try
            {
                // Ensure folder exists
                var folder = System.IO.Path.GetDirectoryName(filePath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                float thermalWidth = 204.9f; // 80mm in points
                float thermalHeight = 800f; // Dynamic height, will adjust

                // Create PDF writer and document
                using var writer = new PdfWriter(filePath);
                using var pdf = new PdfDocument(writer);
                var pageSize = new PageSize(thermalWidth, thermalHeight);
                var doc = new iText.Layout.Document(pdf, pageSize);

                // Minimal margins for thermal printer
                doc.SetMargins(5, 5, 5, 5);

                var normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Add logo if exists
                if (!string.IsNullOrEmpty(slip.Clogo))
                {
                    string logoPath = System.IO.Path.Combine(webRootPath, "Client", "Company", slip.Clogo);

                    if (File.Exists(logoPath))
                    {
                        var imgData = ImageDataFactory.Create(logoPath);
                        var logo = new iText.Layout.Element.Image(imgData);
                        logo.ScaleToFit(140, 70);
                        logo.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                        doc.Add(logo);
                    }
                }

                // Generate and add QR Code
                if (!string.IsNullOrEmpty(slip.ReferenceNumber) && slip.QRCode == 1)
                {
                    byte[] qrCodeBytes = GenerateQRCodeBytes(slip.ReferenceNumber);
                    if (qrCodeBytes != null && qrCodeBytes.Length > 0)
                    {
                        var qrCodeImage = ImageDataFactory.Create(qrCodeBytes);
                        var qrImg = new iText.Layout.Element.Image(qrCodeImage);
                        qrImg.ScaleToFit(80, 80);
                        qrImg.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                        doc.Add(qrImg);
                    }
                }

                // Reference Number - Left aligned
                var refPara = new Paragraph()
                    .Add(new Text("REF #: ").SetFont(boldFont).SetFontSize(10))
                    .Add(new Text(slip.ReferenceNumber ?? "").SetFont(normalFont).SetFontSize(10)).SetMarginBottom(0);
                doc.Add(refPara);

                // Date and Time - Left aligned
                var datePara = new Paragraph()
                    .Add(new Text("DATE: ").SetFont(boldFont).SetFontSize(10))
                    .Add(new Text($"{slip.Date ?? ""} {slip.Time ?? ""}").SetFont(normalFont).SetFontSize(10)).SetMarginTop(0).SetMarginBottom(2);
                doc.Add(datePara);

                // Waiter (if available) - Left aligned
                if (!string.IsNullOrEmpty(slip.Waitername))
                {
                    var waiterPara = new Paragraph()
                        .Add(new Text("Waiter: ").SetFont(boldFont).SetFontSize(10))
                        .Add(new Text(slip.Waitername).SetFont(normalFont).SetFontSize(10)).SetMarginTop(0).SetMarginBottom(2);
                    doc.Add(waiterPara);
                }

                // Table (if available) - Left aligned
                if (!string.IsNullOrEmpty(slip.TableNum))
                {
                    var tablePara = new Paragraph()
                        .Add(new Text("Table: ").SetFont(boldFont).SetFontSize(10))
                        .Add(new Text(slip.TableNum).SetFont(normalFont).SetFontSize(10)).SetMarginTop(0).SetMarginBottom(2);
                    doc.Add(tablePara);
                }
                // KOT RECEIPT Header with border - Centered
                var kotHeaderDiv = new Div()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(0)
                    .SetBorder(new iText.Layout.Borders.SolidBorder(iText.Kernel.Colors.ColorConstants.BLACK, 1));
                kotHeaderDiv.Add(new Paragraph("KOT RECEIPT")
                    .SetFont(boldFont)
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMargin(0));
                doc.Add(kotHeaderDiv);

                // Items Table
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 75, 25 }))
                .UseAllAvailableWidth()
                .SetFontSize(9)
                .SetMarginTop(5)
                .SetMarginBottom(5);


                var descHeader = new Cell()
                .Add(new Paragraph("Description").SetFont(boldFont).SetFontSize(9))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(2)
                .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.7f));
                table.AddHeaderCell(descHeader);

                var qtyHeader = new Cell()
                    .Add(new Paragraph("Qty").SetFont(boldFont).SetFontSize(9))
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetPadding(2)
                    .SetBorder(new SolidBorder(ColorConstants.BLACK, 0.7f));
                table.AddHeaderCell(qtyHeader);

                if (slip.Items != null)
                {
                    for (int i = 0; i < slip.Items.Count; i++)
                    {
                        var item = slip.Items[i];
                        if (item.JobName == "KOT")
                        {
                            bool isLastRow = (i == slip.Items.Count - 1);

                            float borderWidth = isLastRow ? 1.5f : 0.5f;

                            // Description
                            var descCell = new Cell()
                                .Add(new Paragraph(item.Description ?? "")
                                .SetFont(normalFont)
                                .SetFontSize(9)
                                .SetMargin(0))
                                .SetPadding(2)
                                .SetBorderTop(Border.NO_BORDER)
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER)
                                .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, borderWidth));

                            table.AddCell(descCell);

                            // Qty
                            var qtyCell = new Cell()
                                .Add(new Paragraph(item.Quantity?.ToString() ?? "0")
                                .SetFont(normalFont)
                                .SetFontSize(9)
                                .SetMargin(0))
                                .SetTextAlignment(TextAlignment.RIGHT)
                                .SetPadding(2)
                                .SetBorderTop(Border.NO_BORDER)
                                .SetBorderLeft(Border.NO_BORDER)
                                .SetBorderRight(Border.NO_BORDER)
                                .SetBorderBottom(new SolidBorder(ColorConstants.BLACK, borderWidth));

                            table.AddCell(qtyCell);
                        }
                    }
                }
                doc.Add(table);
                // Counter/Salesman - Left aligned
                var counterPara = new Paragraph()
                    .Add(new Text("Counter: ").SetFont(boldFont).SetFontSize(10))
                    .Add(new Text(slip.SalesmanName ?? "").SetFont(normalFont).SetFontSize(10)).SetMarginTop(0).SetMarginBottom(2);
                doc.Add(counterPara);

                doc.Close();

                PrintPdfToThermalSticker(filePath, slip.KotPrinter, $"{webRootPath}\\Client\\SumatraPDF-3.5.2-64\\SumatraPDF.exe",PRINT_COPIES);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error generating PDF slip", ex);
            }
        }
        private static byte[] GenerateQRCodeBytes(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return null;

                text = text.Trim();

                // Define QR code encoding options
                var options = new EncodingOptions
                {
                    Height = 300,
                    Width = 300,
                    Margin = 1
                };

                // Create a barcode writer
                var barcodeWriter = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = options
                };

                // Generate QR code pixel data
                var pixelData = barcodeWriter.Write(text);

                // Convert pixel data to a PNG image
                using (var ms = new MemoryStream())
                {
                    using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height))
                    {
                        for (int y = 0; y < pixelData.Height; y++)
                        {
                            for (int x = 0; x < pixelData.Width; x++)
                            {
                                var color = pixelData.Pixels[(y * pixelData.Width + x) * 4];
                                bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(color, color, color));
                            }
                        }

                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    }

                    return ms.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }
        public static void GenerateStickers(SlipViewModel slip, string filePath, string webRootPath,int PRINT_COPIES)
        {
            List<string> stickerPaths = new List<string>();
            if (slip == null)
                throw new ArgumentNullException(nameof(slip));

            try
            {
                // Ensure folder exists
                var folder = System.IO.Path.GetDirectoryName(filePath);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                // Convert cm to points: 1 cm = 28.35 points
                // 4cm height = 113.4 points, 5cm width = 141.75 points
                float stickerWidth = 127.575f;  // 5cm
                float stickerHeight = 108.4f;  // 4cm

                string fontRegularPath = System.IO.Path.Combine(webRootPath, "fonts", "TCM_____.TTF");
                string fontBoldPath = System.IO.Path.Combine(webRootPath, "fonts", "TCB_____.TTF");


                foreach (var item in slip.Items)
                {
                    if (item.JobName == "STICKER")
                    {
                        if (item.Description == "# Of Items :")
                            break;

                        int totalQty = (int)(item.Quantity ?? 0);
                        string safeDesc = (item.Description ?? "Item").Replace("/", "_").Replace("\\", "_");
                        string stickerPath = System.IO.Path.Combine(
                                                folder,
                                                $"Sticker_{safeDesc}_Qty_{totalQty}.pdf"
                                            );

                        using (PdfWriter writer = new PdfWriter(stickerPath))
                        {
                            PdfDocument pdf = new PdfDocument(writer);

                            PdfFont normalFont = PdfFontFactory.CreateFont(
                                fontRegularPath,
                                PdfEncodings.IDENTITY_H,
                                PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                            );

                            PdfFont boldFont = PdfFontFactory.CreateFont(
                                fontBoldPath,
                                PdfEncodings.IDENTITY_H,
                                PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
                            );

                            using (iText.Layout.Document doc =
                                   new iText.Layout.Document(pdf, new PageSize(stickerWidth, stickerHeight)))
                            {
                                doc.SetMargins(1, 2, 2, 11);

                                // Top section with logo and quantity
                                var topTable = new Table(new float[] { 25, 50, 25 })
                                    .UseAllAvailableWidth()
                                    .SetMarginBottom(2);

                                // Left cell - Empty space for circle (already drawn)
                                var leftCell = new Cell()
                                    .SetBorder(Border.NO_BORDER)
                                    .SetPadding(0)
                                    .SetHeight(16);
                                leftCell.Add(new Paragraph(" ").SetMargin(0).SetPadding(0));

                                // Center cell - Logo (small) + Dine In
                                var centerCell = new Cell()
                                    .SetBorder(Border.NO_BORDER)
                                    .SetPadding(0)
                                    .SetTextAlignment(TextAlignment.CENTER)
                                    .SetVerticalAlignment(VerticalAlignment.MIDDLE);

                                // Add logo if exists (responsive size based on sticker dimensions)
                                float logoMaxWidth = stickerWidth * 0.999f;
                                float logoMaxHeight = stickerHeight * 0.250f;

                                if (!string.IsNullOrEmpty(slip.StickerLogo))
                                {
                                    string logoPath = null;
                                    if (!string.IsNullOrEmpty(slip.StickerLogo))
                                    {
                                        var relativeLogo = slip.StickerLogo.TrimStart('/', '\\');
                                        logoPath = System.IO.Path.Combine(webRootPath, relativeLogo);
                                    }
                                    if (File.Exists(logoPath))
                                    {
                                        var imgData = ImageDataFactory.Create(logoPath);
                                        var logo = new iText.Layout.Element.Image(imgData);
                                        logo.ScaleToFit(logoMaxWidth, logoMaxHeight);
                                        logo.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                                        centerCell.Add(logo);
                                    }
                                }

                                centerCell.Add(new Paragraph(slip.Type ?? "Status")
                                    .SetFont(boldFont)
                                    .SetFontSize(8)
                                    .SetFontColor(ColorConstants.BLACK)
                                    .SetTextAlignment(TextAlignment.CENTER)
                                    .SetMarginTop(0)
                                    .SetMarginBottom(0));


                                topTable.AddCell(leftCell);
                                topTable.AddCell(centerCell);
                                doc.Add(topTable);

                                var itemTable = new Table(1)
                                .UseAllAvailableWidth()
                                .SetMarginBottom(1);

                                // Calculate circle size based on sticker dimensions
                                //float circleSize = Math.Min(stickerWidth, stickerHeight) * 0.10f;

                                //var circleCell = new Cell()
                                //    .SetBorder(Border.NO_BORDER)
                                //    .SetHeight(circleSize)
                                //    .SetPadding(0)
                                //    .SetMarginBottom(2);

                                //circleCell.SetNextRenderer(new CircleTextCellRenderer(circleCell, $"{item.ItemCode}", boldFont, 7));

                                //itemTable.AddCell(circleCell);

                                itemTable.AddCell(new Cell()
                                    .Add(new Paragraph(item.Description ?? "")
                                        .SetFont(boldFont)
                                        .SetFontSize(10)
                                        .SetFontColor(ColorConstants.BLACK))
                                    .SetBorder(Border.NO_BORDER)
                                    .SetPadding(0)
                                    .SetMarginBottom(0).SetMarginTop(0));

                                doc.Add(itemTable);

                                // Date/Time and Reference Code Row
                                var infoRow = new Table(new float[] { 1, 1 })
                                    .UseAllAvailableWidth()
                                    .SetMarginBottom(0).SetMarginTop(0);

                                // Format date/time to match image format (MM/dd/yyyy hh:mm tt)
                                string dateTime = $"{slip.Date ?? ""} {slip.Time ?? ""}";
                                if (!string.IsNullOrEmpty(slip.Date) && DateTime.TryParse(slip.Date, out DateTime parsedDate))
                                {
                                    string timePart = slip.Time ?? "";
                                    DateTime dateTimeObj = parsedDate;

                                    if (!string.IsNullOrEmpty(timePart))
                                    {
                                        if (TimeSpan.TryParse(timePart, out TimeSpan parsedTime))
                                        {
                                            dateTimeObj = parsedDate.Date.Add(parsedTime);
                                        }
                                        else if (DateTime.TryParse($"{slip.Date} {slip.Time}", out DateTime fullDateTime))
                                        {
                                            dateTimeObj = fullDateTime;
                                        }
                                    }

                                    dateTime = dateTimeObj.ToString("MM/dd/yyyy hh:mm tt");
                                }

                                infoRow.AddCell(new Cell()
                                    .Add(new Paragraph(dateTime)
                                        .SetFont(normalFont)
                                        .SetFontSize(9))
                                    .SetFontColor(ColorConstants.BLACK)
                                    .SetBorder(Border.NO_BORDER)
                                    .SetPadding(0).SetMarginTop(0)
                                    .SetMarginBottom(0));

                                //infoRow.AddCell(new Cell()
                                //    .Add(new Paragraph(slip.LocationShortName)
                                //        .SetFont(normalFont)
                                //        .SetFontSize(8)
                                //        .SetFontColor(ColorConstants.BLACK)
                                //        .SetTextAlignment(TextAlignment.RIGHT))
                                //    .SetBorder(Border.NO_BORDER)
                                //    .SetPadding(0)
                                //    .SetTextAlignment(TextAlignment.RIGHT).SetMarginTop(5)
                                //    .SetMarginBottom(0));

                                doc.Add(infoRow);

                                // Customer Name (centered)
                                doc.Add(new Paragraph(slip.CustomerName ?? "Guest")
                                    .SetFont(boldFont)
                                    .SetFontSize(10)
                                    .SetFontColor(ColorConstants.BLACK)
                                    .SetTextAlignment(TextAlignment.LEFT)
                                    .SetMarginTop(-1)
                                    .SetMarginBottom(0));

                                doc.Add(new Paragraph($"Developed by : {slip.FName} ")
                                .SetFont(normalFont)
                                .SetFontSize(8)
                                .SetFontColor(ColorConstants.BLACK)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetFixedPosition(
                                    0,
                                    3,              // bottom se thoda upar
                                    stickerWidth    // full width
                                )
                                .SetMargin(0)
                                .SetPadding(0));

                            }
                        }
                        // Add sticker path to the list
                        stickerPaths.Add(stickerPath);
                    }
                }
                var paths = string.Join(",", stickerPaths);
                PrintPdfToThermalSticker(paths, slip.StickerPrinter, $"{webRootPath}\\Client\\SumatraPDF-3.5.2-64\\SumatraPDF.exe", PRINT_COPIES);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error generating Sticker PDFs", ex);
            }
        }
        public static void PrintPdfToThermalSticker(string pdfPaths, string printerName, string pdfReaderPath,int PRINT_COPIES)
        {
            if (string.IsNullOrWhiteSpace(pdfPaths))
                throw new ArgumentException("No PDF paths provided.", nameof(pdfPaths));

            //PdfReaderSettings settings = new PdfReaderSettings();

            //string pdfReaderPath = settings.SumatraPath;
            if (!System.IO.File.Exists(pdfReaderPath))
                throw new FileNotFoundException("PDF Reader not found.", pdfReaderPath);

            var paths = pdfPaths
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();

            foreach (var pdfPath in paths)
            {
                if (!System.IO.File.Exists(pdfPath))
                    continue;

                int copies = PRINT_COPIES;
                if (copies < 1)
                    copies = 1;

                for (int i = 0; i < copies; i++)
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = pdfReaderPath,
                        Arguments = $"-silent -exit-when-done " +
                        $"-print-to \"{printerName}\" " +
                        $"-print-settings \"portrait,noscale\" " +
                        $"\"{pdfPath}\"",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = true
                    };

                    using (var process = Process.Start(psi))
                    {
                        process.WaitForExit(2000);
                    }
                }
            }
        }
        private static int ExtractQtyFromFileName(string pdfPath)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(pdfPath);

            // Example match: Qty_1 or Qty_5 etc.
            string pattern = @"Qty_(\d+)$";
            var match = System.Text.RegularExpressions.Regex.Match(fileName, pattern);

            if (match.Success && int.TryParse(match.Groups[1].Value, out int qty))
                return qty;

            return 1; // default if not found
        }

    }
    public class CircleTextCellRenderer : CellRenderer
    {
        private readonly string _text;
        private readonly PdfFont _font;
        private readonly float _fontSize;

        public CircleTextCellRenderer(Cell modelElement, string text, PdfFont font, float fontSize = 6f)
            : base(modelElement)
        {
            _text = text;
            _font = font;
            _fontSize = fontSize;
        }
        public override void Draw(DrawContext drawContext)
        {
            base.Draw(drawContext);

            var canvas = drawContext.GetCanvas();
            var rect = GetOccupiedAreaBBox();

            // Bigger circle
            float available = Math.Min(rect.GetWidth(), rect.GetHeight());
            float radius = available * 0.50f;

            // LEFT alignment
            float cx = rect.GetLeft() + radius + 1;
            float cy = rect.GetBottom() + rect.GetHeight() / 2;

            canvas.SaveState();

            canvas.SetFillColor(ColorConstants.BLACK);
            canvas.Circle(cx, cy, radius);
            canvas.Fill();

            // Text
            canvas.BeginText();
            canvas.SetFontAndSize(_font, _fontSize);
            canvas.SetFillColor(ColorConstants.WHITE);

            float textWidth = _font.GetWidth(_text, _fontSize);
            float ascent = _font.GetAscent(_text, _fontSize);
            float descent = _font.GetDescent(_text, _fontSize);

            float textX = cx - (textWidth / 2);
            float textY = cy - ((ascent + descent) / 2);

            canvas.SetTextMatrix(textX, textY);
            canvas.ShowText(_text);
            canvas.EndText();

            canvas.RestoreState();
        }



    }
}

