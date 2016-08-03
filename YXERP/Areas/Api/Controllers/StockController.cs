using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CloudSalesEntity;
using YunXiaoService;
using YXERP.Areas.Api.Models;
using YXERP.Models;

namespace YXERP.Areas.Api.Controllers
{
    [YXERP.Common.ApiAuthorize]
    public class StockController : BaseAPIController
    {
        //
        // GET: /Api/Purchase/

        public ActionResult GetPurchases(string userid, int status, string keywords, string begintime, string endtime, string wareid, string providerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            int total = 0;
            int pagecount = 0;
            var list = StockService.GetPurchases(userid, status, keywords, begintime, endtime, wareid, providerid,
                pageSize, pageIndex, ref total, ref pagecount, agentid, clientid);

            JsonDictionary.Add("TotalCount", total);
            JsonDictionary.Add("PageCount", pagecount);
            JsonDictionary.Add("result", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult AuditPurchase(string filterPurchase, string userid, string agentid, string clientid)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(filterPurchase))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                FilterPurchase model = serializer.Deserialize<FilterPurchase>(filterPurchase);
                if (!string.IsNullOrEmpty(model.DocID))
                {
                    string details = "";
                    if (model.Details != null)
                    {
                        model.Details.ForEach(x => { details += x.AutoID + "-" + x.Num + ":" + x.DepotID+","; });
                    }
                    string errmsg = "";
                    result = StockService.AuditPurchase(model.DocID, model.DocType, model.IsOver, details,
                        model.Remark, userid, agentid, clientid, ref errmsg);
                    JsonDictionary["error_msg"] = errmsg;
                }
                else
                {
                    JsonDictionary["error_code"] = -100;
                    JsonDictionary["error_msg"] = "filterPurchase中订单DocID不能为空";
                }
            }
            else
            { 
                JsonDictionary["error_code"] = -100;
                JsonDictionary["error_msg"] = "filterPurchase 参数不能为空";
            }
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
