using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IItemOpeningService
	{
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(ItemOpening itemOpening, Common common);
        MyHttpResponseMessage GetItemOpeningByCode(int code, Common common);
		MyHttpResponseMessage Delete(int code, Common common);
		MyHttpResponseMessage GetItemOpeningDetailByCode(int code, Common common);
		MyHttpResponseMessage GetBarcodeDetailByCode(int code, Common common);
		MyHttpResponseMessage SaveBarcode(BarcodeOpening barcodeOpening, Common common);
		MyHttpResponseMessage GetBarcodeByCode(int code, Common common);
		MyHttpResponseMessage DeleteBarcode(int code, Common common);
	}
}