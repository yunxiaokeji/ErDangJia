﻿using System;
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

        //public JsonResult GetAgentActionReports(string keyword,string startDate,string endDate)
        //{
        //    var list = AgentsBusiness.GetAgentActionReport(keyword, startDate, endDate);
        //    JsonDictionary.Add("Items", list);

        //    return new JsonResult()
        //    {
        //        Data = JsonDictionary,
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //    };
        //}
        public JsonResult GetAgentActionReports(string keyword, string startDate, string endDate, int type, int pageIndex, string orderBy = "SUM(a.CustomerCount) desc")
        {
            int totalCount = 0, pageCount = 0;
            var list = AgentsBusiness.GetAgentActionReport(keyword, startDate, endDate, type, orderBy, PageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
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
