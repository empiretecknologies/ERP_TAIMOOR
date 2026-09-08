using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class CompanyService : ICompanyService
    {
        public ICompanyRepository _companyRepository { get; set; }
        public CompanyService(ICompanyRepository CompanyRepository)
        {
            _companyRepository = CompanyRepository;
        }
        
        public MyHttpResponseMessage GetCompanies()
        {
            return _companyRepository.GetCompanies();
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _companyRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(Company model, Common common)
        {
            return _companyRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _companyRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetCompanyById(int id, Common common)
        {
            return _companyRepository.GetCompanyById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _companyRepository.Delete(id, common);
        }

        public MyHttpResponseMessage GetCompanyByCode(int code)
        {
            return _companyRepository.GetCompanyByCode(code);
        }
    }
}