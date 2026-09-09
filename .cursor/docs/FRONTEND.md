# Frontend

This document describes the frontend of Empire ERP as implemented in Razor views, layout, CSS, and JavaScript. Only patterns found in the project are documented.

---

## 1. Frontend Structure

| Location | Role |
|----------|------|
| `Empire_ERP/Views/` | Razor views (one folder per module in most cases) |
| `Empire_ERP/Views/Shared/` | Layout, error page, validation scripts partial |
| `Empire_ERP/wwwroot/css/` | Application and vendor CSS |
| `Empire_ERP/wwwroot/js/` | jQuery, Bootstrap, DevExtreme, theme scripts |
| `Empire_ERP/wwwroot/js/Customjs/` | Module scripts (`empr_*.js`) |
| `Empire_ERP/wwwroot/Dx/` | DevExtreme helpers (`ajaxhelper.js`, `ati_dxHelper.js`) and extra DX assets |
| `Empire_ERP/wwwroot/lib/` | jQuery validation, unobtrusive AJAX, Bootstrap dist, toast-notification |

There is **no** `package.json` or `libman.json` in the project. Frontend libraries are stored under `wwwroot`.

React, Angular, Vue, Tailwind, and Material UI are **not used**.

---

## 2. Views

`Views/_ViewStart.cshtml` sets `Layout = "_Layout"` for all views unless a view overrides it.

`Views/_ViewImports.cshtml`:

```
@using Empire_ERP
@using Empire_ERP.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

Most modules expose a single `Index.cshtml`. Additional views exist for login, POS receipts, and a few special screens.

### View folders (modules)

AccountingReports, Approval, AttendanceMachine, BankDetail, BarcodePrint, BatchIssue, BillOfMaterial, Binaries, Branch, CashBookVoucher, CashReceiptVoucher, ChartOfAccount, ClosingShop, CommMap, Company, CostCenter, Currency, CustomerPricing, DailyProduction, DatabaseBackup, DeleteAttendance, DeliveryFeeding, DeliveryFormat, DeliveryOrder, DocumentRetrieval, EmpLeaves, Employee, EmpMasterInfo, EmpPenalty, EmpTransferEntry, FamilyMember, FDeliveryFeeding, FSodaBookFeeding, GatePass, Hawla, Home, HRCandidate, HRInterviewFeedback, HRInterviewSchedule, HRJobPost, HRMaster, HRMasterTable, HROfferLetter, HRSetup, ImportGeneralManifest, ImportManifest, ImportPermit, ImportReport, ItemGroups, ItemMaster, ItemOpening, JournalVoucher, KnockOff, Login, LotRegistration, MachineInfo, MailBox, MaterialRequisition, MembershipCard, MenuDetails, MerchantPurchaseOrder, MerchantPurchaseOrderDetail, MpoLayout, MPORegistration, MpoTrims, Notes, OpeningBalance, PartyOpeningBalance, PartyReceiptVoucher, PartyReports, PartyToParty, PartyTypes, PendingToSRB, Period, POSDiscount, POSDiscountItemWise, POSMapping, POSTransactions, POSUserRights, PurchaseBill, PurchaseBillReports, PurchaseBookVoucher, PurchaseOrder, PurchaseRequisition, PurchaseSaleFormat, Region, ReportType, Role, SalesContract, SalesInvoiceReport, SalesQutation, SaleTaxInvoice, SetupSubType, SetupType, Shared, Shipment, SodaBookFeeding, SPartyReports, StockAdjustment, StockReceive, StockTransfer, StockTransferRequisition, Table, TexSalesInvoice, User, Waiter, Warehouse, WeighBridge, WorkOrder.

Root files: `_ViewStart.cshtml`, `_ViewImports.cshtml`.

---

## 3. Layouts

### Main layout — `Views/Shared/_Layout.cshtml`

Used by almost all authenticated screens.

**Head**

- Viewport: `width=device-width,initial-scale=1`
- CSS: Bootstrap, Poppins, Font Awesome, Themify, Icofont, Toastr, SweetAlert2, DataTables, Select2, DevExtreme (`dx.common.css`, `dx.generic.custom-scheme.css`), `main.css`, `mtstyles.css`, `tautocomplete.css`, `style.css`, `responsive.css`, DatePicker
- Theme CSS variables (`:root`) from `ViewBag.ThemeColors`: `--theme-blue`, `--theme-red`, `--theme-green`, `--theme-white`, and related tokens
- CDN: html2canvas 1.4.1, qz-tray, croppie 2.6.5

**Shell**

- `#Loader` / `.loader-box`
- `.headertopbar`: logo, `#erpNavToggle`, `#erpSidebarToggle`, notifications/search/user dropdown, `#erpMainNav` (`@Html.Raw(ViewBag.Menu)`)
- `.main_wrap` / `.leftsidebar` (Quick Links / favorites) / `.rightsidecontent`
- `.erp-page-toolbar`: breadcrumb (`ViewBag.BreadCrumbHTML`), gear settings, branch slider
- `#gearDropdown`: Row Limit, Data Clear, Search With Detail
- `@RenderBody()`

**Layout sections**

- `LinkStyleSection`
- `InLineStyleSection`
- `LinkScriptSection` — module JS is typically loaded here
- `InLineScriptSection`

**Scripts (bottom of layout, order matters)**

jQuery 3.7.1, jQuery UI, Toastr, jquery.validate, unobtrusive validation, unobtrusive AJAX, perfect-scrollbar, SweetAlert2, `main.js`, parsley, input-mask, Bootstrap bundle, Select2, DataTables, tautocomplete, babel-polyfill / exceljs / FileSaver (CDN), `custom.js`, `favoriteMenu.js`, `base.js`, `dx.all.js`, `~/dx/ajaxhelper.js`, `~/dx/ati_dxHelper.js`, `empr_helper.js`, `empr_ClosingShop.js`, jquery.mapkey, extra toastr from `lib/toast-notification`.

`site.js` and `print.js` exist under `wwwroot/js` but are **not** included in `_Layout.cshtml`.

**Layout-only JS functions**

- `ShowImage(imageSrc)` — Bootstrap `#ImageViewModal`
- `StopIIS()` — SweetAlert confirm → `/Home/StopApplication`
- `Logout()` — SweetAlert confirm → `/login/logout`
- Mobile nav/sidebar IIFE; breakpoint `max-width: 1024px`

### Standalone layouts (`Layout = null`)

- `Views/Login/Index.cshtml` — login
- `Views/Login/Details.cshtml` — company / branch / period selection

These pages include their own CSS/JS (including DevExtreme).

---

## 4. Partial Views

| File | Role |
|------|------|
| `Views/Shared/_ValidationScriptsPartial.cshtml` | jquery.validate + unobtrusive |
| `Views/Shared/_Layout.cshtml` | Main layout |
| `Views/KnockOff/_KnockOffPartial.cshtml` | Knock-off UI fragment |
| `Views/CostCenter/_CostCenterPartial.cshtml` | Cost center UI fragment |

No large set of shared form partials was found. Most screens are self-contained `Index.cshtml` files.

---

## 5. HTML Conventions

Typical transaction screen (`PurchaseBill`, `JournalVoucher`, and similar):

```
.card
  .card-header
    #BtnQuickSearch.btn-light.btn-search  [data-bs-toggle="modal"]
    #BtnNew.btn-primary.btn-new.float-end
  .card-body.Record
    .row → hidden inputs + .form-label + .form-control or DevExtreme host divs
    ul.nav.nav-tabs.nav-primary
    .tab-content → #DetailContainer (line grid)
  #BtnDelete.btn-danger.btn-delete
  #BtnSave.btn-primary.btn-save
  .modal.fade.bs-example-modal-xl → #gridContainer (Quick Search)
@section LinkScriptSection → empr_{Module}.js + InitEvents()
```

Typical master screen (`Company`):

- Same card header/body
- List grid `#gridContainer` on the same page (not always in a modal)

**Common IDs**

| ID | Use |
|----|-----|
| `#BtnSave` | Save |
| `#BtnDelete` | Delete (often hidden until a record is loaded) |
| `#BtnNew` | New record |
| `#BtnQuickSearch` | Open search modal |
| `#BtnPrint` | Print |
| `#BtnApply` | Gear settings apply |
| `#Code` / `#ID` | Record key |
| `#V_DATE` | Voucher date |
| `#VOUCHER_NO` | Voucher number |
| `#ASTATUS` | Active status (often dxSelectBox) |
| `#gridContainer` | Search / list grid |
| `#DetailContainer` / `#detailContainer` | Line-item grid |
| `#Loader` | Page loader |

**Button classes:** `btn-save`, `btn-delete`, `btn-new`, `btn-search`, `btn-print`, `waves-effect`, `waves-light`.

There is **no consistent `<form id="...">` wrapper**. Fields sit in `.card-body.Record`. JS builds the payload in `GetDataToSave()`.

DevExtreme widgets are hosted in empty `div` elements named after the field (`#PARTY_CODE`, `#Currency`, `#GROUP_CODE`).

---

## 6. CSS Structure

| File | Role |
|------|------|
| `wwwroot/css/bootstrap.min.css` | Bootstrap **v5.1.3** |
| `wwwroot/css/style.css` | ERP chrome: header, sidebar, login, toast, cards, DevExtreme drag icon |
| `wwwroot/css/main.css` | Large theme file including DevExtreme selection colors, branch slider |
| `wwwroot/css/mtstyles.css` | Pixel utility classes (`.p-5`, `.pt-5`, `.ml-*`, `.fs-*`) |
| `wwwroot/css/responsive.css` | Shell breakpoints and ERP toggle/overlay classes |
| `wwwroot/css/posstyle.css` | POS-specific layout (loaded by POS views) |
| `wwwroot/css/dx.common.css` | DevExtreme common (file header version **19.2.7**) |
| `wwwroot/css/dx.generic.custom-scheme.css` | Custom DevExtreme theme (19.2.7 ThemeBuilder) |
| `wwwroot/css/site.css` | Default ASP.NET scaffold styles; **not linked** in `_Layout.cshtml` |
| `wwwroot/css/vendors/toastr.css` | Toastr |
| `wwwroot/css/vendors/sweetalert2.min.css` | SweetAlert2 |
| `wwwroot/Views/Shared/_Layout.cshtml.css` | Scoped layout CSS |

Icon fonts: Font Awesome, Themify, Icofont, Material Icons (files present).

---

## 7. JavaScript Structure

### Global / shared

| File | Role |
|------|------|
| `wwwroot/js/jquery-3.7.1.min.js` | jQuery **3.7.1** |
| `wwwroot/js/bootstrap.bundle.min.js` | Bootstrap JS |
| `wwwroot/js/dx.all.js` | DevExtreme bundle (**23.1.6** in file header) |
| `wwwroot/Dx/ajaxhelper.js` | `ajaxHelper.ajaxPostJsonData`, `ajaxHelper.ajaxGetJson` |
| `wwwroot/Dx/ati_dxHelper.js` | SelectBox / DropDownBox / TreeList helpers |
| `wwwroot/js/Customjs/empr_helper.js` | Grid binders, `notify`, lookups, keyboard helpers |
| `wwwroot/js/base.js` | `empr_Base` — gear settings (`/Base/GetSettings`) |
| `wwwroot/js/favoriteMenu.js` | Favorite menu drag/drop, F-keys, `/FavoriteMenu/*` |
| `wwwroot/js/custom.js` | Chart/dashboard DevExtreme config |
| `wwwroot/js/toastr.min.js` | Toasts |
| `wwwroot/js/sweetalert2.min.js` + `sweetalert.script.min.js` | Confirm dialogs (`swal`) |
| `wwwroot/js/site.js` | `ChildMenu(Id)` AJAX to `/Base/GetChildRecord` — **not in layout** |
| `wwwroot/js/print.js` | `window.print()` — **not in layout** |

### Module scripts (`wwwroot/js/Customjs/`)

One `empr_{Module}.js` per screen (111 files). Typical object:

```javascript
var empr_Module = {
  InitEvents: function () { /* bind #BtnSave, #BtnDelete, .elm_edit */ },
  validateForm: function () { empr_helper.notify(...); return valid; },
  GetDataToSave: function () { /* form + grid dataSource */ },
  Save: function () {
    ajaxHelper.ajaxPostJsonData(model, "/Controller/Action", function (data) {
      empr_helper.notify(data.msg, data.msgType);
    }, false, true);
  },
  CreateGrid: function (dataSrc) {
    empr_helper.dxGridbindingVouchers('#gridContainer', col, dataSrc, "Key");
  }
};
$(function () { empr_Module.InitEvents(); });
```

The full Customjs file list is in section 18 below.

---

## 8. jQuery Usage

jQuery is the primary DOM and AJAX library.

Verified uses:

- `$('#BtnSave').click(...)`
- `$('#gridContainer').dxDataGrid(...)` / `$('#x').dxSelectBox(...)` (DevExtreme jQuery API)
- `$('#modal').modal('show')` / `.modal('hide')` (Bootstrap 5 still used this way in module JS)
- `$(function () { ... })` for init
- Occasional raw `$.ajax` for file upload (`FormData`)

---

## 9. AJAX Patterns

Primary wrapper: `ajaxHelper` (`wwwroot/Dx/ajaxhelper.js`).

| Helper | Typical use |
|--------|-------------|
| `ajaxHelper.ajaxPostJsonData(model, url, callback, ...)` | Save, delete, update |
| `ajaxHelper.ajaxGetJson(url, callback, ...)` | QuickSearch, GetByCode, dropdowns |

Server response shape used by UI:

```
{ msgType, msg, data, voucherNo, ... }
```

`msgType == 1` is treated as success in module JS.

Common endpoints:

- `GET /{Controller}/QuickSearch`
- `GET /{Controller}/Get{Entity}ByCode?code=`
- `POST /{Controller}/save` (action names vary by controller)
- `POST /{Controller}/Delete`
- `POST /{Controller}/GetPrintReport`

Loader: `#Loader` is shown/hidden around many AJAX calls (via helper flags).

---

## 10. Bootstrap Usage

- CSS: `~/css/bootstrap.min.css` — **Bootstrap v5.1.3**
- JS: `~/js/bootstrap.bundle.min.js`
- Attributes: `data-bs-toggle`, `data-bs-target`, `data-bs-dismiss`, `btn-close`
- Grid: `container-fluid`, `row`, `col-xl-*`, `col-12`, `col-lg-*`
- Components: `card`, `nav-tabs`, `modal fade`, `dropdown-menu`, `form-check`, `breadcrumb`

---

## 11. Forms

- Not MVC `Html.BeginForm` for main CRUD (AJAX JSON instead).
- Labels: `.form-label`
- Inputs: `.form-control`
- Hidden fields for keys and DevExtreme selected values (`#_hidden` on login details)
- Validation is **custom JS** (`validateForm` / `ValidateMainInfo`) plus HTML `required` on login fields
- `jquery.validate`, unobtrusive validation, and Parsley are **loaded** but Customjs does not call `$().validate()` or Parsley APIs (no matches found)

---

## 12. Modals

**Bootstrap 5 modals are the project standard.** `dxPopup` is **Not found / Not identified**.

Layout-level modals include:

- `#cropImagePop`, `#ImageViewModal`
- Copy dialogs: `#CopyViewModal`, `#CopySalesMan`, `#CustomerPricingCopy`, `#CopyViewModal_LR`, `#CopyViewModal_MPOR`, `#CopyViewModalName`, `#CopyViewModalBOM`
- `#favoriteModal`

Module-level: Quick Search (`.bs-example-modal-xl`), print (`#ShowReportModal`), pick dialogs (`#SodaPickModal`, `#BarcodePickModal`), login OTP/password (`#exampleModal`, `#NewModal`).

---

## 13. Validation

| Mechanism | Status |
|-----------|--------|
| Custom JS `validateForm` + `empr_helper.notify` | **Primary** |
| HTML `required` | Login fields |
| jquery.validate / unobtrusive | Scripts loaded; not used as the main CRUD pattern |
| Parsley | Script loaded; **not found** in Customjs usage |
| DevExtreme `dxValidator` | **Not found / Not identified** |

---

## 14. Notifications

### Toastr via `empr_helper.notify(msg, type)`

| type | Toastr |
|------|--------|
| 1 | `toastr.success` — Success |
| 2 | `toastr.error` — Error |
| 3 | `toastr.warning` — Warning |
| 4 | `toastr.info` — Info |

### SweetAlert

`swal({ title, text, type: 'warning', showCancelButton: true, ... }).then(...)` for delete, copy, logout, Stop IIS.

`DevExpress.ui.notify` is **Not found / Not identified**.

The header notifications dropdown (`#notifications`) in the layout contains placeholder markup and is not documented here as a live backend feed.

---

## 15. Common UI Components

| Component | Implementation |
|-----------|----------------|
| Data grid | DevExtreme `dxDataGrid` via `empr_helper` |
| Dropdown | DevExtreme `dxSelectBox` / `dxDropDownBox` |
| Tree | DevExtreme `dxTreeList` (Chart of Account, Item Groups, Role, etc.) |
| Date | Often `<input>` / date picker CSS; `dxDateBox` as grid `editorType` in Delivery Order |
| Tabs | Bootstrap `nav-tabs` |
| Modal | Bootstrap modal |
| Toast | Toastr |
| Confirm | SweetAlert `swal` |
| Image crop | Croppie (`empr_Cropper.js`, `#cropImagePop`) |
| Favorites | Drag from menu to sidebar (`favoriteMenu.js`) |
| Loader | `#Loader` |
| Select2 | CSS/JS loaded globally; DevExtreme is the primary dropdown for ERP screens |

---

## 16. Responsive Implementation

Viewport meta is set in `_Layout.cshtml` and login pages.

`responsive.css` breakpoints (verified):

- `min-width: 1200px`
- `max-width: 1199.98px`
- `max-width: 1024px` — off-canvas sidebar and collapsible nav
- `max-width: 991.98px`
- `max-width: 767.98px`
- `max-width: 575.98px`
- `max-width: 374.98px`

Layout JS uses `window.matchMedia('(max-width: 1024px)')`.

Classes: `.erp-nav-toggle`, `.erp-sidebar-toggle`, `.erp-sidebar-overlay`, `.erp-main-nav`, `.erp-page-toolbar`, `.ph-hide`, `.collapse_sidebar`.

POS uses `posstyle.css` in addition to the main shell.

---

## 17. Existing Frontend Conventions

1. Module JS object named `empr_{Module}` with `InitEvents`.
2. Save goes through `ajaxHelper` + `empr_helper.notify`.
3. Search grids use `#gridContainer`; line grids use `#DetailContainer`.
4. Lookups use `{ key, value }` objects (`valueExpr: 'key'`, `displayExpr: 'value'`).
5. Permissions from `ViewBag.Permissions` hide/disable buttons in JS (pattern varies by module).
6. Gear menu (`#gearBtn`) stores per-menu Search / Row Limit / Data Clear via `/Base/GetSettings` and `/Base/UpdateSettings`.
7. Menu HTML is server-rendered in `BaseController`, not built in the client.

---

## 18. Important Shared JavaScript Functions

### `empr_helper.js`

- `notify(msg, type)`
- `dxGridbinding`, `dxGridbindingVouchers`, `dxGridbindingKnockOff`, `dxGridbindingForMultiBillPrint`, `dxGridbindingVouchersForApproval`, `dxGridbindingWithoutFeatures`, `dxGridbindingForReports`, `dxGridbindingLazyLoading` (implemented; call sites commented)
- `editableDxGridbinding`, `editableDxGridbindingForTransactions`, `editableDxGridbindingForTransactionsVouchers`, `editableDxGridbindingForPurchaseSale`
- `dxGridbindingForStockReceive`, `dxGridBindingWithSearch`, `MasterDetailDxGridBinding`
- `setLookupColumnWidths`, `MoveFocusToGrid`, `EnableShortCutKeys`
- Static lists: `conditions`, `priority`, `gender`, `maritalStatus`, `SBF_TYPE`, `hrRecommendation`, `hrStatus`, `commType`

### `ati_dxHelper.js`

- `createDropdownSingle` / variants
- `DxGridBoxDropdown` / `MultipleDxGridBoxDropdown`
- `createTreeList`, `createDataGridLazyLoad`

### `empr_Base` (`base.js`)

- `Init()`, `GetSettings($btn)`

### `favoriteMenu.js`

- `loadFavorites`, `AddFavorite` / `DeleteFavorite` endpoints, F-key shortcuts, sidebar state

---

## 19. Important CSS Files (project-specific)

`style.css`, `main.css`, `mtstyles.css`, `responsive.css`, `posstyle.css`, `dx.generic.custom-scheme.css`.

---

## 20. Existing UI Patterns

| Pattern | Where |
|---------|-------|
| Card + Quick Search modal + Save/Delete | Most transaction screens |
| Same-page list grid | Some masters (Company) |
| Bootstrap tabs for master sections | ItemMaster (Main / Attributes / Barcode) |
| Line grid with clone/add/delete cell templates | Journal, Purchase Bill, POS |
| Pick modal grids | Purchase Bill soda/barcode pick |
| Standalone login + details | Login |
| POS full-page UI | `Views/POSTransactions/` + `posstyle.css` |
| Sales quotation POS-style UI | `Views/SalesQutation/Index.cshtml` + `empr_SalesQutation.js` (group sidebar, clickable item cards, JS detail list, save modal) |

### POS views

`Index.cshtml`, `PointOfSale.cshtml`, `PointOfSale_2.cshtml`, `SaleReceipt.cshtml`, `ExpenseReceipt.cshtml`, `GTEX.cshtml`, `C20.cshtml`, `POS_KOT.cshtml`.

Related JS: `empr_POSTransaction.js`, `empr_POSTransaction_old.js`.

POS receipts in this area are Razor HTML prints, not RDLC (see `RDLC_REPORTS.md`).

---

## 21. Customjs File List

`empr_AccountingReports.js`, `empr_AccountOpening.js`, `empr_Approval.js`, `empr_AttendanceMachine.js`, `empr_BankDetail.js`, `empr_BarcodePrint.js`, `empr_BatchIssue.js`, `empr_BillOfMaterial.js`, `empr_Binaries.js`, `empr_Branch.js`, `empr_CashBookVoucher.js`, `empr_CashReceiptVoucher.js`, `empr_ChartOfAccount.js`, `empr_ClosingShop.js`, `empr_CommMap.js`, `empr_Company.js`, `empr_CostCenter.js`, `empr_Cropper.js`, `empr_Currency.js`, `empr_CustomerPricing.js`, `empr_DailyProduction.js`, `empr_DatabaseBackup.js`, `empr_DeleteAttendance.js`, `empr_DeliveryFeeding.js`, `empr_DeliveryFormat.js`, `empr_DeliveryOrder.js`, `empr_DocumentRetrieval.js`, `empr_EmpLeaves.js`, `empr_Employee.js`, `empr_EmpMasterInfo.js`, `empr_EmpPenalty.js`, `empr_EmpTransferEntry.js`, `empr_FamilyMember.js`, `empr_FDeliveryFeeding.js`, `empr_FSodaBookFeeding.js`, `empr_GatePass.js`, `empr_Hawla.js`, `empr_helper.js`, `empr_HRCandidate.js`, `empr_HRInterviewFeedback.js`, `empr_HRInterviewSchedule.js`, `empr_HRJobPost.js`, `empr_HRMaster.js`, `empr_HrMasterTable.js`, `empr_HROfferLetter.js`, `empr_HRSetup.js`, `empr_ImportGeneralManifest.js`, `empr_ImportManifest.js`, `empr_ImportPermit.js`, `empr_ImportReports.js`, `empr_ItemGroups.js`, `empr_ItemMaster.js`, `empr_ItemOpening.js`, `empr_JournalVoucher.js`, `empr_KnockOff.js`, `empr_Login.js`, `empr_LoginDetails.js`, `empr_LotRegistration.js`, `empr_MachineInfo.js`, `empr_MailBox.js`, `empr_MaterialRequisition.js`, `empr_MembershipCard.js`, `empr_MenuDetails.js`, `empr_MerchantPurchaseOrder.js`, `empr_MerchantPurchaseOrderDetail.js`, `empr_MpoLayout.js`, `empr_MPORegistration.js`, `empr_MpoTrims.js`, `empr_Notes.js`, `empr_PartyOpening.js`, `empr_PartyReceiptVoucher.js`, `empr_PartyReports.js`, `empr_PartyToParty.js`, `empr_PartyTypes.js`, `empr_PendingToSRB.js`, `empr_Period.js`, `empr_POSDiscount.js`, `empr_POSDiscountItemWise.js`, `empr_POSMapping.js`, `empr_POSTransaction.js`, `empr_POSTransaction_old.js`, `empr_PosUserRights.js`, `empr_PurchaseBill.js`, `empr_PurchaseBillReports.js`, `empr_PurchaseBookVoucher.js`, `empr_PurchaseOrder.js`, `empr_PurchaseRequisition.js`, `empr_PurchaseSaleFormat.js`, `empr_Region.js`, `empr_ReportType.js`, `empr_Role.js`, `empr_SalesContract.js`, `empr_SalesInvoiceReport.js`, `empr_SalesQutation.js`, `empr_SaleTaxInvoice.js`, `empr_SetupSubType.js`, `empr_SetupType.js`, `empr_Shipment.js`, `empr_SodaBookFeeding.js`, `empr_SPartyReports.js`, `empr_StockAdjustment.js`, `empr_StockReceive.js`, `empr_StockTransfer.js`, `empr_StockTransferRequisition.js`, `empr_Table.js`, `empr_TexSalesInvoice.js`, `empr_TexSalesInvoice .js` (duplicate filename with a trailing space), `empr_User.js`, `empr_Waiter.js`, `empr_Warehouse.js`, `empr_WeighBridge.js`, `empr_WorkOrder.js`.

---

## 22. Not Found / Not Identified

- SPA framework (React/Angular/Vue)
- `dxPopup`, `dxForm` (only commented), `dxNumberBox`, `dxTextBox`, `dxTagBox`, `dxLoadPanel`, `dxChart` in Customjs
- Bundler (Webpack/Vite) / npm scripts
- `site.css` usage in the live layout
- Server-side unobtrusive validation as the main CRUD mechanism
