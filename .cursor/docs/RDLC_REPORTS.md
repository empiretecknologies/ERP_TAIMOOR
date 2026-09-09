# RDLC Reports

This document catalogs RDLC reports under `Empire_ERP/Reports` (source files, not `bin` copies). Report XML uses a generic `<Report Name="Report">`. Subreports were **not found** in any source RDLC. `LeftMargin`, `RightMargin`, `TopMargin`, and `BottomMargin` were **not found** in source RDLC XML (SSRS defaults apply).

POS KOT/sticker PDFs from `PdfService` and grid PDFs from `ReportController` are **not** RDLC.

---

## 1. Generation Pattern

Typical module controller (`GatePassController.GenerateReport` and the same pattern in 40+ controllers):

```
LocalReport report = new LocalReport()
Load Reports\{REPORT_NAME}.rdlc  (ContentRootPath)
EnableExternalImages = true
SetParameters(...)
optional DataSources.Add(ReportDataSource)
Render("PDF")
save under wwwroot/Client/{Module}/
```

`REPORT_NAME` usually comes from `TBL_MENU_BUILDER_DETAIL.REPORT_NAME` (must match the RDLC file name without `.rdlc`).

Package: `ReportViewerCore.NETCore` (`Microsoft.Reporting.NETCore`). Export path found in active code: **PDF only**.

Typed dataset: `Empire_ERP/Reports/Datasets/BarcodeReportDataset.xsd` (generated `BarcodeReportDataset.Designer.cs`).

---

## 2. Datasets Folder

| File | Role |
|------|------|
| `Reports/Datasets/BarcodeReportDataset.xsd` | Typed DataSet |
| `BarcodeReportDataset.Designer.cs` / `.cs` / `.xsc` / `.xss` | Generated / cache |
| `Reports/Datasets/InvoiceReport.rdlc` | Duplicate invoice layout (also under `Reports/InvoiceReport.rdlc`) |

Typed tables include names such as BarcodeReport, StockReport, StockTransferPrintReport, InvoiceReport, PurchaseBill, CashPayment, ListReport, SalesTax, PurchaseOrder, JournalVoucher, MaterialRequisition, PurchaseRequisition, BillOfMaterial, DeliveryOrderReport, InspectionServiceChargesReport, OfferLetter, ImportBillReport, BatchIssue, WorkOrder, SaleTaxInvoice, MPODetail, SalesQutation.

---

## 3. Shared Print Configuration

| Item | Value in active C# |
|------|--------------------|
| Engine | `Microsoft.Reporting.NETCore.LocalReport` |
| Output | `report.Render("PDF")` |
| Images | `EnableExternalImages = true`; company logo URI under `wwwroot/Client/Company/` |
| Path | `Path.Combine(ContentRootPath, $"Reports\\{REPORT_NAME}.rdlc")` |
| ReportViewer WinForms | Commented only in `BarcodePrintController` — not active |

---

## 4. Active Reports

### Barcode / labels

**Report:** Barcode with detail  
**RDLC File:** `Empire_ERP/Reports/BarcodeReportWithDetail.rdlc`  
**Purpose:** Item barcode labels including extra detail fields  
**Data Source:** `BarcodeReportDataset`  
**Parameters:** Not found / Not identified as a complete named list in XML beyond dataset fields (layout uses `Fields!BarcodePath`, `Fields!Rate`, etc.)  
**Dataset:** `ReportDataSet`  
**Related Code:** `BarcodePrintController`, `BarcodePrintService`, `BarcodePrintRepository`  
**Print Configuration:** Page ~4.05 × 0.98 in (landscape label); PDF bytes to client  
**Notes:** Filename selected via `REPORT_NAME`.

**Report:** Barcode without detail  
**RDLC File:** `BarcodeReportWithoutDetail.rdlc`  
**Purpose:** Compact barcode labels  
**Data Source:** `BarcodeReportDataset`  
**Parameters:** Not found / Not identified (field-driven)  
**Dataset:** `ReportDataSet`  
**Related Code:** Same as above  
**Print Configuration:** ~4.05 × 0.98 in landscape  
**Notes:** Grouping: Details / Details2.

**Report:** Barcode two labels  
**RDLC File:** `BarcodeReportWithTwoLabels.rdlc`  
**Purpose:** Two labels per layout  
**Data Source:** `BarcodeReportDataset`  
**Parameters:** Field-driven (`CompanyName`, `Barcode`, …)  
**Dataset:** `ReportDataSet`  
**Related Code:** BarcodePrint  
**Print Configuration:** ~4.05 × 0.98 in landscape  
**Notes:** Variants: `BarcodeReportWithTwoLabelsAndColor.rdlc`, `BarcodeReportWithColor.rdlc`, `BarcodeReportWithSize.rdlc` (same size/dataset pattern).

**Report:** Stock transfer carton sticker  
**RDLC File:** `StockTransferReportLabel.rdlc`  
**Purpose:** Carton/sticker for stock transfer / receive / requisition  
**Data Source:** `BarcodeReportDataset`  
**Parameters:** Voucher/barcode fields  
**Dataset:** `StockDataSet` (`StockReportDataTable` in C#)  
**Related Code:** Hardcoded in `StockTransferController`, `StockReceiveController`, `StockTransferRequisitionController`  
**Print Configuration:** ~4.1 × 3.8 in landscape  
**Notes:** Not solely menu-driven.

---

### Purchase / sales invoices

**Report:** Sales invoice  
**RDLC File:** `SalesInvoice.rdlc`  
**Purpose:** Sales invoice print from purchase-bill-style documents  
**Data Source:** Typed purchase-bill report tables  
**Parameters:** CompanyName, InvoiceNumber, PartyName, Sig1–4, Discount (sample from XML/C#)  
**Dataset:** `PurchaseBillReportDataSet`  
**Related Code:** `PurchaseBillController` / `PurchaseBillService` / `PurchaseBillRepository` (`PROC_PRINT`; menu `MD_ID` 17/18 among others)  
**Print Configuration:** 8.27 × 11.69 in portrait (A4); PDF → `wwwroot/Client/PurchaseBillReport/`  
**Notes:** Expressions include `Sum(Fields!Qty.Value)`, `Sum(Fields!Amount.Value)`, `Code.NumberToWords(...)`. Grouping by ItemName.

**Report:** Delivery challan  
**RDLC File:** `DeliveryChallan.rdlc`  
**Purpose:** Delivery challan layout for the same document family  
**Data Source:** Same family as sales invoice  
**Parameters:** Invoice-style + Discount  
**Dataset:** `PurchaseBillReportDataSet`  
**Related Code:** `PurchaseBillController` via menu `REPORT_NAME`  
**Print Configuration:** A4 portrait  
**Notes:** Qty sums in tablix.

**Report:** Packing list  
**RDLC File:** `PackingList.rdlc`  
**Purpose:** Packing list  
**Data Source:** Sales tax report dataset family  
**Parameters:** Invoice-style header  
**Dataset:** `SalesTaxReportDataSet`  
**Related Code:** `PurchaseBillController`  
**Print Configuration:** A4 portrait  
**Notes:** `FormatNumber(Fields!Qty.Value, 2)`.

**Report:** Sales tax  
**RDLC File:** `SalesTax.rdlc`  
**Purpose:** Sales tax invoice layout  
**Data Source:** Sales tax dataset  
**Parameters:** Invoice-style  
**Dataset:** `SalesTaxReportDataSet`  
**Related Code:** `PurchaseBillController` (menu MD_ID 21/22/25 among others)  
**Print Configuration:** A4 portrait  
**Notes:** Tax totals in layout.

**Report:** Shipment (purchase-bill family)  
**RDLC File:** `Shipment.rdlc`  
**Purpose:** Shipment print using sales-tax dataset  
**Data Source:** Sales tax dataset  
**Parameters:** Invoice-style  
**Dataset:** `SalesTaxReportDataSet`  
**Related Code:** `PurchaseBillController`  
**Print Configuration:** A4 portrait  
**Notes:** Distinct from import `ShipmentController` logistics screen.

**Report:** Inspection service charges  
**RDLC File:** `InspectionServiceCharges.rdlc`  
**Purpose:** Service charges invoice  
**Data Source:** Service charges dataset  
**Parameters:** Curr, CurrSig, TranId, ClientPO, …  
**Dataset:** `ServiceChargesReportDataSet`  
**Related Code:** `PurchaseBillController` (MD_ID 54), `TexSalesInvoiceController`  
**Print Configuration:** A4 portrait  
**Notes:** `Sum(Qty)`, `Sum(CommAmt)`.

**Report:** Sale invoice DDJ  
**RDLC File:** `SaleInvoiceDDJ.rdlc`  
**Purpose:** Alternate sales invoice (DDJ)  
**Data Source:** Purchase order dataset in layout  
**Parameters:** BranchName, BranchGST, OrderType, …  
**Dataset:** `PurchaseOrder`  
**Related Code:** `PurchaseBillController` when `REPORT_NAME == "SaleInvoiceDDJ"`  
**Print Configuration:** A4 portrait  
**Notes:** Special-cased in repository/controller.

**Report:** Purchase order  
**RDLC File:** `PurchaseOrder.rdlc`  
**Purpose:** PO print  
**Data Source:** Purchase order  
**Parameters:** BranchTerms, MenuTerms, Sig1–4, …  
**Dataset:** `PurchaseOrder`  
**Related Code:** `PurchaseOrderController`, `PurchaseOrderRepository`; also some purchase-bill formats  
**Print Configuration:** A4 portrait  
**Notes:** `Sum(NetAmt)`, `Sum(TaxAmt)`.

**Report:** Sale tax invoice (FBR)  
**RDLC File:** `SaleTaxInvoice.rdlc`  
**Purpose:** FBR sale tax invoice  
**Data Source:** SaleTaxInvoice  
**Parameters:** Header, InvoiceNumber, Date, Sig1–4  
**Dataset:** `SaleTaxInvoice`  
**Related Code:** `SaleTaxInvoiceController`, `SaleTaxInvoiceRepository` (`PROC_PRINT`)  
**Print Configuration:** A4 portrait  
**Notes:** Line item fields.

**Report:** Delivery feeding invoice  
**RDLC File:** `InvoiceReport.rdlc`  
**Purpose:** Half-page invoice for delivery feeding  
**Data Source:** Invoice report  
**Parameters:** SodaDate, DeliveryDate, NetAmount, BrokerAmount, …  
**Dataset:** `InvoiceReportDataSet`  
**Related Code:** `DeliveryFeedingController`, `FDeliveryFeedingController`  
**Print Configuration:** 8.3 × 5.85 in landscape  
**Notes:** Duplicate file under `Reports/Datasets/InvoiceReport.rdlc`.

**Report:** Sakhi buyer  
**RDLC File:** `SakhiBuyer.rdlc`  
**Purpose:** Buyer invoice variant  
**Data Source:** Invoice report  
**Parameters:** Invoice-style + broker  
**Dataset:** `InvoiceReportDataSet`  
**Related Code:** Delivery feeding via menu `REPORT_NAME`  
**Print Configuration:** 8.3 × 5.85 in landscape  
**Notes:** Variant `SakhiBuyerWoCmt.rdlc` (without comment).

---

### Stock transfer / DC

**Report:** Stock print  
**RDLC File:** `PrintReport.rdlc`  
**Purpose:** Stock transfer / receive / requisition / adjustment print  
**Data Source:** Stock print dataset  
**Parameters:** TransferFrom/To, VoucherNo, ShowSignature1–4  
**Dataset:** `StockPrintReportDataSet`  
**Related Code:** `StockTransferController`, `StockReceiveController`, `StockTransferRequisitionController`, `StockAdjustmentController`  
**Print Configuration:** 8.3 × 11.7 in portrait  
**Notes:** Menu-selected among several ST layouts.

**Report:** Sales quotation print  
**RDLC File:** `SalesQutationPrintReport.rdlc`  
**Purpose:** Sales quotation voucher print (master + detail)  
**Data Source:** `SalesQutation` table in `BarcodeReportDataset.xsd`  
**Parameters:** CompanyName, CompanyAddress, CompanyPhone, Header, TransactionDate, VoucherNo, PartyName, Remarks, CompanyLogo, ShowSignature1–4, ShowCompanyLogo  
**Dataset:** `SalesQutation`  
**Related Code:** `SalesQutationController`, `SalesQutationService`, `SalesQutationRepository`  
**Print Configuration:** 8.3 × 11.7 in portrait (Stock Transfer layout as visual reference only)  
**Notes:** Detail fields are ItemName, Qty, Rate, Amount (`QTY × RATE`). No Stock Transfer parameters or `StockPrintReportDataSet`. PDF saved under `wwwroot/Client/SalesQutationReport/`.

**Report:** STKTR print  
**RDLC File:** `STKTRPrintReport.rdlc`  
**Purpose:** Alternate stock transfer print  
**Data Source:** Stock print dataset  
**Parameters:** Transfer params (no Type/Description in one variant)  
**Dataset:** `StockPrintReportDataSet`  
**Related Code:** Same stock controllers  
**Print Configuration:** 8.3 × 11.7 in portrait  
**Notes:** Group → Details; `Sum(BalQty)`.

**Report:** DC print  
**RDLC File:** `DCPrintReport.rdlc`  
**Purpose:** Delivery challan style stock print  
**Data Source:** Stock print dataset  
**Parameters:** Includes Type, Reference  
**Dataset:** `StockPrintReportDataSet`  
**Related Code:** Same stock controllers  
**Print Configuration:** 8.3 × 11.7 in portrait  
**Notes:** `Sum(BalQty)`. Variant `DCPrintReportWithRate.rdlc` adds rate/`Sum(Qty1)`.

**Report:** Stock transfer Peter  
**RDLC File:** `StockTransferPeter.rdlc`  
**Purpose:** Alternate transfer layout  
**Data Source:** Stock print dataset  
**Parameters:** Full transfer params  
**Dataset:** `StockPrintReportDataSet`  
**Related Code:** Menu-selected on stock controllers  
**Print Configuration:** 8.3 × 11.7 in portrait  
**Notes:** Barcode/design fields.

**Report:** Stock adjustment label  
**RDLC File:** `StockAdjustmentReportLabel.rdlc`  
**Purpose:** Intended carton label for stock adjustment  
**Data Source:** Not found / Not identified in Reports folder  
**Parameters:** Not found / Not identified  
**Dataset:** Not found / Not identified  
**Related Code:** Hardcoded path in `StockAdjustmentController`  
**Print Configuration:** Not found / Not identified  
**Notes:** **File is referenced in C# but is not present under `Empire_ERP/Reports`.**

---

### Accounting vouchers

Shared dataset name in C#: `CashPaymentReport` bound to `reportData.Detail`. Page: A4 portrait (8.27 × 11.69 in). Expressions include `Sum(Amt)` and `Code.NumberToWords` where present.

**Report:** Cash payment  
**RDLC File:** `CashPayment.rdlc`  
**Purpose:** Cash book / payment voucher  
**Data Source:** CashPaymentReport  
**Parameters:** Company/header/signature style (set in controller)  
**Dataset:** `CashPaymentReport`  
**Related Code:** `CashBookVoucherController`  
**Print Configuration:** A4 portrait PDF  
**Notes:** Debit/credit formatting via IIf in related voucher layouts.

**Report:** Cash receipt DE  
**RDLC File:** `CashReceipt_DE.rdlc`  
**Purpose:** Cash receipt voucher  
**Data Source:** CashPaymentReport  
**Parameters:** Voucher header params  
**Dataset:** `CashPaymentReport`  
**Related Code:** `CashReceiptVoucherController`  
**Print Configuration:** A4 portrait  
**Notes:** —

**Report:** Journal voucher  
**RDLC File:** `JournalVoucher.rdlc`  
**Purpose:** Journal print  
**Data Source:** CashPaymentReport  
**Parameters:** Voucher header params  
**Dataset:** `CashPaymentReport`  
**Related Code:** `JournalVoucherController`  
**Print Configuration:** A4 portrait  
**Notes:** `IIf(Fields!Debit.Value = 0, "", FormatNumber(...))`.

**Report:** Party receipt  
**RDLC File:** `PartyReceipt.rdlc`  
**Purpose:** Party receipt voucher  
**Data Source:** CashPaymentReport  
**Parameters:** Voucher header params  
**Dataset:** `CashPaymentReport`  
**Related Code:** `PartyReceiptVoucherController`  
**Print Configuration:** A4 portrait  
**Notes:** `Sum(Amt)`.

**Report:** Party to party  
**RDLC File:** `PartyToParty.rdlc`  
**Purpose:** Party-to-party voucher  
**Data Source:** CashPaymentReport  
**Parameters:** Voucher header params  
**Dataset:** `CashPaymentReport`  
**Related Code:** `PartyToPartyController`  
**Print Configuration:** A4 portrait  
**Notes:** —

**Report:** Purchase book  
**RDLC File:** `PurchaseBook.rdlc`  
**Purpose:** Purchase book voucher  
**Data Source:** CashPaymentReport  
**Parameters:** Voucher header params  
**Dataset:** `CashPaymentReport`  
**Related Code:** `PurchaseBookVoucherController`  
**Print Configuration:** A4 portrait  
**Notes:** —

---

### Production / manufacturing

**Report:** Bill of material  
**RDLC File:** `BillOfMaterial.rdlc`  
**Purpose:** BOM print with rates  
**Data Source:** BillOfMaterialDataSet  
**Parameters:** Header/signatures from menu  
**Dataset:** `BillOfMaterialDataSet`  
**Related Code:** `BillOfMaterialController`; repo checks `REPORT_NAME == "BillOfMaterial"`  
**Print Configuration:** A4 portrait; PDF → `Client/BillOfMaterial/`  
**Notes:** `Sum(AMT)`. Variant `BillOfMaterialWoRate.rdlc` omits rate columns.

**Report:** Batch issue  
**RDLC File:** `BatchIssue.rdlc`  
**Purpose:** Batch issue document  
**Data Source:** BatchIssueDataSet  
**Parameters:** Header params  
**Dataset:** `BatchIssueDataSet`  
**Related Code:** `BatchIssueController`, `BatchIssueRepository`  
**Print Configuration:** A4 portrait  
**Notes:** `Format(QTY)`; grouping Details2.

**Report:** Daily production  
**RDLC File:** `DailyProduction.rdlc`  
**Purpose:** Daily production print  
**Data Source:** DailyProductionDataSet  
**Parameters:** Header params  
**Dataset:** `DailyProductionDataSet`  
**Related Code:** `DailyProductionRepository` print SQL; controller file is `DailyProductionController - Copy.cs` only  
**Print Configuration:** A4 portrait  
**Notes:** Live `DailyProductionController.cs` **Not found / Not identified**.

**Report:** Work order  
**RDLC File:** `WorkOrder.rdlc`  
**Purpose:** Work order print  
**Data Source:** WorkOrderDataSet  
**Parameters:** Header params  
**Dataset:** `WorkOrderDataSet`  
**Related Code:** `WorkOrderController`  
**Print Configuration:** 8.3 × 11.7 in portrait  
**Notes:** Qty/Amount fields.

---

### Requisitions / orders

**Report:** Material requisition  
**RDLC File:** `MaterialReq.rdlc`  
**Purpose:** Material requisition  
**Data Source:** MaterialRequisition  
**Parameters:** Header params  
**Dataset:** `MaterialRequisition`  
**Related Code:** `MaterialRequisitionController`  
**Print Configuration:** A4 portrait  
**Notes:** —

**Report:** Purchase requisition  
**RDLC File:** `PurchaseReq.rdlc`  
**Purpose:** Purchase requisition  
**Data Source:** PurchaseRequisition  
**Parameters:** Header params  
**Dataset:** `PurchaseRequisition`  
**Related Code:** `PurchaseRequisitionController`  
**Print Configuration:** A4 portrait  
**Notes:** —

**Report:** Delivery order report  
**RDLC File:** `DeliveryOrderReport.rdlc`  
**Purpose:** Delivery order with dataset  
**Data Source:** DeliveryOrderReport  
**Parameters:** Header params  
**Dataset:** `DeliveryOrderReport`  
**Related Code:** `DeliveryOrderController`, `DeliveryOrderService` / `Repository`  
**Print Configuration:** A4 portrait; PDF → `Client/DeliveryOrderReport/`  
**Notes:** Distinct from parameter-only `DeliveryOrder.rdlc`.

---

### MPO

**Report:** MPO master  
**RDLC File:** `mpo_master.rdlc`  
**Purpose:** Merchant purchase order header print  
**Data Source:** None bound in controller (parameters only)  
**Parameters:** 30+ report parameters in `MerchantPurchaseOrderController`  
**Dataset:** None  
**Related Code:** `MerchantPurchaseOrderController`  
**Print Configuration:** A4 portrait  
**Notes:** Layout is parameter-driven.

**Report:** MPO detail  
**RDLC File:** `mpo_detail.rdlc`  
**Purpose:** MPO layout/trims/detail print  
**Data Source:** MPODetail  
**Parameters:** Header/detail params  
**Dataset:** `MPODetail`  
**Related Code:** `MpoLayoutController`, `MpoTrimsController`, `MerchantPurchaseOrderDetailController`  
**Print Configuration:** A4 portrait  
**Notes:** `mpo_detail2.rdlc` and `mpo_detail_dub.rdlc` have **no C# filename reference**.

---

### Lists / import / HR / misc

**Report:** List  
**RDLC File:** `List.rdlc`  
**Purpose:** Landscape list (purchase-sale format / import manifest)  
**Data Source:** ListReportDataSet  
**Parameters:** List header params  
**Dataset:** `ListReportDataSet`  
**Related Code:** `PurchaseSaleFormatController` (single + multi-bill), `ImportManifestController`  
**Print Configuration:** 11.69 × 8.27 in landscape  
**Notes:** —

**Report:** IGM  
**RDLC File:** `IGMReport.rdlc`  
**Purpose:** Import general manifest list layout  
**Data Source:** ListReportDataSet  
**Parameters:** List header params  
**Dataset:** `ListReportDataSet`  
**Related Code:** `PurchaseSaleFormatController` via menu `REPORT_NAME`  
**Print Configuration:** Landscape A4-width  
**Notes:** —

**Report:** Import bill  
**RDLC File:** `ImportBill.rdlc`  
**Purpose:** Import bill  
**Data Source:** ImportBillReportDataSet  
**Parameters:** Bill header params  
**Dataset:** `ImportBillReportDataSet`  
**Related Code:** `ImportReportController` — **hardcoded** path (REPORTID 131 in code comments/logic)  
**Print Configuration:** A4 portrait; PDF → `Client/PurchaseBillReport/`  
**Notes:** Not only menu-driven.

**Report:** Gate pass  
**RDLC File:** `GatePass.rdlc`  
**Purpose:** Gate pass  
**Data Source:** None (parameters only; dataset add is commented in controller)  
**Parameters:** CompanyName, CompanyAddress, CompanyPhone, CompanyLogo, ShowCompanyLogo, Header, V_DATE, VOUCHER_NO, PARTY_CODE, DUE_NO, DRIVER, VEHICLE, QUANTITY, LOT_NO, ITEM_CODE, UNIT, MENU_SIG1–4, MENU_TERMS  
**Dataset:** None  
**Related Code:** `GatePassController.GenerateReport`  
**Print Configuration:** A4 portrait; PDF → `Client/GatePass/`  
**Notes:** Verified parameter list from controller.

**Report:** Offer letter  
**RDLC File:** `OfferLetterReport.rdlc`  
**Purpose:** HR offer letter  
**Data Source:** None (parameters)  
**Parameters:** CAN_NAME, JOB_TITLE, TOTAL_PACKAGE, … (HROfferLetter controller)  
**Dataset:** None  
**Related Code:** `HROfferLetterController`  
**Print Configuration:** A4 portrait; PDF → `Client/HROfferLetter/`  
**Notes:** —

**Report:** Delivery order (parameter layout)  
**RDLC File:** `DeliveryOrder.rdlc`  
**Purpose:** Parameter-driven document used by several modules  
**Data Source:** None in several callers  
**Parameters:** V_DATE, TBAG, LOT_NO, and DeliveryOrder-style fields  
**Dataset:** None in those callers  
**Related Code:** `MembershipCardController`, `DeliveryFormatController`, `HRJobPostController`, `POSMappingController`  
**Print Configuration:** A4 portrait  
**Notes:** Same RDLC reused; not always a delivery order.

---

## 5. Controller Map (RDLC users)

| Controller | Typical output folder / notes |
|------------|-------------------------------|
| BarcodePrintController | PDF bytes to client |
| PurchaseBillController | `Client/PurchaseBillReport/` |
| PurchaseOrderController | `Client/PurchaseOrder/` |
| SaleTaxInvoiceController | `Client/SaleTaxInvoice/` |
| DeliveryFeedingController / FDeliveryFeedingController | `Client/DeliveryFeeding/` (and F variant) |
| StockTransfer / StockReceive / StockTransferRequisition / StockAdjustment | Dynamic ST reports + sticker |
| SalesQutationController | `Client/SalesQutationReport/` |
| BillOfMaterialController | `Client/BillOfMaterial/` |
| BatchIssueController | PDF |
| WorkOrderController | PDF |
| MaterialRequisition / PurchaseRequisition | PDF |
| DeliveryOrderController | `Client/DeliveryOrderReport/` |
| Journal / CashBook / CashReceipt / PartyReceipt / PartyToParty / PurchaseBook | PDF |
| PurchaseSaleFormatController | `Client/PurchaseSaleFormatList/` |
| ImportReportController | Hardcoded ImportBill |
| ImportManifestController | PDF |
| GatePassController | `Client/GatePass/` |
| HROfferLetterController | `Client/HROfferLetter/` |
| MerchantPurchaseOrder / MpoLayout / MpoTrims / MerchantPurchaseOrderDetail | PDF |
| TexSalesInvoiceController | PDF |
| MembershipCard / DeliveryFormat / HRJobPost / POSMapping | Parameter RDLC |

**Commented RDLC:** `HRCandidateController`, `HRInterviewScheduleController`, `HRInterviewFeedbackController`, `MachineInfoController`.

---

## 6. Legacy / Unused Copies

Present under `Reports/` but **not referenced by filename** in active C#:

`BarcodeReportWithTwoLabels - Copy.rdlc`, `BarcodeReportWithTwoLabels - Copy (2).rdlc`, `BatchIssue_Old.rdlc`, `BillOfMaterial_old.rdlc`, `DailyProduction_old.rdlc`, `DeliveryOrder - Copy.rdlc`, `DeliveryOrder - Copy (2).rdlc`, `List - copy.rdlc`, `OLDDCPrintReport - Copy.rdlc`, `OLDInvoiceReport.rdlc`, `OLDInvoiceReport(4).rdlc`, `OLDInvoiceReport_Old.rdlc`, `OLDSTKTRPrintReport.rdlc`, `PackingList_Backup.rdlc`, `PrintReport - Copy.rdlc`, `PurchaseOrder_old.rdlc`, `SalesInvoice - Backup.rdlc`, `SalesInvoice - Backup2.rdlc`, `SalesInvoice - Copy (2).rdlc`, `SalesInvoice_Old.rdlc`, `SalesTax - Copy.rdlc`, `mpo_detail_dub.rdlc`, `mpo_detail2.rdlc`, `Reports/Datasets/InvoiceReport.rdlc` (duplicate).

---

## 7. Non-RDLC Printing (for contrast)

| Component | Mechanism |
|-----------|-----------|
| `ReportController.GeneratePDF` | iTextSharp from grid JSON |
| `PdfService` | iText 7 KOT / stickers |
| POS Razor views | `PointOfSale_2`, `SaleReceipt`, `POS_KOT`, `STICKER` |

---

## 8. Notes

- Menu-configured `REPORT_NAME` means the database, not the C# switch, often chooses which RDLC file is loaded.
- Number-to-words uses a custom report `Code.NumberToWords` expression on some invoice/voucher layouts.
- Grouping and sorting are tablix-specific; details above are from XML where they were identifiable.
- Exact remaining report parameter names for every file: **Not found / Not identified** unless listed from controller code (GatePass is fully listed).
