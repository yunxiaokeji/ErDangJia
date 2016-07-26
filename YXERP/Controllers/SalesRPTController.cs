using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CloudSalesBusiness;
using CloudSalesEntity;

namespace YXERP.Controllers
{
    public class SalesRPTController : BaseController
    {
        //
        // GET: /SalesRPT/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserOrderReport()
        {
            ViewBag.Types = SystemBusiness.BaseBusiness.GetOrderTypes(CurrentUser.AgentID, CurrentUser.ClientID);
            return View();
        }

        public ActionResult OrderFunnelReport()
        {
            return View();
        }

        public ActionResult OrderMapReport()
        {
            //ViewBag.Types = SystemBusiness.BaseBusiness.GetOrderTypes(CurrentUser.AgentID, CurrentUser.ClientID);
            return View();
        }

        public ActionResult OrderDetailRPT()
        {
            return View();
        }

        #region 销售订单统计

        public JsonResult GetUserOrders(int type,string userid, string teamid, string beginTime, string endTime,string ordertype)
        {

            var list = SalesRPTBusiness.BaseBusiness.GetUserOrders(userid, teamid, beginTime, endTime, CurrentUser.AgentID, CurrentUser.ClientID, ordertype);

            if (type == 2)
            {
                Dictionary<string, List<TypeOrderEntity>> customerlist =  new Dictionary<string, List<TypeOrderEntity>>(); 
                
                List<TypeOrderEntity> listcustomer = new List<TypeOrderEntity>();
                list.ForEach(x => listcustomer.AddRange(x.ChildItems));
                customerlist.Add("TotalList", listcustomer.OrderByDescending(x =>  x.TCount).Take(15).ToList());
                customerlist.Add("MoneyList", listcustomer.OrderByDescending(x =>  x.TMoney).Take(15).ToList());
                JsonDictionary.Add("items", customerlist);
            }
            else
            {
                JsonDictionary.Add("items", list);
            }
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region Ajax 订单分布统计

        public JsonResult GetOrderMapReport(int type, string beginTime, string endTime, string UserID, string TeamID)
        {

            var list = SalesRPTBusiness.BaseBusiness.GetOrderMapReport(type, beginTime, endTime, UserID, TeamID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            if (type == 1)
                list.Sort((g1, g2) => { return Comparer<int>.Default.Compare(g2.value, g1.value); });
            else if (type == 3)
                list.Sort((g1, g2) => { return Comparer<int>.Default.Compare(int.Parse(g2.name), int.Parse(g1.name)); });

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 销售转化率

        public JsonResult GetOpportunityStageRate(string beginTime, string endTime, string UserID, string TeamID)
        {
            decimal forecast = 0;
            var list = SalesRPTBusiness.BaseBusiness.GetOpportunityStageRate(beginTime, endTime, UserID, TeamID, CurrentUser.AgentID, CurrentUser.ClientID, out forecast);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("forecast", forecast);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUserOpportunitys(string userid, string teamid, string beginTime, string endTime)
        {

            var list = SalesRPTBusiness.BaseBusiness.GetUserOpportunitys(userid, teamid, beginTime, endTime, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 订单明细统计
        public JsonResult GetOrderDetailRPT(int pageSize, int pageIndex, string keyWords, string begintime, string endtime, string orderBy,string customerid)
        {
            int totalCount = 0, pageCount = 0;
            var list = SalesRPTBusiness.BaseBusiness.GetOrderDetailRPT(pageSize, pageIndex, CurrentUser.ClientID, keyWords, begintime, endtime,orderBy, customerid, ref totalCount, ref pageCount);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
         
        public ActionResult ExportOrderDetailRPT(string keyWords, string begintime, string endtime, string orderBy,string customerid, bool test=false, string model = "", string filleName = "")
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, ExcelFormatter> dic = new Dictionary<string, ExcelFormatter>();
            Dictionary<string, ExcelModel> listColumn = new Dictionary<string, ExcelModel>();
            var modelname = "orderdetailrpt";
            listColumn = GetColumnForJson(modelname, ref dic, !string.IsNullOrEmpty(model) ? model : "Item", test ? "testexport" : "export", CurrentUser.ClientID);
            var excelWriter = new ExcelWriter();
            foreach (var key in listColumn)
            {
                excelWriter.Map(key.Key, key.Value.Title);
            }
            byte[] buffer;
            DataTable dt = new DataTable();
            //模版导出
            if (test)
            {
                DataRow dr = dt.NewRow();
                foreach (var key in listColumn)
                {
                    DataColumn dc1 = new DataColumn(key.Key, Type.GetType("System.String"));
                    dt.Columns.Add(dc1);
                    dr[key.Key] = key.Value.DefaultText;
                }
                dt.Rows.Add(dr);
            }
            else
            {
                int totalCount = 0;
                dt = SalesRPTBusiness.BaseBusiness.GetOrderDetailRPTBase(int.MaxValue, 1, CurrentUser.ClientID, keyWords, begintime, endtime, orderBy, customerid, ref totalCount, ref totalCount);
            }
            buffer = excelWriter.Write(dt, dic, "");
            var fileName = CurrentUser.Client.CompanyName + filleName + (test ? "导入模版" : "") + DateTime.Now.ToString("yyyyMMdd");
            if (!Request.ServerVariables["http_user_agent"].ToLower().Contains("firefox"))
                fileName = HttpUtility.UrlEncode(fileName);
            this.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");
            return File(buffer, "application/ms-excel");
        }

        public JsonResult GetStockDetailRPT(int pageSize, int pageIndex, string keyWords, string begintime, string endtime)
        {
            int totalCount = 0, pageCount = 0;
            var list = SalesRPTBusiness.BaseBusiness.GetaStockDetailReport(pageSize, pageIndex, CurrentUser.ClientID, keyWords, begintime, endtime,  ref totalCount, ref pageCount);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult ExportStockDetailRPT(string keyWords, string begintime, string endtime, bool test = false, string model = "", string filleName = "")
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, ExcelFormatter> dic = new Dictionary<string, ExcelFormatter>();
            Dictionary<string, ExcelModel> listColumn = new Dictionary<string, ExcelModel>();
            var modelname = "stockdetailrpt";
            listColumn = GetColumnForJson(modelname, ref dic, !string.IsNullOrEmpty(model) ? model : "Item", test ? "testexport" : "export", CurrentUser.ClientID);
            var excelWriter = new ExcelWriter();
            foreach (var key in listColumn)
            {
                excelWriter.Map(key.Key, key.Value.Title);
            }
            byte[] buffer;
            DataTable dt = new DataTable();
            //模版导出
            if (test)
            {
                DataRow dr = dt.NewRow();
                foreach (var key in listColumn)
                {
                    DataColumn dc1 = new DataColumn(key.Key, Type.GetType("System.String"));
                    dt.Columns.Add(dc1);
                    dr[key.Key] = key.Value.DefaultText;
                }
                dt.Rows.Add(dr);
            }
            else
            {
                int totalCount = 0;
                dt = SalesRPTBusiness.BaseBusiness.GetaStockDetailReportBase(int.MaxValue, 1, CurrentUser.ClientID, keyWords, begintime, endtime, ref totalCount, ref totalCount);
            }
            buffer = excelWriter.Write(dt, dic, "");
            var fileName = CurrentUser.Client.CompanyName + filleName + (test ? "导入模版" : "") + DateTime.Now.ToString("yyyyMMdd");
            if (!Request.ServerVariables["http_user_agent"].ToLower().Contains("firefox"))
                fileName = HttpUtility.UrlEncode(fileName);
            this.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");
            return File(buffer, "application/ms-excel");
        }


        #endregion
    }
}
