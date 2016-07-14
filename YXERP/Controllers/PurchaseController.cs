﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using CloudSalesEnum;
using CloudSalesBusiness;
using CloudSalesEntity;
using System.Data;

namespace YXERP.Controllers
{
    public class PurchaseController : BaseController
    {
        //
        // GET: /Purchase/

        public ActionResult Index()
        {
            return View("Purchase");
        }
        

        /// <summary>
        /// 我的采购
        /// </summary>
        /// <returns></returns>
        public ActionResult Purchase()
        {
            ViewBag.Type = (int)EnumDocType.RK;
            ViewBag.Title = "采购入库";
            return View("FilterProducts");
        }

        public ActionResult Purchases()
        {
            ViewBag.Title = "所有采购";
            ViewBag.Type = (int)EnumSearchType.All;
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID).Where(m => m.Status == 1).ToList();
            return View("Purchases");
        }

        /// <summary>
        /// 我的采购详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DocDetail(string id)
        {
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.AgentID, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID))
            {
                return Redirect("/Purchase/MyPurchase");
            }
            ViewBag.Model = model;
            return View();
        }

        /// <summary>
        /// 采购单确认页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmPurchase(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Purchase/MyPurchase");
            }
            var ware = SystemBusiness.BaseBusiness.GetWareByID(id, CurrentUser.ClientID);
            if (ware == null || string.IsNullOrEmpty(ware.WareID))
            {
                return Redirect("/Purchase/MyPurchase");
            }
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            ViewBag.Ware = ware;
            ViewBag.Items = ShoppingCartBusiness.GetShoppingCart(EnumDocType.RK, ware.WareID, CurrentUser.UserID);
            return View();
        }
        /// <summary>
        /// 采购审核页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AuditDetail(string id)
        {
            ViewBag.Model = StockBusiness.GetStorageDetail(id, CurrentUser.AgentID, CurrentUser.ClientID);
            return View();
        }

        #region Ajax

        public JsonResult SubmitPurchase(string wareid, string providerid, string remark)
        {
            var bl = StockBusiness.CreateStorageDoc(wareid, providerid, remark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetPurchases(string keyWords, int pageIndex, int totalCount, int status = -1, int type = 1, string begintime = "", string endtime = "", string wareid = "", string providerid = "")
        {
            int pageCount = 0;
            List<StorageDoc> list = StockBusiness.GetPurchases(type == 3 ? string.Empty : CurrentUser.UserID, (EnumDocStatus)status, keyWords, begintime, endtime, wareid, providerid, PageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeletePurchase(string docid)
        {
            var bl = new StockBusiness().DeleteDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InvalidPurchase(string docid)
        {
            var bl = new StockBusiness().InvalidDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateStorageDetailWare(string docid,string autoid, string wareid, string depotid)
        {
            var bl = new StockBusiness().UpdateStorageDetailWare(docid, autoid, wareid, depotid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AuditPurchase(string docid, int doctype, int isover, string details, string remark)
        {
            int result = 0;
            string errinto = "";
            bool bl = new StockBusiness().AuditStorageIn(docid, doctype, isover, details, remark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, ref result, ref errinto);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetStorageDocLog(string docid, int pageIndex, int totalCount)
        {
            int pageCount = 0;
            List<StorageDocAction> list = StockBusiness.GetStorageDocAction(docid, 10, pageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult ExportFromPurchases(string keyWords, int status = -1, int type = 1, int doctype=1,string begintime = "", string endtime = "", string wareid = "", string providerid = "", bool test=false, string model = "", string filleName = "")
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, ExcelFormatter> dic = new Dictionary<string, ExcelFormatter>();
            Dictionary<string, ExcelModel> listColumn = new Dictionary<string, ExcelModel>();
            var modelname = "purchase";
            switch (doctype)
            {
                case 4:
                case 3:
                    modelname = "overflow";
                    break;
            }
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
                dt = StockBusiness.GetPurchasesByExcel(type >=3 ? string.Empty : CurrentUser.UserID, status, keyWords, begintime, endtime, wareid, providerid, doctype, CurrentUser.ClientID);
                if (!dt.Columns.Contains("statusstr"))
                {
                    dt.Columns.Add("statusstr", Type.GetType("System.String"));
                }
                if (!dt.Columns.Contains("createusername"))
                {
                    dt.Columns.Add("createusername", Type.GetType("System.String"));
                }
                if (!dt.Columns.Contains("wareid"))
                {
                    dt.Columns.Add("wareid", Type.GetType("System.String"));
                }
                foreach (DataRow drRow in dt.Rows)
                {
                    var user = OrganizationBusiness.GetUserByUserID(drRow["CreateUserID"].ToString(), CurrentUser.AgentID);
                    drRow["createusername"]=user!=null?user.Name:"";
                    var house=SystemBusiness.BaseBusiness.GetWareByID(drRow["wareid"].ToString(), CurrentUser.ClientID);
                    drRow["wareid"] = house != null ? house.Name : "";
                    drRow["statusstr"] = StockBusiness.GetDocStatusStr(Convert.ToInt32(drRow["doctype"]), Convert.ToInt32(drRow["status"]));
                }  
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
