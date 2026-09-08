using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class EntityService : IEntityService
    {
        public IEntityRepository _entityRepository { get; set; }
        public EntityService(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public MyHttpResponseMessage GetEntity(int menuid)
        {
            return _entityRepository.GetEntity(menuid);
        }
    }
}
