using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPOSUserRightsService
    {

        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(POSUserright model, Common common);
        MyHttpResponseMessage GetPosUserByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        //MyHttpResponseMessage CopyRecord(CopyRecordSalesman code, Common common);
        MyHttpResponseMessage DeleteCommisionMapDetailByCode(int gcode, int code, Common common);
    }
}
