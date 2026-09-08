using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class ItemOpeningService : IItemOpeningService
	{
        public IItemOpeningRepository _itemOpeningRepository { get; set; }
        public ItemOpeningService(IItemOpeningRepository itemOpeningRepository)
        {
			_itemOpeningRepository = itemOpeningRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _itemOpeningRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(ItemOpening itemOpening, Common common)
        {
            return _itemOpeningRepository.Save(itemOpening, common);
        }

        public MyHttpResponseMessage GetItemOpeningByCode(int code, Common common)
        {
            return _itemOpeningRepository.GetItemOpeningByCode(code, common);
		}

		public MyHttpResponseMessage Delete(int code, Common common)
		{
			return _itemOpeningRepository.Delete(code, common);
		}

		public MyHttpResponseMessage GetItemOpeningDetailByCode(int code, Common common)
		{
			return _itemOpeningRepository.GetItemOpeningDetailByCode(code, common);
		}

		public MyHttpResponseMessage GetBarcodeDetailByCode(int code, Common common)
		{
			return _itemOpeningRepository.GetBarcodeDetailByCode(code, common);
		}

		public MyHttpResponseMessage SaveBarcode(BarcodeOpening barcodeOpening, Common common)
		{
			return _itemOpeningRepository.SaveBarcode(barcodeOpening, common);
		}

		public MyHttpResponseMessage GetBarcodeByCode(int code, Common common)
		{
			return _itemOpeningRepository.GetBarcodeByCode(code, common);
		}

		public MyHttpResponseMessage DeleteBarcode(int code, Common common)
		{
			return _itemOpeningRepository.DeleteBarcode(code, common);
		}
	}
}