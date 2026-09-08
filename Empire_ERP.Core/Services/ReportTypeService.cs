using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class ReportTypeService : IReportTypeService
    {
        public IReportTypeRepository _reportTypeRepository { get; set; }
        public ReportTypeService(IReportTypeRepository reportTypeRepository)
        {
            _reportTypeRepository = reportTypeRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _reportTypeRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(ReportType model, Common common)
        {
            return _reportTypeRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _reportTypeRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetReportTypeById(int id, Common common)
        {
            return _reportTypeRepository.GetReportTypeById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _reportTypeRepository.Delete(id, common);
        }
    }
}