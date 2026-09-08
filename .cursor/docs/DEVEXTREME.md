# DevExtreme

This document describes DevExtreme usage as implemented in Empire ERP. No new widgets or patterns are proposed.

---

## 1. Assets and Version

Loaded from `Views/Shared/_Layout.cshtml` (authenticated screens):

| Asset | Path | Version in file header |
|-------|------|------------------------|
| JS | `wwwroot/js/dx.all.js` | **23.1.6** |
| CSS | `wwwroot/css/dx.common.css` | **19.2.7** |
| CSS | `wwwroot/css/dx.generic.custom-scheme.css` | **19.2.7** (ThemeBuilder generic scheme) |

Helpers loaded after `dx.all.js`:

- `~/dx/ajaxhelper.js` → `wwwroot/Dx/ajaxhelper.js`
- `~/dx/ati_dxHelper.js` → `wwwroot/Dx/ati_dxHelper.js`
- `~/js/customjs/empr_helper.js`

Login pages also include DevExtreme CSS/JS. `Views/Login/Details.cshtml` references `~/dx/dx.all.js` (`wwwroot/Dx/`). Extra files `wwwroot/Dx/dx.light.css` and `wwwroot/Dx/dx.common.css` are **not** used by `_Layout.cshtml`.

A commented CDN reference to DevExtreme **23.2.4** exists in the layout; it is not the active script.

**Note:** JS 23.1.6 is paired with theme CSS 19.2.7.

CDN scripts used with export: exceljs, FileSaver, babel-polyfill (layout). Grids use `DevExpress.excelExporter` / `DevExpress.pdfExporter`.

Razor views do **not** instantiate DevExtreme widgets in markup. Widgets are created in Customjs on empty `div` hosts.

---

## 2. Components Used

Counts are Customjs files under `wwwroot/js/Customjs` that contain the API name.

| Component | Files (approx.) | Status |
|-----------|-----------------|--------|
| `dxDataGrid` | 79 | Primary grid |
| `dxSelectBox` | 103 | Primary dropdown |
| `dxDropDownBox` | 37 | Dropdown with nested grid (`ati_dxHelper.DxGridBoxDropdown`) |
| `dxTreeList` | 13 | Chart of accounts, item groups, roles, etc. |
| `dxDateBox` | 1 (`empr_DeliveryOrder.js`) | Grid `editorType` only |
| `dxCheckBox` | 5 | BarcodePrint, BankDetail, HRSetup, PurchaseRequisition, Shipment |
| `dxButton` | 1 (`empr_helper.js` toolbar refresh) | Via `onToolbarPreparing` |
| `dxPopup` | 0 | **Not found / Not identified** |
| `dxForm` | commented only | CommMap, CustomerPricing |
| `dxLookup` | 0 widget | Grid column `lookup: { }` is used instead |
| `dxNumberBox` / `dxTextBox` / `dxTabPanel` / `dxTreeView` / `dxTagBox` / `dxLoadPanel` / `dxValidator` / `dxPivotGrid` / `dxChart` | 0 | **Not found / Not identified** in Customjs |
| `dxHtmlEditor` | commented in `empr_Notes.js` | Not active |

---

## 3. Data Sources and AJAX Integration

**Dominant pattern (active):**

1. `ajaxHelper.ajaxGetJson('/Controller/Action', ...)` loads a JSON array.
2. The array is passed to `empr_helper.dxGridbindingVouchers` (or an editable helper) as `dataSource`.
3. `remoteOperations: false` — filtering/sorting/paging run on the client against that array.
4. Save reads `$('#DetailContainer').dxDataGrid('instance').option('dataSource')` and POSTs JSON.

**Not used in production call sites:** DevExtreme `CustomStore` insert/update/remove CRUD. `dxGridbindingLazyLoading` implements `CustomStore` POST (`skip`, `take`, `sort`, `filter`, `group`) but **all call sites found are commented out**.

Typical URLs:

| Action | Example |
|--------|---------|
| List / search | `GET /ItemMaster/QuickSearch`, `GET /Company/QuickSearch`, `GET /JournalVoucher/GetJournalVouchers` |
| Load one | `GET /PurchaseBill/GetPurchaseBillByCode?code=` |
| Pick | `GET /PurchaseBill/GetPickDataByParty`, `GET /PurchaseBill/GetBarcodeList` |
| Save | `POST` module save action |
| POS | `/POSTransactions/GetPayQuickSearch`, `GetCustomerHistory`, `GetItemsMasterByGroup`, `GetBanksName`, `GetPartyName` |

Lookup lists for SelectBox/grid lookup: `{ key, value }` (sometimes `data.data` from wrapper).

---

## 4. dxDataGrid — Project Patterns

Reusable implementations live in `empr_helper.js`. Module files mainly supply **column definitions** and AJAX.

### 4.1 Quick search / list — `dxGridbindingVouchers`

**Signature:** `dxGridbindingVouchers(div, columns, datasrc, fileName, selectionMode, extraOptions)`

**Used for:** `#gridContainer` on most screens (`empr_Company.CreateGrid`, `empr_ItemMaster.CreateGrid`, POS quick search, pick grids).

Verified defaults:

| Option | Value |
|--------|--------|
| `dataSource` | In-memory array |
| `remoteOperations` | `false` |
| `paging.pageSize` | `10` |
| `pager.allowedPageSizes` | `[100, 200, 300, 'all']` |
| `filterRow.visible` | `true` |
| `headerFilter.visible` | `true` (with search) |
| `searchPanel.visible` | `true` |
| `groupPanel.visible` | `true` |
| `selection.mode` | `"multiple"` unless 5th argument set (e.g. `"single"` for pick grids) |
| `columnAutoWidth` | `true` |
| `allowColumnResizing` | `true` |
| `rowAlternationEnabled` | `true` |
| `allowColumnReordering` | `true` |
| `export` | Excel + PDF exporters |
| `onToolbarPreparing` | Refresh `dxButton` clearing localStorage |
| `onContentReady` | Column chooser click override |
| `onCellPrepared` | Blank default dates |
| `onExporting` | Export filename from `fileName` |
| Summary | Count on Action; sum on qty-style columns (helper logic) |

Action columns are custom `cellTemplate` HTML (edit/copy icons), often fixed left.

### 4.2 Transaction line grids — `editableDxGridbindingForTransactionsVouchers`

**Signature:** `editableDxGridbindingForTransactionsVouchers(div, columns, datasrc, fileName, firstColumn, selectionMode)`

**Used for:** `#DetailContainer` (Journal Voucher, Purchase Bill, and similar).

Verified options:

| Option | Value |
|--------|--------|
| `editing.mode` | `'batch'` |
| `allowUpdating` / `allowAdding` | `true` |
| `allowDeleting` | `false` (delete via custom cell buttons) |
| `startEditAction` | `'click'` |
| `newRowPosition` | `'first'` |
| `paging.enabled` | `false` |
| `filterRow.visible` | `false` |
| `searchPanel.visible` | `true` |
| `groupPanel.visible` | `true` |
| `headerFilter.search.enabled` | `false` |
| `height` | `300` |
| `showBorders` | `true` (set in helper) |
| `export.enabled` | `false` |
| `stateStoring` | custom localStorage keyed by `fileName` (enabled flag commented in places) |
| `remoteOperations` | `false` |

Related helpers: `editableDxGridbinding`, `editableDxGridbindingForTransactions`, `editableDxGridbindingForPurchaseSale`.

Row add/clone/delete often manipulate `dataSource` in module JS (`unshift`, `deleteRow`, `saveEditData`) rather than grid `editing.allowDeleting`.

### 4.3 Other grid helpers

| Function | Role |
|----------|------|
| `dxGridbinding` | Read-only with export/group/filter |
| `dxGridbindingKnockOff` | Knock-off |
| `dxGridbindingForMultiBillPrint` | Multi-bill print selection |
| `dxGridbindingVouchersForApproval` | Approval list |
| `dxGridbindingWithoutFeatures` | Minimal |
| `dxGridbindingForReports` | Report inquiry grids |
| `dxGridbindingForStockReceive` | Stock receive |
| `dxGridBindingWithSearch` | Search variant |
| `MasterDetailDxGridBinding` | Master-detail; used in `empr_SalesInvoiceReport.js` |
| `dxGridbindingLazyLoading` | CustomStore POST — **call sites commented** |
| `setLookupColumnWidths` | Auto width for lookup columns |

`ati_dxHelper.createDataGridLazyLoad` also builds a CustomStore grid (not the main voucher pattern).

### 4.4 Columns and lookups

Journal example (`empr_JournalVoucher.js`): Action template; `custoM_ACT_CODE` with

```javascript
lookup: {
  dataSource: { store: Accounts, paginate: true, pageSize: 50 },
  displayExpr: 'value',
  valueExpr: 'key',
  searchEnabled: true
},
setCellValue: function (newData, value, currentRowData) { ... }
```

Purchase Bill: lookups for item, color, size, grade; warehouse via `editorType: "dxDropDownBox"` with nested `dxDataGrid` `selection.mode: "single"` and `onSelectionChanged`.

Date columns commonly: `dataType: 'date', format: 'dd-MM-yyyy'` without a standalone DateBox widget.

### 4.5 Filtering, sorting, grouping, paging

On **search grids**: filter row, header filter, search panel, group panel, pager (page size 10, allowed sizes 100/200/300/all). Client-side.

On **line grids**: paging off, filter row off, search panel on, grouping panel on.

Sorting: default DevExtreme column sorting (`allowSorting` not globally disabled in the voucher helper).

### 4.6 Editing and selection

- Search grids: generally not `editing.mode`; edit by loading a row (`GetByCode`) into the form.
- Line grids: **batch** editing.
- Selection: multiple on lists; single on pick dialogs (`"single"` argument).

### 4.7 Events and templates

| Event | Typical use |
|-------|-------------|
| `onToolbarPreparing` | Refresh button |
| `onContentReady` | Column chooser |
| `onCellPrepared` | Dates / styling |
| `onRowPrepared` | Journal: red row for cost-center/knock-off status |
| `onCellClick` | Line grid navigation |
| `onSelectionChanged` | Nested DropDownBox grid |
| `onExporting` | File name |
| `cellTemplate` | Action icons, images, checkboxes (POS return) |
| `onValueChanged` | SelectBox / DateBox editors |

### 4.8 Custom buttons

- Header: `#BtnSave`, `#BtnDelete`, `#BtnNew`, `#BtnQuickSearch` are **Bootstrap**, not `dxButton`.
- Grid toolbar: refresh `dxButton` inside `dxGridbindingVouchers`.
- Line Action column: HTML clone/add/delete.

---

## 5. dxSelectBox

Inline pattern (ItemMaster, Journal, POS, Login Details):

```javascript
$('#Currency').dxSelectBox({
  dataSource: data.data,
  displayExpr: 'value',
  valueExpr: 'key',
  searchEnabled: true,
  width: '100%',
  placeholder: 'Search',
  showClearButton: true,
  pagingEnabled: true,
  searchTimeout: 500,
  onValueChanged: function (e) { ... }
});
```

Wrapper: `ati_dxHelper.createDropdownSingle` / `empr_helper.bindSingleSelectDropDown`.

Static lists exist in `empr_helper` (`priority`, `gender`, `commType`, …) and in modules (`InitBarcodeTypeDDL`: Manual/Auto).

Read/set: `$('#ASTATUS').dxSelectBox('instance').option('value', ...)`.

---

## 6. dxDateBox

Only found as grid `editorType` in `empr_DeliveryOrder.js`:

- `type: 'time'`
- `displayFormat: 'hh:mm a'`
- `interval: 15` or `pickerType: 'rollers'`
- `useMaskBehavior: true`
- `showDropDownButton: false`

Standalone page-level DateBox widgets: **Not found / Not identified**.

---

## 7. dxPopup / dxForm

- **dxPopup:** not used. Dialogs are Bootstrap modals.
- **dxForm:** commented samples only.

---

## 8. dxDropDownBox and dxTreeList

`ati_dxHelper.DxGridBoxDropdown` / `MultipleDxGridBoxDropdown`: Select-like control whose drop-down is a `dxDataGrid`.

`ati_dxHelper.createTreeList`: account/item/role trees.

---

## 9. POS (`empr_POSTransaction.js`)

Same layout DevExtreme stack. SelectBoxes for bank, party, salesman, expense, report type.

Grids:

| Container | Helper | Role |
|-----------|--------|------|
| `#detailContainer` | Calls `empr_POSTransaction.editableDxGridbindingForTransactions` | Lines |
| `#gridContainer` | `dxGridbindingVouchers` single | Quick search |
| `#CustomergridContainer` | `dxGridbindingVouchers` | Customer |
| `#ExpgridContainer` | `dxGridbindingVouchers` | Expense |
| `#BarcodePickGridContainer` | `editableDxGridbinding` | Barcode pick |

The method `editableDxGridbindingForTransactions` is implemented on `empr_POSTransaction` in `empr_POSTransaction_old.js`. Other modules call `empr_helper.editableDxGridbindingForTransactions`. POS summaries use `getTotalSummaryValue('AmountTotal')` / `'NetTotal'` in the old helper.

---

## 10. Reusable Patterns (summary)

1. Empty `div` + JS init, not Razor DevExtreme helpers.
2. `{ key, value }` lookups.
3. GET whole list, bind locally; POST whole document on save.
4. Two grid hosts: `#gridContainer` (search) and `#DetailContainer` (lines).
5. Batch edit on lines; custom row buttons.
6. Bootstrap modals wrapping grids.
7. Toastr for messages, not `DevExpress.ui.notify`.
8. Export on search grids; disabled on voucher line grids.
9. localStorage state storing keyed by module `fileName`.

---

## 11. Important JavaScript Functions

See `FRONTEND.md` for the full `empr_helper` / `ati_dxHelper` list. Grid-specific names to keep stable:

- `empr_helper.dxGridbindingVouchers`
- `empr_helper.editableDxGridbindingForTransactionsVouchers`
- `empr_helper.MasterDetailDxGridBinding`
- `empr_helper.setLookupColumnWidths`
- `empr_helper.MoveFocusToGrid` / `EnableShortCutKeys`
- `ati_dxHelper.createDropdownSingle`
- `ati_dxHelper.DxGridBoxDropdown`
- `ajaxHelper.ajaxGetJson` / `ajaxPostJsonData`

---

## 12. Not Found / Not identified

- ASP.NET Core DevExtreme MVC helpers (`@(Html.DevExtreme()...)`)
- `dxPopup`, active `dxForm`, PivotGrid, Charts in Customjs
- Remote CRUD CustomStore on live voucher screens
- `DevExpress.ui.notify`
