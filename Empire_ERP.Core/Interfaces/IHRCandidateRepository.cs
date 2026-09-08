using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IHRCandidateRepository
    {
        MyHttpResponseMessage GetHRJobPosts(Common common);
        MyHttpResponseMessage GetAccountsForTreeView(Common common);
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetHRJobPostById(int id, Common common);
        MyHttpResponseMessage GetEmpMails(Common common);
        MyHttpResponseMessage Save(HRCandidate model, Common common);
        string GenerateNextId(Common common);
        string GenerateGrCode(string ParentId, Common common);
        string GenerateCardNo(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
        MyHttpResponseMessage GetDataForReport(HRCandidateReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}
