
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

        public ActionResult MyPurchase()
        {
            ViewBag.Title = "我的采购";
            ViewBag.Type = (int)EnumSearchType.Myself;
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            return View("Purchases");
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
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.ClientID);
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
            ViewBag.Model = StockBusiness.GetStorageDetail(id, CurrentUser.ClientID);
            return View();
        }

        #region Ajax

        #region 供应商

        

        #endregion

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

        /// <summary>
        /// 获取我的采购单
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult GetPurchases(string keyWords, int pageIndex, int totalCount, int status = -1, int type = 1, string begintime = "", string endtime = "", string wareid = "", string providerid = "")
        {
            int pageCount = 0;
            List<StorageDoc> list = StockBusiness.GetStorageDocList(type == 3 ? string.Empty : CurrentUser.UserID, EnumDocType.RK, (EnumDocStatus)status, keyWords, begintime, endtime, wareid, providerid, PageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        ///  删除单据
        /// </summary>
        /// <param name="docid"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 作废单据
        /// </summary>
        /// <param name="docid"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 更换入库仓库和货位
        /// </summary>
        /// <param name="autoid"></param>
        /// <param name="wareid"></param>
        /// <param name="depotid"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 审核上架
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public JsonResult AuditPurchase(string ids)
        {
            bool bl = new StockBusiness().AuditStorageIn(ids, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取单据日志
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="pageIndex"></param>
        /// <param name="totalCount"></param>
        /// <param name="status"></param>
        /// <returns></returns>
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

        #endregion

    }
}
