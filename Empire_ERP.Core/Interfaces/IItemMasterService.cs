using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IItemMasterService
    {
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage AttributeDelete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage Save(ItemMaster itemMaster, Common common);
        //MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetItemMasterByCode(int code, Common common);
        MyHttpResponseMessage GetBarcodeInfoByCode(int code, Common common);
        MyHttpResponseMessage SaveBarcodeInfo(Barcode barCode, Common common);
        MyHttpResponseMessage SaveAttributeInfo(ItemAttribute itemAttribute, Common common);
        MyHttpResponseMessage GetBarcodeInfoByBarcodeId(int barCodeId, int code, Common common);
        MyHttpResponseMessage DeleteBarcodeInfo(int barCodeId, int code, Common common);
    }
}