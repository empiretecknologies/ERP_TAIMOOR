using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;

namespace Empire_ERP.Helpers
{
    public class ServiceRegistrationHelper
    {
        public static void RegisterServices(IServiceCollection services)
        //public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            //services.AddControllersWithViews();
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(365);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            //services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
            //{
            //    options.Limits.MaxRequestBodySize = 1_000_000; // if don't set default value is: 30 MB
            //});

            // Configure the request size limit
            //services.Configure<FormOptions>(options =>
            //{
            //    //options.MultipartBodyLengthLimit = 2147483648; // 2GB in bytes
            //    options.ValueLengthLimit = int.MaxValue; // The maximum allowed individual value length
            //    options.MultipartBodyLengthLimit = int.MaxValue; // The maximum length of the request body
            //    options.MemoryBufferThreshold = int.MaxValue; // The maximum length of the request body before buffering in memory
            //});


            //services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
            //{
            //    options.Limits.MaxRequestBodySize = 1_000_000; // if don't set default value is: 30 MB
            //});
            //services.Configure<FormOptions>(x =>
            //{
            //    x.ValueLengthLimit = 1_000_000;
            //    x.MultipartBodyLengthLimit = 1_000_000; // if don't set default value is: 128 MB
            //    x.MultipartHeadersLengthLimit = 1_000_000;
            //});

            services.Configure<FormOptions>(x =>
            {
                x.ValueCountLimit = 52428800;
            });

            //services.Configure<SQLService>(options =>
            //{
            //    options.SetConnection(configuration.GetConnectionString("DefaultConnection"));
            //});

            // Add your service and repository registrations
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ILoginRepository, LoginRepository>();

            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IPeriodService, PeriodService>();
            services.AddScoped<IPeriodRepository, PeriodRepository>();

            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IMenuRepository, MenuRepository>();

            services.AddScoped<IChartOfAccountService, ChartOfAccountsService>();
            services.AddScoped<IChartOfAccountRepository, ChartOfAccountRepository>();
            services.AddScoped<IAccountGroupService, AccountGroupService>();
            services.AddScoped<IAccountGroupRepository, AccountGroupRepository>();
            services.AddScoped<IAccountNatureService, AccountNatureService>();
            services.AddScoped<IAccountNatureRepository, AccountNatureRepository>();

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IEntityService, EntityService>();
            services.AddScoped<IEntityRepository, EntityRepository>();
            services.AddScoped<IRegionService, RegionService>();
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<IPartyService, PartyService>();
            services.AddScoped<IPartyRepository, PartyRepository>();
            services.AddScoped<ISalesManService, SalesManService>();
            services.AddScoped<ISalesManRepository, SalesManRepository>();

            services.AddScoped<IItemGroupService, ItemGroupService>();
            services.AddScoped<IItemGroupRepository, ItemGroupRepository>();

            services.AddScoped<ISetupSubTypeService, SetupSubTypeService>();
            services.AddScoped<ISetupSubTypeRepository, SetupSubTypeRepository>();

            services.AddScoped<ISetupTypeService, SetupTypeService>();
            services.AddScoped<ISetupTypeRepository, SetupTypeRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IItemMasterService, ItemMasterService>();
            services.AddScoped<IItemMasterRepository, ItemMasterRepository>();

            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();

			services.AddScoped<IItemOpeningService, ItemOpeningService>();
			services.AddScoped<IItemOpeningRepository, ItemOpeningRepository>();

			services.AddScoped<IPartyOpeningService, PartyOpeningService>();
			services.AddScoped<IPartyOpeningRepository, PartyOpeningRepository>();

            services.AddScoped<IAccountOpeningService, AccountOpeningService>();
            services.AddScoped<IAccountOpeningRepository, AccountOpeningRepository>();

            services.AddScoped<IMaterialRequisitionService, MaterialRequisitionService>();
            services.AddScoped<IMaterialRequisitionRepository, MaterialRequisitionRepository>();

            services.AddScoped<IStockTransferService, StockTransferService>();
            services.AddScoped<IStockTransferRepository, StockTransferRepository>();

            services.AddScoped<IStockTransferRequisitionService, StockTransferRequisitionService>();
            services.AddScoped<IStockTransferRequisitionRepository, StockTransferRequisitionRepository>();

            services.AddScoped<IBarcodePrintService, BarcodePrintService>();
            services.AddScoped<IBarcodePrintRepository, BarcodePrintRepository>();

            services.AddScoped<IPurchaseBillService, PurchaseBillService>();
            services.AddScoped<IPurchaseBillRepository, PurchaseBillRepository>();

            services.AddScoped<IPOSDiscountService, POSDiscountService>();
            services.AddScoped<IPOSDiscountRepository, POSDiscountRepository>();

            services.AddScoped<IClosingShopService, ClosingShopService>();
            services.AddScoped<IClosingShopRepository, ClosingShopRepository>();

            services.AddScoped<ISodaBookFeedingService, SodaBookFeedingService>();
            services.AddScoped<ISodaBookFeedingRepository, SodaBookFeedingRepository>();

            services.AddScoped<IImportGeneralManifestService, ImportGeneralManifestService>();
            services.AddScoped<IImportGeneralManifestRepository, ImportGeneralManifestRepository>();

            services.AddScoped<IDeliveryFeedingService, DeliveryFeedingService>();
            services.AddScoped<IDeliveryFeedingRepository, DeliveryFeedingRepository>();

            services.AddScoped<IBillOfMaterialService, BillOfMaterialService>();
            services.AddScoped<IBillOfMaterialRepository, BillOfMaterialRepository>();

            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ICommonRepository, CommonRepository>();

            services.AddScoped<ICashReceiptVoucherService, CashReceiptVoucherService>();
            services.AddScoped<ICashReceiptVoucherRepository, CashReceiptVoucherRepository>();

            services.AddScoped<IPartyReceiptVoucherService, PartyReceiptVoucherService>();
            services.AddScoped<IPartyReceiptVoucherRepository, PartyReceiptVoucherRepository>();

            services.AddScoped<IPurchaseBookVoucherService, PurchaseBookVoucherService>();
            services.AddScoped<IPurchaseBookVoucherRepository, PurchaseBookVoucherRepository>();

            services.AddScoped<IJournalVoucherService, JournalVoucherService>();
            services.AddScoped<IJournalVoucherRepository, JournalVoucherRepository>();

            services.AddScoped<IAccountingReportService, AccountingReportService>();
            services.AddScoped<IAccountingReportRepository, AccountingReportRepository>();

            services.AddScoped<IPartyReportService, PartyReportService>();
            services.AddScoped<IPartyReportRepository, PartyReportRepository>();

            services.AddScoped<ISPartyReportService, SPartyReportService>();
            services.AddScoped<ISPartyReportRepository, SPartyReportRepository>();

            services.AddScoped<IPurchaseBillReportService, PurchaseBillReportService>();
            services.AddScoped<IPurchaseBillReportRepository, PurchaseBillReportRepository>();

            services.AddScoped<IDatabaseBackupService, DatabaseBackupService>();
            services.AddScoped<IDatabaseBackupRepository, DatabaseBackupRepository>();

            services.AddScoped<IPOSTransactionService, POSTransactionService>();
            services.AddScoped<IPOSTransactionRepository, POSTransactionRepository>();

            services.AddScoped<IDeliveryFormatService, DeliveryFormatService>();
            services.AddScoped<IDeliveryFormatRepository, DeliveryFormatRepository>();

            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            services.AddScoped<IMenuDetailsService, MenuDetailsService>();
            services.AddScoped<IMenuDetailsRepository, MenuDetailsRepository>();

            services.AddScoped<IReportTypeService, ReportTypeService>();
            services.AddScoped<IReportTypeRepository, ReportTypeRepository>();

            services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();

            services.AddScoped<IPurchaseSaleFormatService, PurchaseSaleFormatService>();
            services.AddScoped<IPurchaseSaleFormatRepository, PurchaseSaleFormatRepository>();

            services.AddScoped<IPartyToPartyService, PartyToPartyService>();
            services.AddScoped<IPartyToPartyRepository, PartyToPartyRepository>();

            services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();
            services.AddScoped<IStockAdjustmentRepository, StockAdjustmentRepository>();

            services.AddScoped<ISalesInvoiceReportService, SalesInvoiceReportService>();
            services.AddScoped<ISalesInvoiceReportRepository, SalesInvoiceReportRepository>();

            services.AddScoped<IPurchaseRequisitionService, PurchaseRequisitionService>();
            services.AddScoped<IPurchaseRequisitionRepository, PurchaseRequisitionRepository>();

            services.AddScoped<IImportManifestService, ImportManifestService>();
            services.AddScoped<IImportManifestRepository, ImportManifestRepository>();

            services.AddScoped<INotesService, NotesService>();
            services.AddScoped<INotesRepository, NotesRepository>();

            services.AddScoped<ILotRegistrationService, LotRegistrationService>();
            services.AddScoped<ILotRegistrationRepository, LotRegistrationRepository>();

            services.AddScoped<IGatePassService, GatePassService>();
            services.AddScoped<IGatePassRepository, GatePassRepository>();

            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            
            services.AddScoped<IAttendanceMachineService, AttendanceMachineService>();
            services.AddScoped<IAttendanceMachineRepository, AttendanceMachineRepository>();
            
            services.AddScoped<IPOSMappingService, POSMappingService>();
            services.AddScoped<IPOSMappingRepository, POSMappingRepository>();
            
            services.AddScoped<IMachineInfoService, MachineInfoService>();
            services.AddScoped<IMachineInfoRepository, MachineInfoRepository>();
            
            services.AddScoped<IPOSDiscountItemWiseService, POSDiscountItemWiseService>();
            services.AddScoped<IPOSDiscountItemWiseRepository, POSDiscountItemWiseRepository>();
            
            services.AddScoped<ISalesContractService, SalesContractService>();
            services.AddScoped<ISalesContractRepository, SalesContractRepository>();
            
            services.AddScoped<IImportPermitService, ImportPermitService>();
            services.AddScoped<IImportPermitRepository, ImportPermitRepository>();
            
            services.AddScoped<IShipmentService, ShipmentService>();
            services.AddScoped<IShipmentRepository, ShipmentRepository>();
            
            services.AddScoped<IBankDetailService, BankDetailService>();
            services.AddScoped<IBankDetailRepository, BankDetailRepository>();
            
            services.AddScoped<IDocumentRetrievalService, DocumentRetrievalService>();
            services.AddScoped<IDocumentRetrievalRepository, DocumentRetrievalRepository>();
            
            services.AddScoped<IHawlaService, HawlaService>();
            services.AddScoped<IHawlaRepository, HawlaRepository>();
            
            services.AddScoped<IBatchIssueService, BatchIssueService>();
            services.AddScoped<IBatchIssueRepository, BatchIssueRepository>();
            
            services.AddScoped<IDeliveryOrderService, DeliveryOrderService>();
            services.AddScoped<IDeliveryOrderRepository, DeliveryOrderRepository>();

            services.AddScoped<IMembershipCardService, MembershipCardService>();
            services.AddScoped<IMembershipCardRepository, MembershipCardRepository>();

            services.AddScoped<ICashBookVoucherService, CashBookVoucherService>();
            services.AddScoped<ICashBookVoucherRepository, CashBookVoucherRepository>();

            services.AddScoped<IDailyProductionService, DailyProductionService>();
            services.AddScoped<IDailyProductionRepository, DailyProductionRepository>();
          
            services.AddScoped<IWeighBridgeService, WeighBridgeService>();
            services.AddScoped<IWeighBridgeRepository, WeighBridgeRepository>();

            services.AddScoped<IWorkOrderService, WorkOrderService>();
            services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
          
            services.AddScoped<IHRMasterService, HRMasterService>();
            services.AddScoped<IHRMasterRepository, HRMasterRepository>();

            services.AddScoped<IFamilyMemberService, FamilyMemberService>();
            services.AddScoped<IFamilyMemberRepository, FamilyMemberRepository>();

            services.AddScoped<IHrMasterTableServices, HrMasterTableServices>();
            services.AddScoped<IHrMasterTableRepository, HrMasterTableRepository>();

            services.AddScoped<IEmpMasterInfoService, EmpMasterInfoService>();
            services.AddScoped<IEmpMasterInfoRepository, EmpMasterInfoRepository>();
            
            services.AddScoped<IEmpLeavesService, EmpLeavesService>();
            services.AddScoped<IEmpLeavesRepository, EmpLeavesRepository>();

            services.AddScoped<IBinariesService, BinariesService>();
            services.AddScoped<IBinariesRepository, BinariesRepository>();
            
            services.AddScoped<IEmpPenaltyService, EmpPenaltyService>();
            services.AddScoped<IEmpPenaltyRepository, EmpPenaltyRepository>();
            
            services.AddScoped<IHRSetupService, HRSetupService>();
            services.AddScoped<IHRSetupRepository, HRSetupRepository>();

            services.AddScoped<IEmpTransferEntryService, EmpTransferEntryService>();
            services.AddScoped<IEmpTransferEntryRepository, EmpTransferEntryRepository>();
           
            services.AddScoped<IHRJobPostService, HRJobPostService>();
            services.AddScoped<IHRJobPostRepository, HRJobPostRepository>();

            services.AddScoped<IMerchantPurchaseOrderService, MerchantPurchaseOrderService>();
            services.AddScoped<IMerchantPurchaseOrderRepository, MerchantPurchaseOrderRepository>();

            services.AddScoped<IMerchantPurchaseOrderDetailService, MerchantPurchaseOrderDetailService>();
            services.AddScoped<IMerchantPurchaseOrderDetailRepository, MerchantPurchaseOrderDetailRepository>();

            services.AddScoped<IHRCandidateService, HRCandidateService>();
            services.AddScoped<IHRCandidateRepository, HRCandidateRepository>();

			services.AddScoped<IHRInterviewScheduleService, HRInterviewScheduleService>();
			services.AddScoped<IHRInterviewScheduleRepository, HRInterviewScheduleRepository>();

            services.AddScoped<IHRInterviewFeedbackService, HRInterviewFeedbackService>();
            services.AddScoped<IHRInterviewFeedbackRepository, HRInterviewFeedbackRepository>();

            services.AddScoped<IHROfferLetterService, HROfferLetterService>();
            services.AddScoped<IHROfferLetterRepository, HROfferLetterRepository>();

            services.AddScoped<ICostCenterService, CostCenterService>();
            services.AddScoped<ICostCenterRepository, CostCenterRepository>();

            services.AddScoped<IImportReportService, ImportReportService>();
            services.AddScoped<IImportReportRepository, ImportReportRepository>();
            
            services.AddScoped<ITableService, TableService>();
            services.AddScoped<ITableRepository, TableRepository>();
                
            services.AddScoped<IWaiterService, WaiterService>();
            services.AddScoped<IWaiterRepository, WaiterRepository>();

            services.AddScoped<IStockReceiveService, StockReceiveService>();
            services.AddScoped<IStockReceiveRepository, StockReceiveRepository>();

            services.AddScoped<IFSodaBookFeedingService, FSodaBookFeedingService>();
            services.AddScoped<IFSodaBookFeedingRepository, FSodaBookFeedingRepository>();

            services.AddScoped<IFDeliveryFeedingService, FDeliveryFeedingService>();
            services.AddScoped<IFDeliveryFeedingRepository, FDeliveryFeedingRepository>();

            services.AddScoped<IKnockOffService, KnockOffService>();
            services.AddScoped<IKnockOffRepository, KnockOffRepository>();

            services.AddScoped<ITexSalesInvoiceService, TexSalesInvoiceService>();
            services.AddScoped<ITexSalesInvoiceRepository, TexSalesInvoiceRepository>();

            services.AddScoped<IMPORegistrationService, MPORegistrationService>();
            services.AddScoped<IMPORegistrationRepository, MPORegistrationRepository>();

            services.AddScoped<ICommMapService, CommMapService>();
            services.AddScoped<ICommMapRepository, CommMapRepository>();

            services.AddScoped<IMpoLayoutService, MpoLayoutService>();
            services.AddScoped<IMpoLayoutRepository, MpoLayoutRepository>();

            services.AddScoped<IMpoTrimsService, MpoTrimsService>();
            services.AddScoped<IMpoTrimsRepository, MpoTrimsRepository>();
            
            services.AddScoped<ICustomerPricingService, CustomerPricingService>();
            services.AddScoped<ICustomerPricingRepository, CustomerPricingpRepository>();

            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<IBaseRepository, BaseRepository>();

            services.AddScoped<ISaleTaxInvoiceService, SaleTaxInvoiceService>();
            services.AddScoped<ISaleTaxInvoiceRepository, SaleTaxInvoiceRepository>();

            services.AddScoped<IPOSUserRightsService, POSUserRightsService>();
            services.AddScoped<IPOSUserRightsRepository, POSUserRightsRepository>();

            services.AddScoped<IApprovalService, ApprovalService>();
            services.AddScoped<IApprovalRepository, ApprovalRepository>();

            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<IGradeRepository, GradeRepository>();

            services.AddScoped<IInvoiceReportsService, InvoiceReportsService>();
            services.AddScoped<IInvoiceReportsRepository, InvoiceReportsRepository>();

            services.AddScoped<IUploadItemImagesService, UploadItemImagesService>();
            services.AddScoped<IUploadItemImagesRepository, UploadItemImagesRepository>();
        }
    }
}