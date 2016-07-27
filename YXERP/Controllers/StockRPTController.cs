using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CloudSalesBusiness;

namespace YXERP.Controllers
{
    public class StockRPTController : BaseController
    {
        //
        // GET: /StockRPT/

        public ActionResult OrderDetailRPT()
        {
            return View();
        }
        public ActionResult StockDetailRPT()
        {
            return View();
        }


        #region 订单明细统计
        public JsonResult GetOrderDetailRPT(int pageSize, int pageIndex, string keyWords, string begintime, string endtime, string orderBy, string customerid)
        {
            int totalCount = 0, pageCount = 0;
            var list = SalesRPTBusiness.BaseBusiness.GetOrderDetailRPT(pageSize, pageIndex, CurrentUser.ClientID, keyWords, begintime, endtime, orderBy, customerid, ref totalCount, ref pageCount);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult ExportOrderDetailRPT(string keyWords, string begintime, string endtime, string orderBy, string customerid, bool test = false, string model = "", string filleName = "")
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
            var list = SalesRPTBusiness.BaseBusiness.GetaStockDetailReport(pageSize, pageIndex, CurrentUser.ClientID, keyWords, begintime, endtime, ref totalCount, ref pageCount);
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
