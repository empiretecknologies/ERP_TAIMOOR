# Business Logic

This document records business rules and workflows that can be verified in C# controllers, services, repositories, and SQL strings in this repository. Stored procedure **bodies** are not in the repo; only call sites and parameters visible in C# are documented.

Do not treat this file as a complete specification of SQL Server objects. Where a rule lives only inside a stored procedure, that is stated.

---

## 1. Cross-Cutting Rules

### Soft delete

- Active rows: `DLT = 'T'`
- Delete: `UPDATE ... SET DLT = 'F'` (physical `DELETE` is not the standard pattern)

### Active status

- Many masters: `ASTATUS = 'Y'` (active) / `'N'` (inactive)

### Session context (`Common`)

Transactional modules commonly filter by:

- `BCODE` = session branch
- `PERIOD_ID` = session period
- Company from session (`CCODE` / `Company`)

### Menu-driven configuration (`TBL_MENU_BUILDER`)

Per screen, menu metadata can supply:

- `TABLE1` / `TABLE2` — header / detail table names
- `PICK_TABLE_MASTER` / `PICK_TABLE_DETAIL`
- `PERFIX`, `VOUCHER_LEN`
- `STK_STATUS`, `B_I`
- Report names via `TBL_MENU_BUILDER_DETAIL.REPORT_NAME`

Exact physical table names for many vouchers are therefore **runtime values**, not hardcoded.

### Standard response

Repositories return `MyHttpResponseMessage`:

- `msgType = 1` success
- `msgType = 2` error
- Optional `data`, `voucherNo`, `SaveData`, `msg`

### Transactions

Save methods in many repositories use `SqlConnection.BeginTransaction()` with Commit / Rollback. Files that do this include (non-exhaustive): PurchaseBill, JournalVoucher, POS (via related repos), StockTransfer, StockAdjustment, Login, Party, Company, and others listed in architecture notes.

### Stock check (C# orchestration)

When menu `STK_STATUS == 'Y'` and `B_I == 'B'`, several save paths call:

```
EXEC STKPROC 71, ... item codes ..., 'B', ...
```

Returned `BALANCE` is compared to requested qty. Insufficient stock blocks save.

POS item availability uses `EXEC STKPROC 26, ... barcode ..., 'B', ...`.

Full `STKPROC` mode list beyond **26** and **71** is **Not found / Not identified** in C#.

### Tax amounts

Line fields persisted in several documents: `TAX`, `TAX_AMT`, `DISC`, `DISC_AMT`, `NET_AMT`, `AMT`, `RATE`, `QTY`.

Server-side tax **formulas** are often not recalculated in C#; values are taken from the posted model. One verified formula:

- Sale Tax Invoice FBR export: `(AMT - DISC_AMT) * TAX / 100 AS ST_APPLICABLE`

### Voucher numbers (verified formats)

| Area | Format in code |
|------|----------------|
| Journal voucher, Purchase Bill, Delivery Order, Sale Tax Invoice (typical) | `{B_SHORT_NAME}/{PERFIX}/{yy-MM}/{padded TRAN_ID}` |
| POS | `{PERFIX}/{padded seq}` from `MAX(TRAN_ID)` on `TBL_POS_MASTER` |
| POS SRB voucher | Same prefix/length; sequence from `COUNT(SRB_VN)` where `SRB_VN != ''` |

### Role permissions (`TBL_ROLE`)

Flags: `R_ADD`, `R_EDIT`, `R_DLT`, `R_VIEW`, `R_PRINT`, `R_COPY`, `R_BCODE`, `RMENU_ID`, `MODULE_ID`.

`RoleType = 'A'` is treated as administrator (all menus; permissions string `"Admin"` in many controllers).

---

## 2. Authentication and Login

**Files:** `LoginController`, `LoginService`, `LoginRepository`, `CommonService.EncryptString`

| | |
|--|--|
| **Purpose** | Authenticate user; then bind company, branch, and period |
| **Input** | Username, password |
| **Processing** | `TBL_USER` joined to `TBL_ROLE`; password AES-encrypted then compared to `UPASS`; requires `ASTATUS='Y'` and `DLT='T'` |
| **Output** | Session user keys; redirect to `Login/Details` then `Home/Index` |
| **Tables** | `TBL_USER`, `TBL_ROLE`, `TBL_WLABEL`, `TBL_COMPANY`, `TBL_BRANCH`, `TBL_PERIOD`, `TBL_FPASSWORD` |

Password reset: OTP stored in `TBL_FPASSWORD` (`STATUS` pending); repository logic uses a 120-second expiry; new password must not match previous `UPASS`.

Branch list: `RoleType = 'A'` sees all branches; otherwise branches from `TBL_ROLE.R_BCODE`.

---

## 3. Setup / Masters

### Company

**Files:** `CompanyController`, `CompanyService`, `CompanyRepository`  
**Table:** `TBL_COMPANY`  
**Rules:** Duplicate check on `C_NAME`. Delete sets `DLT='F'`. Not branch/period scoped in the usual voucher sense.

### Branch

**Files:** `BranchController`, `BranchService`, `BranchRepository`  
**Tables:** `TBL_BRANCH`, `TBL_ROLE`  
**Rules:** Filtered by company (`CCODE`), `DLT='T'`, `ASTATUS='Y'`. `B_SHORT_NAME` is used in voucher numbers.

### Period

**Files:** `PeriodController`, `PeriodService`, `PeriodRepository`  
**Table:** `TBL_PERIOD`  
**Rules:** Per branch. `START_D` / `START_E` used as date range for stock checks. `CLOSING=0` used when resolving a target branch period on stock transfer.

### User

**Files:** `UserController`, `UserService`, `UserRepository`  
**Tables:** `TBL_USER`, `TBL_ROLE`, `TBL_BRANCH`, `TBL_FPASSWORD`  
**Rules:** Encrypted password; duplicate checks on `EMAIL` / `USERNAME`; delete `DLT='F'`.

### Role

**Files:** `RoleController`, `RoleService`, `RoleRepository`  
**Purpose:** Permissions against menus, report types, party types, chart, items (`MODULE_ID` distinguishes types).

### Menu

**Files:** `MenuService`, `MenuRepository`, `MenuDetailsController`, `MenuDetailsRepository`  
**Tables:** `TBL_MENU_BUILDER`, `TBL_MENU_BUILDER_DETAIL`  
**Output:** Screen tables, prefix, voucher length, stock flags, print report names, `DATA_CLEAR`, `SEARCH`, `D_LIMIT` / `LIMIT`.

### Item master

**Files:** `ItemMasterController`, `ItemMasterService`, `ItemMasterRepository`  
**Tables:** `TBL_ITEMSMASTER`, `TBL_BARCODE`, `TBL_ITEM_ATT`, `TBL_CATEGORY`, `TBL_SUB_CATEGORY`, `TBL_COLOR`, `TBL_SIZE`, `TBL_GRADE`, `TBL_BLABEL`  
**Rules:** Cannot delete item if active barcodes exist (`TBL_BARCODE DLT='T'` and `ASTATUS='Y'`). Barcode generation uses `TBL_BLABEL` and menu `VOUCHER_LEN`. Copy record supported.

### Party

**Files:** `PartyTypesController` (UI), `PartyService`, `PartyRepository`  
**Tables:** `TBL_PARTY_TYPES` and related chart/category/entity/region  
**Rules:** Links to `ACT_CODE`; fields include `COMM`, `DISC`, `WHT`, `T_CAT`. Branch sub-records. Delete soft-deletes party and branch rows. `PARTY_TYPE_CODE` from menu context.

There is **no** `PartyController.cs` file; party UI is `PartyTypesController`.

### Warehouse / Chart of accounts

- Warehouse: hierarchical `GR_CODE` via menu table.
- Chart: `TBL_CHART` with groups/natures/currency/types. Delete blocked if child accounts exist (`ACT_PARENT_CODE`). `ACT_TYPE='C'` used in filters.

---

## 4. POS / Sales

### POS transaction (core)

**Files:** `POSTransactionsController`, `POSTransactionService`, `POSTransactionRepository`  
**JS:** `empr_POSTransaction.js`  
**Tables (hardcoded in POS code):** `TBL_POS_MASTER`, `TBL_POS_DETAIL`, `TBL_POS_CUS`, `TBL_POS_EXP`, `TBL_POS_STATUS`, `TBL_POS_MS`, `TBL_POS_DISC`, `TBL_POS_MAP`, `TBL_WAITER`, `TBL_TABLE`, `TBL_BARCODE`, `TBL_ITEMSMASTER`

| | |
|--|--|
| **Purpose** | POS billing: KOT, hold, payment, delivery/advance, membership points, SRB |
| **Input** | `CustomPOSTransaction` (master + details), `Common`, print menu detail |
| **Bill status** | `K` = KOT, `H` = Hold, `P` = Paid / pending payment |
| **INV_STATUS** | From `TBL_POS_STATUS`; advance booking uses `INV_STATUS = 3` |
| **CLOSING** | `0` open, `1` closed (set by shop close) |

**Save processing (verified in repository):**

1. Optional stock check (`STK_STATUS` / `B_I`) aggregating qty by `ITEM_CODE`; net change vs previous bill qty.
2. `SqlTransaction`.
3. New bill (`TRAN_ID = 0` or return): generate `TRAN_ID`, `VOUCHER_NO`; optional `SRB_VN` if `BILL_STATUS='P'` and `SRBSTATUS='Y'`.
4. Insert/update customer `TBL_POS_CUS`.
5. Master totals, discounts, payment splits (`CASH`, `BANK`, `PARTY`), taxes, waiter, table, advance, delivery.
6. **Net total:** `Net_Total = Master.NET_TOTAL - Round(Sum(Detail.DISC_AMT))`.
7. Detail: `QTY`, `RATE`, `AMT`, `DISC`, `DISC_AMT`, `NET_AMT`, `RITEM`, `PICK_ID`, `REMARKS`.
8. KOT: collect `dt_codes` when `BILL_STATUS='K'`.
9. Membership: if `CARD_NO` set, update `TBL_POS_MS.POINT_RATE` using `CardDiscValue` vs `DISC_AMT`.
10. **Block edit** if paid (`BILL_STATUS='P'`, `COMPLETE=1`).
11. Commit; return voucher, SRB number, KOT codes.

**Discounts:** `TBL_POS_DISC` where `BCODE`, `DISC_EXP=0`, `ASTATUS='Y'`, and `TDATE > '2023-11-20'` (literal date in code).

**Tables:** `GetAllTables` excludes tables used by open non-paid bills (`COMPLETE=0`, `BILL_STATUS <> 'P'`).

**Delete:** soft delete master/detail.

**Expense:** `TBL_POS_EXP` with its own voucher sequence.

**Print:** `PROC_PRINT` with names such as `POS_KOT`. POS screens also render Razor receipts (`PointOfSale_2`, `SaleReceipt`, `POS_KOT`) rather than RDLC. `PdfService` builds KOT/sticker PDFs with iText 7.

### POS mapping

**Table:** `TBL_POS_MAP` (per branch)  
**Purpose:** Cash/bank/party accounts, tax rates (`CASH_TAX`, `BANK_TAX`, `PARTY_TAX`), SRB credentials (`SRB_NAME`, `SRB_NTN`, `SRB_ID`, `SRB_STATUS`, `SRB_URL`), UI flags (`KOT_BTN`, payment buttons), `CASH_ACT`.

### SRB (Sindh Revenue Board)

- On paid bill: generate `SRB_VN`; store `SRB_INV`.
- `SRBApi_Status` sets `API_STATUS='Y'` and `SRB_INV`.
- `PendingToSRBController`: paid bills where `SRB_INV` is null/empty/non-numeric.
- Full API request/response payload: **Not found / Not identified** beyond status update SQL.

### Closing shop

**Files:** `ClosingShopController`, `ClosingShopService`, `ClosingShopRepository`

- `EXEC POS_CLOSED '{from}','{to}',{branch},{period},'{user}','{cashAct}'` with cash account from `TBL_POS_MAP.CASH_ACT`.
- `UpdateClosedData`: `CLOSING=1` on `TBL_POS_MASTER` (paid bills in range) and `TBL_POS_EXP`; non-admin users limited to own `EDIT_USER_ID`.

### Sale tax invoice / textile sales invoice

- **SaleTaxInvoice:** `TBL_FBR_SB_MASTER` / `TBL_FBR_SB_DETAIL`, `TBL_FBR_TYPE`; FBR tax formula above; `FBRApi_Status`; print `PROC_PRINT`.
- **TexSalesInvoice:** `TBL_TSB_MASTER` + menu `TABLE2`; party/MPO/barcode; `STKPROC 71`; tax/discount line fields.

### Sales quotation

**Files:** `SalesQutationController`, `SalesQutationService`, `SalesQutationRepository`  
**JS:** `empr_SalesQutation.js`  
**Tables:** menu `TABLE1` / `TABLE2` (expected `TBL_SQ_MASTER` / `TBL_SQ_DETAIL`)

| | |
|--|--|
| **Purpose** | Sales quotation with party and item quantities |
| **Master** | `V_DATE`, `VOUCHER_NO`, `PARTY_CODE`, `ACT_CODE`, `REMARKS`, `ASTATUS` |
| **Detail** | `DT_CODE`, `ITEM_CODE`, `QTY`, `RATE` (Amount = `QTY × RATE`, display-only) |

**Processing:** Save in `SqlTransaction`; insert/update master; soft-delete then insert/update details. Voucher `{B_SHORT_NAME}/{PERFIX}/{yy-MM}/{padded TRAN_ID}`. Delete sets `DLT='F'` on master and details. Line delete uses detail `DT_CODE`.

**UI item catalog:** Item groups use `IPOSTransactionService.GetItemsGroup`; items for a group use `GetItemsMasterByGroup` via `SalesQutationController.GetItemsMasterByGroup` (same POS role/module filters: `ITEM_TYPE='F'`, `GROUP_TYPE='S'`). Selected lines are kept in a JavaScript array (`itemCode`, `rate`, `qty`) and posted as `Detail` to the existing Save transaction. The detail DevExtreme grid is not shown.

### Delivery order / sales contract

- Delivery order: pick from purchase/sales pick tables (`ASTATUS='Y'`); `STKPROC 71`; warehouse/color/size/tax on lines.
- Sales contract: links to soda book feeding / picks (`TBL_SBF_*`, `TBL_ISC_*` via menu). Item access can be role-filtered on `TBL_ITEMSMASTER`.

---

## 5. Purchase

### Purchase bill (core)

**Files:** `PurchaseBillController`, `PurchaseBillService`, `PurchaseBillRepository`

| | |
|--|--|
| **Purpose** | Purchase invoice with tax, discount, commission, pick from PO/MPO |
| **Tables** | Menu `TABLE1`/`TABLE2`; `TBL_COMM_GEN`; joins to party, barcode, item, color, size, grade, unit, currency, `TBL_MPO_MASTER` |
| **Master** | `PARTY_CODE`, `ACT_CODE`, `DOC`, `TERMS`, `REF`, `CURR_CODE`, `CRATE`, `COMM`, `COMM_VAL`, `COMM_AMT`, `DISC`, `BTYPE`, `ASTATUS` |
| **Detail** | `QTY`, `BAL_QTY`, `RATE`, `AMT`, `DISC`, `DISC_AMT`, `TAX`, `TAX_AMT`, `ADV`, `ADV_AMT`, `NET_AMT`, `WAREHOUSE`, `HS_CODE`, `PICK_ID`, `PICK_ID_D` |

**Processing:**

1. Stock check on `BAL_QTY` when `STK_STATUS='Y'` and `B_I='B'` (`STKPROC 71`).
2. Resolve `SACODE` from `TBL_PARTY_TYPES.SACT_CODE` when salesman is set.
3. Insert/update master and details.
4. Commission in `TBL_COMM_GEN` when `B_I='I'` (per-item or `GetComm()` template).
5. Voucher `{B_SHORT_NAME}/{PERFIX}/{yy-MM}/{paddedId}`.
6. Commit.

**Pick:** `GetPickDataByParty` aggregates pick-document qty minus already vouchered qty.  
**Print:** `EXEC PROC_PRINT`.  
**Delete:** `DLT='F'` on master.

### Purchase order / requisition / purchase book voucher

Same master/detail + `BCODE` / `PERIOD_ID` / `DLT` pattern. Requisition screens feed later pick documents. Purchase book voucher follows accounting voucher save (transactional).

### Merchant purchase order (MPO)

Master/detail plus layout, trims, registration, and customer pricing modules. Print uses `mpo_master.rdlc` / `mpo_detail.rdlc` (see reports doc).

---

## 6. Inventory and Production

### Stock transfer

**Save:** `STKPROC 71`; target branch period from `TBL_PERIOD` where `BCODE = TBCODE` and `CLOSING = 0`. On save, existing details are soft-deleted then bulk-inserted.

### Stock transfer requisition / stock receive / stock adjustment / item opening

Same voucher pattern. Stock adjustment can pick from soda book feeding; carton sticker print; `UpdatePrintStatus`. Item opening includes barcode opening.

### Material requisition / batch issue / BOM / work order / daily production

- BOM: finished item to components; `TBL_ITEMSMASTER`, `TBL_PROCESS`.
- Daily production: batch from `TBL_BOM_BT_MASTER`; process-based entries. Print support exists in repository; live controller file is `DailyProductionController - Copy.cs` only.
- Work order / batch issue: transactional save; `PROC_PRINT` for batch issue.

### Barcode print

Builds label rows from barcode/item data; RDLC name from report config; PDF bytes returned to the client.

---

## 7. Accounting / Finance

### Journal voucher (core)

**Files:** `JournalVoucherController`, `JournalVoucherService`, `JournalVoucherRepository`

| | |
|--|--|
| **Purpose** | Multi-line GL journal |
| **Structure** | Menu `TABLE1`: each line is a row sharing `TRAN_ID` / `VOUCHER_NO`; `DT_CODE` distinguishes lines |
| **Line fields** | `ACT_CODE`, `PARTY_CODE`, `DEBIT`, `CREDIT`, `CHQ_NO`, `CHQ_DATE`, `DT_DESC` |

**Save:**

1. `UPDATE {table} SET DLT='F' WHERE TRAN_ID=...` (replace all lines).
2. Re-insert lines with `DLT='T'`.
3. New voucher: generate `TRAN_ID` and `{shortName}/{prefix}/{yy-MM}/{paddedId}`.
4. **Knock-off protection:** if `TBL_CC_DETAIL` exists for a line and `DEBIT < SUM(AMOUNT)`, update is blocked.
5. Default `CHQ_DATE = V_DATE` if empty.
6. Commit.

Quick search aggregates `SUM(DEBIT)`, `SUM(CREDIT)` per `TRAN_ID`.  
Delete: `DLT='F'` all lines for `TRAN_ID`.  
**Debit = credit validation in C# Save:** **Not found / Not identified** (may be client-side or database).

### Cash receipt / cash book / party receipt / party-to-party / purchase book

Journal-style lines. Chart lists can be role-filtered (`TBL_ROLE` module 3). Knock-off / `TBL_CC_DETAIL` checks before edit/delete.

### Knock-off / account opening / party opening

- Knock-off: `TBL_KNOCKOFF` links invoices to receipts; `KNOCKOFF_PICK` used for pick. Soft delete.
- Openings: opening balances for accounts/parties/items.

### Accounting reports

**Files:** `AccountingReportsController`, `AccountingReportService`, `AccountingReportRepository`  
`EXEC APROC '{ReportID}','{FromDate}','{ToDate}','{ControlCode}','{AccountCode}','{Branch}','{Period}'`  
Catalog: `TBL_REPORT_TYPES`.

Party / S-party / purchase bill **inquiry reports** use `PPROC`, `SODA_SUMMARY`, `STK_ORDER_PROC`, and related report repositories. Grid PDF for those screens may go through `ReportController` (iTextSharp), not RDLC.

### Approval

**Files:** `ApprovalController`, `ApprovalService`, `ApprovalRepository`, `BaseRepository.GetApproval`

- List: `EXEC APPROVAL {branch},{period}`
- Setup list: `EXEC APPROVAL_SETUP`
- Approve: `UPDATE {TABLER} SET ASTATUS='Y' WHERE MENU_ID=... AND PERIOD_ID=...`
- Setup approve: `ASTATUS='Y'` by `MENU_ID` and `GROUP_CODE`
- User gate: role `MODULE_ID = 5` for current menu

---

## 8. HR

**Not a full payroll engine in this repo.** HR covers masters and recruitment:

| Module | Notes |
|--------|--------|
| Employee | Duplicate check on `EMP_ID` or `MACHINE_CODE` per branch; joins shift/religion/act group |
| Emp master info / leaves / penalty / transfer | CRUD, branch-scoped where coded |
| Attendance machine / delete attendance | Machine records |
| Family member | Transactional save |
| HR master / HR setup / HR master table | Setup lists |
| Job post / candidate / interview schedule / feedback / offer letter | Recruitment pipeline; offer letter prints RDLC; candidate joins job type/education |

Salary calculation, tax slabs, and payslip posting: **Not found / Not identified** in active code.

---

## 9. Import / Logistics

| Module | Verified behavior |
|--------|-------------------|
| Import manifest | Master/detail; duplicate check on `ITEM_CODE`; shipping agents on detail |
| Import general manifest | Transactional save |
| Import permit | Permit header/detail |
| Import report | Hardcoded `ImportBill.rdlc`; `APROC` / `COST_CENTER` in repository |
| Shipment | Transactional logistics save |
| Hawla | `PROC_PICK_DATA`; can link journal (`GetTJVRecord`) |
| GatePass | Lot, unit, party, item, driver, vehicle; parameter-driven RDLC |
| WeighBridge | Transactional save |
| Soda book feeding / F-soda / delivery feeding | Contract/delivery feeding documents; invoice-style RDLC |

---

## 10. Other Modules

| Module | Notes |
|--------|--------|
| Membership card | Card types; `POINT_START_VALUE`, `POINT_RATE`, `MAX_POINTS_DISC`; POS updates `TBL_POS_MS` |
| Table / Waiter | POS seating/service masters |
| Database backup | `sp_spaceused`; backup repository |
| MailBox | Mail UI/controller present |
| Document retrieval | File/document save with transaction |
| Customer pricing | Copy/pricing UI |
| Comm map | Commission mapping |
| Favorite menu | `TBL_FAVORITE_MENUS` including `iS_SIDEBAR_OPEN` |
| POS user rights | POS rights repository |
| Machine info | Hardware/machine master; RDLC print commented |

---

## 11. Reporting Logic (non-RDLC)

`ReportController.GeneratePDF` builds a grouped PDF from DevExtreme grid JSON using **iTextSharp**. It does not load `.rdlc` files.

`PdfService` generates POS KOT and stickers with **iText 7** and ZXing.

---

## 12. Calculations Summary (verified in C#)

| Calculation | Location |
|-------------|----------|
| POS net total after line discounts | `Net_Total = Master.NET_TOTAL - Round(Sum(Detail.DISC_AMT))` |
| FBR ST applicable | `(AMT - DISC_AMT) * TAX / 100` |
| Stock availability | `STKPROC` 71 / 26 |
| Voucher sequential IDs | `MAX(TRAN_ID)` or similar per table |
| Journal replace-all lines | Soft-delete then re-insert |
| Accounting format display | `CommonRepository.ToAccountingFormat` (negatives in parentheses) |

Line tax/discount **arithmetic not recalculated on the server** in many save methods: posted JSON values are stored.

---

## 13. Gaps

- Stored procedure internals (`STKPROC`, `APROC`, `APPROVAL`, `POS_CLOSED`, `PROC_PRINT`, …): **Not found / Not identified** in this repo.
- Journal debit/credit balance check in Save: **Not found / Not identified**.
- GST vs sales-tax legal rules beyond stored `TAX` fields: **Not found / Not identified**.
- Complete SRB/FBR HTTP payloads: **Not found / Not identified**.
- Payroll processing: **Not found / Not identified**.
- Physical table names for menu-driven vouchers: resolved at runtime from `TBL_MENU_BUILDER`.
