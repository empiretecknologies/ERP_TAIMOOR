using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPOSUserRightsRepository
    {
        MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Common common);
        MyHttpResponseMessage Save(POSUserright model, Common common);
        MyHttpResponseMessage GetPosUserByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        //MyHttpResponseMessage CopyRecord(CopyRecordSalesman record, Common common, Menu menu);
        MyHttpResponseMessage DeleteCommisionMapDetailByCode(int gcode, int code, Common common, Menu menu);
    }
}
