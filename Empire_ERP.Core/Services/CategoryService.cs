using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class CategoryService : ICategoryService
    {
        
        public ICategoryRepository _categoryRepository { get; set; }
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public MyHttpResponseMessage GetCategoryies(int menuid)
        {
            return _categoryRepository.GetCategoryies(menuid);
        }
    }
}
