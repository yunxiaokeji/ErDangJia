using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Areas.Api.Controllers
{
    [YXERP.Common.ApiAuthorize]
    public class StockController : BaseAPIController
    {
        //
        // GET: /Api/Purchase/

        public ActionResult GetPurchases(string userid, int status, string keywords, string begintime, string endtime, string wareid, string providerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
           
             
        }

    }
}
