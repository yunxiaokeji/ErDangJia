using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesBusiness;
using System.Web.Script.Serialization;
using CloudSalesEntity;
using CloudSalesBusiness.Manage;
namespace YXManage.Controllers
{
    [YXManage.Common.UserAuthorize]
    public class ReportController :BaseController
    {
        //
        // GET: /Report/

        public ActionResult AgentActionReport()
        {
            return View();
        }

        public JsonResult GetAgentActionReports(string keyword,string startDate,string endDate)
        {
            var list = AgentsBusiness.GetAgentActionReport(keyword, startDate, endDate);
            JsonDictionary.Add("Items", list);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult GetClientVitalityReport(int dateType, string beginTime, string endTime, string clientId)
        {
            var list = ClientBusiness.GetClientsVitalityReport(dateType, beginTime, endTime, clientId);
            JsonDictionary.Add("items", list);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
