# Architecture

This document describes the architecture of Empire ERP as implemented in the repository. Information is taken from project files, classes, and configuration. Items that could not be verified in code are marked **Not found / Not identified**.

---

## 1. Project Overview

Empire ERP is an ASP.NET Core 7.0 MVC web application for ERP operations. The solution is named `Empire_ERP.sln` and contains three projects:

| Project | Type | Target | Role |
|---------|------|--------|------|
| `Empire_ERP` | `Microsoft.NET.Sdk.Web` | `net7.0` | Presentation (MVC controllers, Razor views, static files, RDLC reports) |
| `Empire_ERP.Core` | `Microsoft.NET.Sdk` | `net7.0` | Domain entities, service interfaces, service implementations, SQL connection helper |
| `Empire_ERP.Infrastructure` | `Microsoft.NET.Sdk` | `net7.0` | Repository implementations (ADO.NET / SQL Server) |

The default MVC route is:

```
{controller=Login}/{action=Index}/{id?}
```

Configured in `Empire_ERP/Program.cs`. Unauthenticated users land on the login screen.

---

## 2. Technology Stack

Verified from `.csproj` files, `Program.cs`, views, and `wwwroot`:

| Area | Technology | Evidence |
|------|------------|----------|
| Framework | ASP.NET Core 7.0 MVC | `TargetFramework` = `net7.0`; `AddControllersWithViews()` |
| Language | C# | All application projects |
| Architecture style | Three-project Clean Architecture (with noted deviations) | Core / Infrastructure / Web |
| Database | SQL Server | `Microsoft.Data.SqlClient` 5.1.5; connection in `appsettings.json` |
| ORM | **Not found / Not identified** | No `DbContext`, no Entity Framework packages |
| Data access | ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataReader`, `SqlHelper`) | Repositories + `SQLService.cs` |
| JSON | Newtonsoft.Json via `Microsoft.AspNetCore.Mvc.NewtonsoftJson` 7.0.15 | `ServiceRegistrationHelper` |
| Frontend | Razor, HTML, CSS, JavaScript, jQuery 3.7.1, Bootstrap 5.1.3, DevExtreme | `_Layout.cshtml`, `wwwroot` |
| Grids | DevExtreme `dxDataGrid` (JS 23.1.6) | `wwwroot/js/dx.all.js` |
| Reports | RDLC via `ReportViewerCore.NETCore` 15.1.21 (`Microsoft.Reporting.NETCore.LocalReport`) | Controllers under `Empire_ERP/Controllers` |
| PDF (non-RDLC) | iTextSharp, iText 7, DinkToPdfAll, EvoPdf | Web and Core packages |
| Email | MailKit 4.12.1 | Web project package |
| Barcodes / QR | ZXing.Net, SkiaSharp | Core and Infrastructure |
| Real-time | SignalR (`OrderHub` at `/orderHub`) | `Program.cs`, `Helpers/OrderHub.cs` |
| Authentication | Session-based (no ASP.NET Identity, no JWT) | `LoginController`, `CheckSession` |
| Session | Cookie session, idle timeout 365 days | `ServiceRegistrationHelper` |

---

## 3. .NET Version

- **Target framework:** `net7.0` on all three projects.
- **Nullable:** enabled.
- **Implicit usings:** enabled.
- **Startup model:** minimal hosting (`Program.cs` only). `Startup.cs` is **Not found / Not identified**.

---

## 4. Clean Architecture Implementation

The solution follows a three-layer split:

```
Empire_ERP (Web / Presentation)
    → references Empire_ERP.Core
    → references Empire_ERP.Infrastructure

Empire_ERP.Infrastructure (Data)
    → references Empire_ERP.Core

Empire_ERP.Core (Domain / Application)
    → no project references to Web or Infrastructure
```

### Intended dependency flow

```
HTTP Request
    → Controller (Empire_ERP)
        → I*Service (Empire_ERP.Core.Interfaces)
            → *Service (Empire_ERP.Core.Services)
                → I*Repository (Empire_ERP.Core.Interfaces)
                    → *Repository (Empire_ERP.Infrastructure.Repositories)
                        → SQL Server via SQLService.getconnstring() + ADO.NET
```

### Observed deviations (from actual code)

These exist in the current codebase and should be treated as facts, not as recommended design:

1. **`SQLService` lives in Core** (`Empire_ERP.Core/Services/SQLService.cs`) and reads `appsettings.json` directly. Repositories typically call `new SQLService().getconnstring()` rather than injecting a connection.
2. **Some SQL is executed from the Web layer** (for example `BaseController.isSidebarOpens()` and `CommonHelper.GetPermissionByMenueID`).
3. **There is no separate Application project.** Services and interfaces sit in Core alongside entities.
4. **ViewModels folder is not present.** Screen/DTO types live in `Empire_ERP.Core/Entities` (including names such as `ClosingViewModal`, `SlipViewModel`).
5. **Core contains data-access helpers** (`SqlHelper` inside `SQLService.cs`) that are typically an infrastructure concern.

---

## 5. Project / Layer Structure

### 5.1 `Empire_ERP` (Presentation)

| Folder | Contents |
|--------|----------|
| `Controllers/` | 116 controller files |
| `Views/` | Razor views, one folder per module |
| `Helpers/` | DI registration, session filters, SignalR hub, `CommonHelper` |
| `Models/` | `ErrorViewModel.cs` only |
| `Reports/` | RDLC files and typed dataset (`Reports/Datasets/`) |
| `wwwroot/` | CSS, JS, images, client-generated PDFs |
| `Properties/` | Launch / publish profiles |

**Not found / Not identified:** `Filters/`, `Attributes/`, `Middleware/`, `ViewModels/` folders.

### 5.2 `Empire_ERP.Core`

| Folder | Contents |
|--------|----------|
| `Entities/` | 151 entity / DTO classes |
| `Interfaces/` | `I*Service` and `I*Repository` contracts |
| `Services/` | 114 service classes plus `SQLService`, `CommonService`, `DropdownService`, `PdfService`, `BaseService` |

### 5.3 `Empire_ERP.Infrastructure`

| Folder | Contents |
|--------|----------|
| `Repositories/` | 111 repository classes implementing Core interfaces |

---

## 6. Responsibilities of Each Layer

### Presentation (`Empire_ERP`)

- Receive HTTP requests.
- Enforce session (`[CheckSession]`) and menu code (`[ExtractMenuCode]`).
- Build navigation, breadcrumbs, and theme in `BaseController.OnActionExecuting`.
- Return Razor views or JSON (`JsonResult`) for AJAX.
- Generate RDLC PDFs in many module controllers (`LocalReport.Render("PDF")`).
- Serve static files from `wwwroot`.

### Core (`Empire_ERP.Core`)

- Define entities used across layers.
- Define service and repository interfaces.
- Implement services that usually **delegate** to repositories with little extra logic.
- Provide shared utilities: `CommonService` (encryption, parsing), `DropdownService`, `PdfService` (POS KOT/sticker PDF via iText 7), `SQLService` (connection string).

### Infrastructure (`Empire_ERP.Infrastructure`)

- Execute SQL (inline `CommandType.Text` and some `EXEC` stored procedures).
- Map `SqlDataReader` results to entities / anonymous objects.
- Run `SqlTransaction` commit/rollback on save methods that use it.
- Soft-delete via `DLT = 'F'` and filter active rows with `DLT = 'T'`.

---

## 7. Controllers

Controllers live in `Empire_ERP/Controllers`. Most inherit `BaseController` and are decorated with `[CheckSession]` and `[ExtractMenuCode]`.

### Base controller

`BaseController` (`Empire_ERP/Controllers/BaseController.cs`):

- Injects `IMenuService` and `IBaseService`.
- On non-AJAX requests: builds HTML menus from `TBL_MENU_BUILDER` (via menu service), breadcrumbs, company/branch/period labels, and theme colors.
- If query string `Code` is present and the user’s menu list does not contain that ID, redirects to `Login/Index`.
- After action: loads approval status into `ViewBag.StatusApp`.
- Exposes `UpdateSettings` and `GetSettings` (menu row-limit / search / data-clear).

### Controller inventory (by functional area)

**Auth / home / shared**

- `LoginController`, `HomeController`, `BaseController`, `CommonController`, `FavoriteMenuController`, `ReportController`

**Setup / masters**

- `CompanyController`, `BranchController`, `PeriodController`, `UserController`, `RoleController`, `MenuDetailsController`, `SetupTypeController`, `SetupSubTypeController`, `CategoryController`, `EntityController`, `RegionController`, `CurrencyController`, `WarehouseController`, `ItemGroupsController`, `ItemMasterController`, `PartyTypesController`, `SalesManController`, `ChartOfAccountController`, `AccountGroupController`, `AccountNatureController`, `CostCenterController`, `BankDetailController`, `DeliveryFormatController`, `PurchaseSaleFormatController`, `ReportTypeController`, `NotesController`, `BinariesController`

**Sales / POS**

- `POSTransactionsController`, `POSMappingController`, `POSDiscountController`, `POSDiscountItemWiseController`, `POSUserRightsController`, `ClosingShopController`, `MembershipCardController`, `TableController`, `WaiterController`, `SaleTaxInvoiceController`, `TexSalesInvoiceController`, `DeliveryOrderController`, `SalesContractController`, `SalesQutationController`, `SalesInvoiceReportController`, `PendingToSRBController`

**Purchase**

- `PurchaseBillController`, `PurchaseOrderController`, `PurchaseRequisitionController`, `PurchaseBookVoucherController`, `PurchaseBillReportsController`, `MerchantPurchaseOrderController`, `MerchantPurchaseOrderDetailController`, `MPORegistrationController`, `MpoLayoutController`, `MpoTrimsController`, `CustomerPricing.cs` (class name `CustomerPricing`, not `*Controller`)

**Inventory / production**

- `StockTransferController`, `StockTransferRequisitionController`, `StockAdjustmentController`, `StockReceiveController`, `ItemOpeningController`, `MaterialRequisitionController`, `BarcodePrintController`, `BatchIssueController`, `BillOfMaterialController`, `WorkOrderController`, `LotRegistrationController`
- `DailyProductionController - Copy.cs` exists. A file named `DailyProductionController.cs` is **Not found / Not identified**.

**Accounting / finance**

- `JournalVoucherController`, `CashReceiptVoucherController`, `CashBookVoucherController`, `PartyReceiptVoucherController`, `PartyToPartyController`, `OpeningBalanceController`, `PartyOpeningBalanceController`, `KnockOffController`, `AccountingReportsController`, `PartyReportsController`, `SPartyReportsController`, `ApprovalController`

**HR**

- `EmployeeController`, `EmpMasterInfoController`, `EmpLeavesController`, `EmpPenaltyController`, `EmpTransferEntryController`, `AttendanceMachineController`, `DeleteAttendanceController`, `FamilyMemberController`, `HRMasterController`, `HRMasterTableController`, `HRSetupController`, `HRJobPostController`, `HRCandidateController`, `HRInterviewScheduleController`, `HRInterviewFeedbackController`, `HROfferLetterController`

**Import / logistics**

- `ImportManifestController`, `ImportGeneralManifestController`, `ImportPermitController`, `ImportReportController`, `ShipmentController`, `HawlaController`, `GatePassController`, `WeighBridgeController`, `SodaBookFeedingController`, `FSodaBookFeedingController`, `DeliveryFeedingController`, `FDeliveryFeedingController`, `CommMapController`

**Other**

- `MachineInfoController`, `DocumentRetrievalController`, `DatabaseBackupController`, `MailBoxController`

A dedicated **Payroll** module (payslip generation, salary processing) is **Not found / Not identified** in active code. Commented payroll RDLC paths exist only in `BarcodePrintController.cs`.

---

## 8. Services

Services live in `Empire_ERP.Core/Services` and implement matching `I*Service` interfaces. Typical pattern:

```csharp
public class XService : IXService
{
    public IXRepository _xRepository { get; set; }
    public XService(IXRepository xRepository) { _xRepository = xRepository; }
    public MyHttpResponseMessage Save(...) => _xRepository.Save(...);
}
```

Most services are thin wrappers. Business SQL and transactions sit in repositories.

### Shared / infrastructure-like services in Core

| Class | Role |
|-------|------|
| `SQLService` | Reads `AppSettings:DefaultConnection` (and `ZKConnection` if present) from `appsettings.json` |
| `SqlHelper` (same file) | Static ADO.NET helpers (`ExecuteDataset`, `ExecuteNonQuery`, etc.) |
| `CommonService` | AES encrypt/decrypt, parsing, URL helpers, timezone (`Pakistan Standard Time`) |
| `DropdownService` | Shared dropdown data |
| `PdfService` | POS KOT / sticker PDFs (iText 7 + ZXing); not RDLC |
| `BaseService` | Menu settings, approval flag, theme colors |
| `MenuService` | Application menu from `TBL_MENU_BUILDER` |

All service/repository pairs are registered as **Scoped** in `Empire_ERP/Helpers/ServiceRegistrationHelper.RegisterServices`.

---

## 9. Repositories

Repositories live in `Empire_ERP.Infrastructure/Repositories`. They:

- Open `SqlConnection` with `new SQLService().getconnstring()`.
- Run parameterized or interpolated SQL (`CommandType.Text`).
- Call stored procedures via `EXEC {name}` (and rarely `CommandType.StoredProcedure`).
- Return `MyHttpResponseMessage` (`msgType`, `msg`, `data`, optional `SaveData`, `voucherNo`).

`msgType` convention observed in controllers/JS: **1 = success**, **2 = error**.

---

## 10. Models and ViewModels

### Entities (`Empire_ERP.Core/Entities`)

151 classes covering masters, vouchers, POS, HR, import, and report DTOs. Examples:

- Context: `Common`, `Base`, `MyHttpResponseMessage`, `Colors`, `Info`, `Menu`
- Masters: `Company`, `Branch`, `Period`, `User`, `Role`, `ItemMaster`, `PartyTypes`, `Warehouse`, `ChartOfAccount`
- Transactions: `PurchaseBill`, `PurchaseBillDetail`, `JournalVoucher`, `POSTransaction`, `POSTransactionDetail`, `StockTransfer`
- Reports: `RDLCReport`, `AccountingReport`, `PartyReport`, `PrintRequest`, `ListPrintReport`
- View-style types in Entities (no separate ViewModels project): `ClosingViewModal`, `SlipViewModel`, `ClosingShopDTOModel`, `SaveClosingRequest`, `POSBookingItemDto`

### MVC Models (`Empire_ERP/Models`)

Only `ErrorViewModel` (`RequestId`, `ShowRequestId`).

### ViewModels folder

**Not found / Not identified.**

---

## 11. Database / Data-Access Layer

- **Engine:** SQL Server.
- **Connection:** `AppSettings:DefaultConnection` in `Empire_ERP/appsettings.json` (read by `SQLService`, not `ConnectionStrings:`).
- **ZK / attendance connection:** `AppSettings:ZKConnection` is referenced in `SQLService.get_zkconnstring()`. The key is commented out in the current `appsettings.json`.
- **No EF migrations.** Schema is assumed to already exist in SQL Server.
- **Table naming:** `TBL_*` (for example `TBL_USER`, `TBL_COMPANY`, `TBL_MENU_BUILDER`, `TBL_POS_MASTER`).
- **Soft delete:** `DLT = 'T'` means active; delete sets `DLT = 'F'`.
- **Active flag:** many masters use `ASTATUS = 'Y'` / `'N'`.
- **Multi-company:** `Company` (session) / `CCODE`.
- **Multi-branch:** `Branch` / `BCODE`.
- **Fiscal period:** `Period` / `PERIOD_ID` (`TBL_PERIOD`).

### Stored procedures referenced in C#

| Procedure | Where used (examples) |
|-----------|------------------------|
| `STKPROC` | Stock balance checks (PurchaseBill, POS, DeliveryOrder, StockTransfer, ItemMaster, others) |
| `PROC_PRINT` | Print data for several document reports |
| `APROC` | Accounting reports |
| `PPROC` | Party reports |
| `APPROVAL` / `APPROVAL_SETUP` | Approval lists |
| `POS_CLOSED` | POS day-end closing |
| `SODA_SUMMARY` / `STK_ORDER_PROC` | S-party / soda reports |
| `KNOCKOFF_PICK` | Knock-off pick |
| `COST_CENTER` | Import reports |
| `SUMMARY_BARCODE` / `SUMMARY_POS` | Sales invoice reports |
| `PROC_PICK_DATA` | Hawla pick |
| `sp_spaceused` | Database backup (`CommandType.StoredProcedure`) |

Full stored-procedure bodies are **Not found / Not identified** in this repository (they live in SQL Server).

---

## 12. Dependency Injection

Registered in `Empire_ERP/Helpers/ServiceRegistrationHelper.RegisterServices`, called from `Program.cs`.

Also configured there:

- `AddControllersWithViews().AddNewtonsoftJson` (ignore reference loops; include nulls)
- `AddSession` (365-day idle timeout, HttpOnly, IsEssential)
- `ForwardedHeaders` (`X-Forwarded-For`, `X-Forwarded-Proto`)
- `FormOptions.ValueCountLimit = 52428800`

In `Program.cs` additionally:

- `AddSignalR()`
- `AddControllersWithViews()` (also called in the helper)
- `FormOptions.MultipartBodyLengthLimit = 262144000` (250 MB)
- `MvcOptions.MaxModelBindingCollectionSize = 10000`

Lifetime: **Scoped** for all `I*Service` / `I*Repository` pairs listed in the helper.

`SQLService` is **not** registered in DI; repositories instantiate it with `new SQLService()`.

---

## 13. Authentication / Authorization

### Authentication

- **Not** ASP.NET Identity, cookies authentication middleware, or JWT.
- `Program.cs` calls `UseSession()` then `UseAuthorization()` but does **not** call `UseAuthentication()`.
- Login: `LoginController.LoginAttempt` → `LoginService.CheckCredentials` → `LoginRepository` queries `TBL_USER` joined to `TBL_ROLE`.
- Password compared after `CommonService.EncryptString` (AES) against `TBL_USER.UPASS`.
- User must have `ASTATUS = 'Y'` and `DLT = 'T'`.
- On success, session keys are set (see table below), then the user selects company/branch/period on `Login/Details`.

### Session keys set at login (verified)

| Key | Source |
|-----|--------|
| `Username` | `User.USERNAME` |
| `Id` | `User.U_ID` |
| `Email` | `User.EMAIL` |
| `Picture` | `User.PICTURES` |
| `Name` | `User.FULLNAME` |
| `RoleId` | `User.ROLEID` |
| `RoleType` | `User.ROLE_TYPE` |
| `Logo` / `Icon` | `TBL_WLABEL` |
| `ShowSelected` | `User.SHOW_SELECTED` |
| `IPAddress` / `ComputerName` | `CommonHelper.SetValues` |
| `Company` / `Branch` / `Period` | `SaveLoginUserDetails` |
| `MenuID` | `ExtractMenuCode` (`Code` query string) |
| `ProjectColors` | `IBaseService.GetColors()` |

### Authorization

- `[CheckSession]`: if `Session["Id"]` is null, redirect to `Login/Index`.
- `[ExtractMenuCode]`: stores `Code` as `MenuID`; if missing (and not coming from `Login/Details`), redirect to `Home/Index`.
- Menu access: `BaseController` checks that `Code` exists in the user’s menu list. Role type `"A"` loads all menus; otherwise menus are filtered by role.
- Fine-grained flags from `TBL_ROLE`: `R_ADD`, `R_EDIT`, `R_DLT`, `R_VIEW`, `R_PRINT`, `R_COPY`, `R_BCODE`, `RMENU_ID`, `MODULE_ID`.
- Controllers commonly set `ViewBag.Permissions` to `"Admin"` when `RoleType == "A"`, else `CommonHelper.GetPermissionByMenueID`.
- Approval rights: `BaseRepository.GetApproval` checks `TBL_ROLE` with `MODULE_ID = 5`.

### Password reset

Open endpoints on `LoginController`: username verification, OTP (`TBL_FPASSWORD`, 120-second expiry in repository logic), password update.

### Remember-me cookie

`CommonService.LoginUserCookie` exists; restore-from-cookie logic in `LoginController.Index` is commented out / unused (`loginCookieValue` forced to empty string).

---

## 14. Important Shared Components

| Component | Location | Role |
|-----------|----------|------|
| `Common` entity | `Empire_ERP.Core/Entities/Common.cs` | Per-request context from session |
| `CommonHelper` | `Empire_ERP/Helpers/CommonHelper.cs` | Session → `Common`; IP; company/branch/period names; permissions |
| `CommonService` | `Empire_ERP.Core/Services/CommonService.cs` | Encryption, parsing, website URL helpers |
| `MyHttpResponseMessage` | `Empire_ERP.Core/Entities/MyHttpResponseMessage.cs` | Standard JSON envelope |
| `CheckSession` / `ExtractMenuCode` | `Empire_ERP/Helpers/SessionHelper.cs` | Action filters |
| `ServiceRegistrationHelper` | `Empire_ERP/Helpers/ServiceRegistrationHelper.cs` | DI |
| `OrderHub` | `Empire_ERP/Helpers/OrderHub.cs` | SignalR: `SendOrder` → `ReceiveOrder` |
| `Menu` / `IMenuService` | Core | Navigation and screen metadata (`TABLE1`, `TABLE2`, `PERFIX`, `VOUCHER_LEN`, `STK_STATUS`, `B_I`, report names) |
| `IBaseService` | Core | Settings, approval, colors |
| `CommonRepository` | Infrastructure | QR/barcode images, zip, accounting number format, DevExtreme filter → SQL |

---

## 15. Important Project Conventions

1. **Menu-driven tables.** Many vouchers do not hardcode table names. `TBL_MENU_BUILDER` supplies `TABLE1` (header), `TABLE2` (detail), prefix, voucher length, and stock flags.
2. **One controller + one Razor Index + one `empr_*.js` file** per screen in most modules.
3. **AJAX JSON CRUD.** Save/delete/search return `Json(MyHttpResponseMessage)` rather than full page posts.
4. **Soft delete** (`DLT`) rather than `DELETE FROM`.
5. **Company / branch / period** on transactional queries via `Common`.
6. **Voucher numbers** often `{B_SHORT_NAME}/{PERFIX}/{yy-MM}/{padded TRAN_ID}` (module-specific variants exist; POS uses `{PERFIX}/{padded seq}`).
7. **RDLC filename** often comes from `TBL_MENU_BUILDER_DETAIL.REPORT_NAME`.
8. **Admin role type** is the string `"A"`.
9. **Generated PDFs** are written under `wwwroot/Client/{Module}/`.
10. **Copy / backup RDLC and JS files** exist (`- Copy`, `_Old`, `_Backup`); they are not all referenced by live code.

---

## 16. Request / Response Flow (Identifiable)

### Page load (non-AJAX)

1. Browser requests `/{Controller}/Index?MOID=...&Code=...`.
2. `CheckSession` requires `Session["Id"]`.
3. `ExtractMenuCode` stores `Code` as `MenuID`.
4. `BaseController.OnActionExecuting` builds menu HTML, breadcrumb, theme.
5. Controller `Index` sets `ViewBag.Permissions` / `DATA_CLEAR` and returns `View()`.
6. `_Layout.cshtml` renders chrome; module view renders the card/form/grid placeholders.
7. `LinkScriptSection` loads `empr_{Module}.js` and calls `InitEvents()`.

### Save (AJAX)

1. JS collects form + DevExtreme grid `dataSource` in `GetDataToSave()`.
2. `ajaxHelper.ajaxPostJsonData` POSTs JSON to `/{Controller}/{SaveAction}`.
3. Controller calls service → repository.
4. Repository may start `SqlTransaction`, insert/update header and details, commit.
5. JSON `{ msgType, msg, data, voucherNo, ... }` returns; `empr_helper.notify` shows Toastr.

### Print (RDLC)

1. JS posts print model (`TRAN_ID`, `MD_ID`, report type).
2. Controller `GetPrintReport` → service `GetDataForReport` → repository (`PROC_PRINT` or SQL).
3. Controller `GenerateReport` loads `Reports\{REPORT_NAME}.rdlc`, sets parameters/datasets, `Render("PDF")`, saves under `wwwroot/Client/...`.
4. Client opens or downloads the PDF path.

### Login

1. `Login/Index` (no layout).
2. Credentials posted → session user keys.
3. `Login/Details` selects company, branch, period.
4. Redirect to `Home/Index`.

---

## 17. Configuration

| File | Role |
|------|------|
| `Empire_ERP/appsettings.json` | Logging, `AllowedHosts`, `AppSettings:DefaultConnection`, `IISSettings` (site name / app path for `HomeController.StopApplication`) |
| `Empire_ERP/appsettings.Development.json` | Logging only |
| `Program.cs` | Pipeline, SignalR, form limits, default route |

Connection string value is environment-specific and is **not copied into this document**.

---

## 18. Pipeline (`Program.cs`)

```
CreateBuilder
AddSignalR
AddControllersWithViews
ServiceRegistrationHelper.RegisterServices
Build
[if not Development: UseExceptionHandler("/Home/Error"), UseHsts]
MapHub<OrderHub>("/orderHub")
UseHttpsRedirection
UseStaticFiles
UseRouting
UseSession
UseAuthorization
MapControllerRoute default → Login/Index
Run
```

Custom middleware classes: **Not found / Not identified**.

---

## 19. Real-Time

`OrderHub.SendOrder(string orderJson)` broadcasts `ReceiveOrder` to all clients. Mapped at `/orderHub`. Further business rules around who calls `SendOrder` are module-specific and not centralized.

---

## 20. Areas That Could Not Be Fully Identified

- Entity Framework / ORM mappings: **not present**.
- ASP.NET Identity / OAuth / JWT: **not present**.
- Dedicated ViewModels project: **not present**.
- Stored procedure source code: **not in this repo**.
- Active `DailyProductionController.cs` (only a `DailyProductionController - Copy.cs` file was found).
- Active Payroll module: **not present** (commented references only).
- Exact SQL table names for many vouchers: **menu-configured at runtime**, not hardcoded.
- Remember-me login restore: **not active** in `LoginController.Index`.
