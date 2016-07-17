
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using CloudSalesEnum;
using CloudSalesBusiness;
using CloudSalesEntity;

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

        public ActionResult Purchases()
        {
            ViewBag.Title = "所有采购";
            ViewBag.Type = (int)EnumSearchType.All;
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
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
            var wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID).Where(m => m.Status == 1).ToList();
            var list = ShoppingCartBusiness.GetShoppingCart(EnumDocType.RK, CurrentUser.UserID, CurrentUser.UserID, CurrentUser.ClientID);
            Dictionary<string, string> providers = new Dictionary<string, string>();
            foreach (var model in list)
            {
                if (!providers.ContainsKey(model.ProviderID))
                {
                    providers.Add(model.ProviderID, model.ProviderName);
                }
            }
            ViewBag.wares = wares;
            ViewBag.items = list;
            ViewBag.guid = CurrentUser.UserID;
            ViewBag.providers = providers;
            return View();
        }

        public ActionResult ChooseProducts(string id)
        {
            ViewBag.Type = (int)EnumDocType.RK;
            ViewBag.GUID = CurrentUser.UserID;
            ViewBag.Title = "选择采购产品";
            return View("FilterProducts");
        }

        #region Ajax

        public JsonResult SubmitPurchase(string wareid, string ids, string remark)
        {
            bool bl = false;
            if (!string.IsNullOrEmpty(wareid) && !string.IsNullOrEmpty(ids))
            {
                ids = ids.Substring(0, ids.Length - 1);
                bl = StockBusiness.CreateStorageDoc(wareid, ids, remark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            
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

        public JsonResult GetPurchasesDetails(string docid)
        {
            List<StorageDoc> list = StockBusiness.GetStorageDocDetails(docid, CurrentUser.AgentID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
