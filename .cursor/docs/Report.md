# Grid PDF Reports (iTextSharp + DevExtreme Export)

This document describes how **grid-based PDF reports** are generated in Empire ERP, with emphasis on the **PartyReports** form and `ReportController.GeneratePDF`.

This is **not** the RDLC/`LocalReport` print path. RDLC reports are documented separately in `.cursor/docs/RDLC_REPORTS.md`.

---

## 1. Overview

There are **two PDF engines** used by report grids:

| Engine | Where | Used by |
|--------|--------|---------|
| **Server iTextSharp** | `Empire_ERP/Controllers/ReportController.cs` → `GeneratePDF` | `empr_helper.DxGridBindingForReportsWithSetting_Aging`, `empr_helper.editableDxGridbinding_ItemImageUpload` |
| **Client DevExtreme + jsPDF** | Browser via `DevExpress.pdfExporter.exportDataGrid` | `empr_helper.DxGridBindingForReportsWithSetting` (and most other report binders) |

**PartyReports** uses **both**, depending on report ID and which grid tab is exported.

Theme color used in headers/PDF styling: `#055A87` (`BaseColor(0x05, 0x5A, 0x87)`).

---

## 2. End-to-End Flow (PartyReports → PDF)

```
PartyReports/Index.cshtml
        │
        ▼
empr_PartyReports.js  (filters + Generate Report)
        │
        ▼
POST /PartyReports/GenerateReport
        │
        ▼
PartyReportsController.GenerateReport
        │
        ▼
PartyReportService.GetReportData
        │
        ▼
PartyReportRepository.GetReportData  (SQL: EXEC PPROC ...)
        │
        ▼
CustomPartyReport rows → JSON
        │
        ▼
empr_PartyReports.InitReportGrid / InitTradeReportGrid / InitAgingReportGrid
        │
        ├── DxGridBindingForReportsWithSetting_Aging  ──► POST /Report/GeneratePDF (iTextSharp)
        │                                                 Group OR Normal branch
        │
        └── DxGridBindingForReportsWithSetting ─────────► Client jsPDF + DevExpress.pdfExporter
                                                          (grouping handled by DevExtreme grid)
```

### 2.1 Form / UI entry

| Item | Location |
|------|----------|
| View | `Empire_ERP/Views/PartyReports/Index.cshtml` |
| Script | `Empire_ERP/wwwroot/js/Customjs/empr_PartyReports.js` |
| Helper | `Empire_ERP/wwwroot/js/Customjs/empr_helper.js` |
| Controller | `Empire_ERP/Controllers/PartyReportsController.cs` |
| Service | `Empire_ERP.Core/Services/PartyReportService.cs` |
| Repository | `Empire_ERP.Infrastructure/Repositories/PartyReportRepository.cs` |
| Models | `Empire_ERP.Core/Entities/PartyReport.cs` (`PartyReport`, `CustomPartyReport`) |

**Index.cshtml** sets:

- `empr_helper.companyName` from `ViewBag.CompanyName`
- Period dates, branch, parties
- Tabs: REPORT OPTION / REPORT VIEW (REPORT, TRADE, Aging Report)
- Grids: `#ReportGridContainer`, `#TradeGridContainer`, `#AgingReportGridContainer`

**Generate button** → `empr_PartyReports.ValidateInfo()` → `GenerateReport()` → `GetReport()`.

### 2.2 Data load

1. `GetDataToSave()` builds the request (report ID, dates, control, party, nature, branch, category, period).
2. `POST /PartyReports/GenerateReport` with `PartyReport`.
3. `PartyReportService.GetReportData` allows IDs: `10, 11, 38, 39, 51, 52, 58, 70, 97, 98, 99, 102, 136`.
4. Repository runs `EXEC PPROC 'ReportID','From','To',...` and maps rows into `CustomPartyReport`.
5. Reports `38`/`39` require chart password check before data.

### 2.3 Grid binding (columns + grouping)

`empr_PartyReports.InitReportGrid(dataSrc, reportId)`:

- Builds DevExtreme column definitions (captions, formats, `groupIndex`, summaries, templates).
- Sets `empr_helper.reportName`, `fromDate`, `toDate`, and for aging-style reports `lastAmt` / `lastDate`.
- Binds the main grid as follows:

| Report IDs (main grid) | Binder | PDF engine |
|------------------------|--------|------------|
| `10, 11, 51, 52, 136` (+ delayed rebind also for `97, 98, 99, 102`) | `DxGridBindingForReportsWithSetting_Aging` | **Server** `/Report/GeneratePDF` |
| All other PartyReports IDs | `DxGridBindingForReportsWithSetting` | **Client** DevExpress/jsPDF |

Additional tabs:

| Tab | Method | Binder | PDF engine |
|-----|--------|--------|------------|
| TRADE (report 10/38) | `InitTradeReportGrid` | `DxGridBindingForReportsWithSetting` on `#TradeGridContainer` | Client |
| Aging (report 10) | `InitAgingReportGrid` | `DxGridBindingForReportsWithSetting` on `#AgingReportGridContainer` | Client |

**Examples of built-in grouping (`groupIndex`) in PartyReports columns:**

- Ledger-style (`10`, `38`, `70`): `accountName` → `groupIndex: 1`
- Summary (`11`): `accountName` group 1, `category` group 2
- Aging (`51`/`52` main path): `accountName` / `category` grouped
- Aging tab (`InitAgingReportGrid`): `partyName` → `groupIndex: 1`

Users can also change grouping via the **group panel** and **column chooser**; those changes affect PDF output.

---

## 3. Custom layout / editing before PDF

PDF content follows the **current grid state**, not a fixed server template.

### 3.1 State storing (custom column layout)

Both `DxGridBindingForReportsWithSetting` and `DxGridBindingForReportsWithSetting_Aging` enable:

```
stateStoring → localStorage key = report fileName
saves: dataField, visible, visibleIndex, groupIndex, width
```

Refresh toolbar button clears that key and reloads the page.

Effects on PDF:

- Hidden columns are omitted.
- `groupIndex` values become group headers in PDF (server or client).
- Column order/visibility follows `getVisibleColumns()`.

### 3.2 Group panel / column chooser

- `groupPanel.visible: true`
- `columnChooser.enabled: true`
- Dragging columns into the group panel changes `groupIndex` → changes Group vs Normal server path when using Aging binder.

### 3.3 Batch editing (Doc images) — related server PDF path

`empr_helper.editableDxGridbinding_ItemImageUpload`:

- Batch editing enabled (`allowUpdating: true`), especially for `doc`.
- PDF export posts to `/Report/GeneratePDF`.
- Server `AddMultiGroupedTableList` embeds `Doc` images from `wwwroot` (resize/encode JPEG) when present.

PartyReports main grids do **not** use this editable binder; image embedding applies to the Item Image Upload report grids that call the same controller.

### 3.4 Short ledger / calculated display

PartyReports may hide Transaction # / Description (`SHORTLEDGER`), and uses `calculateCellValue` / `cellTemplate` for Balance display (parentheses + red for negatives). Server PDF uses raw dataSource field values mapped by **caption**, so display templates are not re-run server-side; numeric formatting is reapplied in C#.

---

## 4. Client PDF path (Normal / Group via DevExtreme)

**Entry:** `empr_helper.DxGridBindingForReportsWithSetting` → `onExporting` when `e.format === 'pdf'`.

**Steps:**

1. Create `jsPDF` (portrait/landscape A4).
2. Draw header: company, report name, From/To, printed date/time (theme `#055a87`).
3. `DevExpress.pdfExporter.exportDataGrid({ jsPDFDocument, component, customizeCell, ... })`.
4. Re-apply header + page numbers on each page.
5. Open blob URL in a new window.

**Grouping:** DevExtreme exports group rows / group footers / total footers from the live grid. No `ReportController` call.

**Formatting in `customizeCell`:**

- Zero values → blank
- Date placeholders (`1900-01-01`, etc.) → blank
- Debit/credit/balance-style fields → comma format; negatives → `(abs)` + red
- Transaction columns → theme text color
- Header/group/footer → bold sizing

---

## 5. Server PDF path (`ReportController`) — Group vs Normal

**Entry points that POST to `/Report/GeneratePDF`:**

1. `DxGridBindingForReportsWithSetting_Aging` (PartyReports main grid for aging/ledger-style IDs)
2. `editableDxGridbinding_ItemImageUpload` (image upload grids; container `#gridContainer`)

### 5.1 Client payload preparation (Aging binder)

In `onExporting` (`e.format === 'pdf'`):

1. Resolve grid instance(s) — typically `#ReportGridContainer`; optionally `#SubGridContainer` for “Master Plaining Sheet”.
2. For each grid:
   - `visibleCols` from `getVisibleColumns()` (`dataField` + `caption`)
   - `groupColsCap` = captions of columns with `groupIndex >= 0` (sorted by `groupIndex`)
   - `groupCols` = dataFields of those group columns
   - `GridData` = dataSource rows remapped so **keys are captions** (not dataFields)
   - `Totals` from DevExtreme summary total items
3. POST JSON:

```json
{
  "CompanyName": "...",
  "ReportName": "...",
  "lastDate": "...",
  "lastAmount": 0,
  "From": "...",
  "To": "...",
  "IsLandscape": true/false,
  "Grids": [
    {
      "GridTitle": "...",
      "GridData": [ { "Date": "...", "Debit": 1, ... } ],
      "GroupColumnsCap": [ "A/c Name", "Category" ],
      "GroupColumns": [ "accountName", "category" ],
      "Totals": { },
      "IsLandscape": true
    }
  ]
}
```

4. Response is a PDF blob opened in a new tab (`FullReport.pdf`).

### 5.2 Controller entry: `GeneratePDF`

**Class:** `Empire_ERP.Controllers.ReportController`  
**Method:** `GeneratePDF([FromBody] PDFRequest request)`

**Models (same file):**

| Class | Role |
|-------|------|
| `PDFRequest` | Company/report meta, dates, landscape, `Grids`, `lastDate`, `lastAmount` |
| `GridRequest` | `GridData` (`JArray`), `GridTitle`, `GroupColumnsCap` |
| `GridReport` | Internal: `DataTable`, visible columns, group captions |
| `PdfPageHeader` | `PdfPageEventHelper` — repeating page header |
| `PageNumberCellEvent` | `Page : N of M` in header |

**Pipeline per grid:**

1. Skip empty `GridData`.
2. Deserialize to `List<Dictionary<string, object>>` → `DataTable` (column names = captions).
3. `SortDataTableByGroups(ref dt, GroupColumnsCap)` — sorts by all group columns ASC when groups exist.
4. `VisibleColumns` = all DataTable columns **except** group columns (groups become headers, not data columns).
5. Build `GridReport` → `AddGridToPDF(...)`.
6. Close document → return `File(..., "application/pdf", "FullReport.pdf")`.

**Document setup:**

- Library: **iTextSharp** (`Document`, `PdfWriter`, `PdfPTable`)
- Page: A4 or A4 landscape; margins `25, 25, 65, 40`
- Font: `wwwroot/fonts/TCM_____.TTF` (default); Arial Bold from `C:\Windows\Fonts\arialbd.ttf` for headers/totals
- Page event: `PdfPageHeader` (company, report, From/To, printed date/time, page numbers)

**Second-grid banner:** when `gridCount == 1`, `AddGridToPDF` inserts a theme bar with grid title + **Last Date** + **Last Amount** (from `PDFRequest`).

### 5.3 Branch: Group-wise vs Normal

Inside `AddGridToPDF`:

```csharp
if (grid.GroupColumnsCap != null && grid.GroupColumnsCap.Count > 0)
    GenerateDynamicMultiLevelGroupedPDF(...);   // GROUP-WISE
else
    AddMultiGroupedTableList(...);              // NORMAL (flat)
```

| Mode | Condition | Method |
|------|-----------|--------|
| **Group-wise PDF** | `GroupColumnsCap.Count > 0` | `GenerateDynamicMultiLevelGroupedPDF` |
| **Normal PDF** | no group columns | `AddMultiGroupedTableList` |

---

## 6. Group-wise PDF generation (detail)

**Method:** `ReportController.GenerateDynamicMultiLevelGroupedPDF`

### 6.1 Purpose

Render multi-level grouped reports (e.g. A/c Name → Category) with:

- Group header rows
- Column headers under the **first** group level
- Per-level subtotals
- Grand totals

### 6.2 Data prerequisites

- Rows already sorted by `SortDataTableByGroups` on `GroupColumnsCap`.
- Group column values remain in the `DataTable`; they are **not** in `visibleColumns`.
- Group detection compares consecutive row values per level.

### 6.3 Layout behavior

1. Create `PdfPTable` with fixed widths for known captions (`Date`, `Transaction #`, `Debit`, `Credit`, `Balance`, aging columns, etc.; default `80f`).
2. For each data row, for each group level `0..n-1`:
   - If group value changed:
     - Emit totals for closing previous group(s) (`AddTotals`).
     - **Level 0:** group header (`"{Column} : {Value}"`, bold ~7.5pt) + full column header row (theme color).
     - **Level > 0:** subgroup header only (~7pt), no repeated column headers.
     - Reset lower-level group values and totals.
3. Emit detail cells:
   - Numeric / month-named columns: accumulate into each level’s totals + grand totals; format `{0:N0}`; negatives as `(abs)` in red.
   - Dates: blank if `1900-01-01`; else `dd-MM-yy`.
   - Transaction # uses theme color.
4. After all rows: close remaining group totals (deepest first), then grand totals if any non-zero.
5. `document.Add(table)`.

### 6.4 Totals

**Method:** `AddTotals`

- Builds a footer row across visible columns.
- Shows sums for configured amount columns (Debit, Credit, Amount, Due Amount, aging buckets, Balance, etc.) and month-named columns.
- Zero → blank; negative → red parentheses.

**Helpers:**

- `IsNumericColumn(string)` — caption contains known amount keywords
- `IsNumericValue(string)` — parse after removing commas

---

## 7. Normal PDF generation (detail)

**Method:** `ReportController.AddMultiGroupedTableList`

Used when **no** `GroupColumnsCap` are present (flat export).

### 7.1 Purpose

Render a single flat table with:

- One column header row (`table.HeaderRows = 1`)
- Detail rows
- Optional group logic if `groupColumns` were passed (method still accepts a list), but PartyReports Aging binder only reaches this method with an **empty** list
- Grand totals (and group totals if groups were supplied)

### 7.2 Layout behavior

1. Build `PdfPTable` with the same style of width map (Doc width differs: `30f` here vs `80f` in grouped path).
2. Header cells: CamelCase split → title case; theme bold ~7pt; top/bottom border.
3. Detail rows:
   - Numeric: right-aligned `N0`; blank for empty/zero.
   - Dates: blank sentinel / `dd-MM-yy`.
   - **`Doc` column:** load image from `wwwroot` + path, high-quality resize to 120×120, JPEG quality 80, scale into cell ~40×40 (`GetEncoder` helper). Fallback to text if missing.
4. After rows: any open group totals (if groups existed) + grand totals via `AddTotals`.
5. `document.Add(table)`.

### 7.3 Normal vs Group differences (summary)

| Aspect | Normal (`AddMultiGroupedTableList`) | Group-wise (`GenerateDynamicMultiLevelGroupedPDF`) |
|--------|-------------------------------------|----------------------------------------------------|
| Trigger | Empty `GroupColumnsCap` | Non-empty `GroupColumnsCap` |
| Column headers | Once at top | Repeated under each level-0 group |
| Group headers | N/A (typical PartyReports use) | Multi-level `Column : Value` rows |
| Sorting | No group sort needed | `SortDataTableByGroups` |
| Doc images | Supported | Not rendered as images (text path) |
| Negative numbers | Plain `N0` (no red brackets in detail) | Red `(abs)` in detail + totals |
| Month columns | Via `IsNumericColumn` only | Explicit month-name regex in totals |

---

## 8. Page header / footer rendering

**Class:** `PdfPageHeader : PdfPageEventHelper`

- `OnOpenDocument` — create total-pages template  
- `OnEndPage` — write company, report name, From/To bar, printed date/time, page cell  
- `OnCloseDocument` — fill total page count into template  

**Class:** `PageNumberCellEvent` — draws `Page : {current} of {total}`.

Client path draws equivalent header/footer with jsPDF in `onExporting`.

---

## 9. Important file / method reference map

### Controllers

| File | Members |
|------|---------|
| `Empire_ERP/Controllers/ReportController.cs` | `GeneratePDF`, `AddGridToPDF`, `GenerateDynamicMultiLevelGroupedPDF`, `AddMultiGroupedTableList`, `SortDataTableByGroups`, `AddGroupHeader`, `AddTotals`, `IsNumericColumn`, `IsNumericValue`, `GetEncoder`, `GridReport`, `PDFRequest`, `GridRequest`, `PdfPageHeader`, `PageNumberCellEvent` |
| `Empire_ERP/Controllers/PartyReportsController.cs` | `Index`, `GetControls`, `GetRegions`, `GetParties`, `GetReportTypes`, `GenerateReport` |

### Core / Infrastructure

| File | Role |
|------|------|
| `Empire_ERP.Core/Entities/PartyReport.cs` | Request + `CustomPartyReport` row DTO |
| `Empire_ERP.Core/Services/PartyReportService.cs` | Allowed report IDs + `GetReportData` |
| `Empire_ERP.Core/Interfaces/IPartyReportService.cs` | Service contract |
| `Empire_ERP.Infrastructure/Repositories/PartyReportRepository.cs` | `PPROC` execution + mapping |

### Frontend

| File | Role |
|------|------|
| `Empire_ERP/Views/PartyReports/Index.cshtml` | Filters, tabs, grid hosts, company name init |
| `Empire_ERP/wwwroot/js/Customjs/empr_PartyReports.js` | Report options, data fetch, column defs, grid init |
| `Empire_ERP/wwwroot/js/Customjs/empr_helper.js` | `DxGridBindingForReportsWithSetting`, `DxGridBindingForReportsWithSetting_Aging`, `editableDxGridbinding_ItemImageUpload`, date helpers, shared PDF/Excel export |

### Assets

| Path | Role |
|------|------|
| `Empire_ERP/wwwroot/fonts/TCM_____.TTF` | Default embedded PDF body font |
| `C:\Windows\Fonts\arialbd.ttf` | Bold headers/totals (server) |

---

## 10. PartyReports: which PDF path for which action

```
User clicks Export → PDF on a grid
        │
        ├─ Main #ReportGridContainer
        │     ├─ Report IDs using *_Aging binder
        │     │     → Build GroupColumnsCap from groupIndex
        │     │     → POST /Report/GeneratePDF
        │     │           ├─ GroupColumnsCap.Any() → GenerateDynamicMultiLevelGroupedPDF
        │     │           └─ else                 → AddMultiGroupedTableList
        │     │
        │     └─ Other report IDs
        │           → DevExpress.pdfExporter + jsPDF (client)
        │
        ├─ #TradeGridContainer
        │     → Client DevExpress/jsPDF
        │
        └─ #AgingReportGridContainer
              → Client DevExpress/jsPDF
```

**Practical meaning of Group vs Normal on PartyReports (server path):**

- If the grid still has columns with `groupIndex` (default definitions or user-grouped) → **Group-wise** server PDF.
- If the user clears all grouping from the group panel before export → **Normal** server PDF (flat table).

---

## 11. Out of scope / related systems

- **RDLC** voucher/bill prints (`LocalReport.Render("PDF")`) — see `RDLC_REPORTS.md`.
- Excel export on the same grids uses ExcelJS / `DevExpress.excelExporter` (not covered in depth here).
- `SPartyReports` mirrors PartyReports patterns for some trade/item reports and can feed PartyReports trade tabs.

---

## 12. Source-of-truth notes

- Branching between Group and Normal PDFs is **only** in `AddGridToPDF` based on `GroupColumnsCap`.
- Caption-based JSON keys are required because server `DataTable` columns and group lists use **captions**, not dataFields.
- PartyReports data APIs do not generate PDFs; they only supply JSON for the grid. PDF is produced at export time from the bound grid state.
