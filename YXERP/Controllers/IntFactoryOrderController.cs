using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IntFactory.Sdk;
using IntFactory.Sdk.Business;
using CloudSalesBusiness;
using CloudSalesEnum;
using Newtonsoft.Json;

namespace YXERP.Controllers
{
    public class IntFactoryOrderController : BaseController
    {
        //
        // GET: /IntFactoryOrder/
        /// <summary>
        /// 需求单下载
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DownOrder(string id)
        { 
            ViewBag.ClientID = id;
            ViewBag.Items = ClientBusiness.BaseBusiness.GetClientCategorys("", EnumCategoryType.Order);
            ViewBag.Categorys = ClientBusiness.BaseBusiness.GetProcessCategorys(id);
            return View();
        }

        /// <summary>
        /// 打样单列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Orders(string id)
        {
            if (string.IsNullOrEmpty(CurrentUser.Client.OtherSysID))
            {
                Response.Write("<script>alert('您的账号暂未授权到智能工厂,不能使用在线下单功能，请授权后再使用.');location.href='/Default/Index';</script>");
                Response.End(); 
            }
            ViewBag.ClientID = id;
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID); 
            return View();
        }

        /// <summary>
        /// 订购订单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Type = (int)EnumDocType.RK;
            ViewBag.ZNGCID = "";
            ViewBag.SouceType = 2;
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            return View();
        }

        /// <summary>
        /// 打样单详情
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public ActionResult OrdersDetail(string orderid,string clientid)
        {
            ViewBag.ClientID = clientid;
            ViewBag.OrderID = orderid;
            var obj= OrderBusiness.BaseBusiness.GetOrderDetailByID(orderid, clientid);
            if (obj.error_code == 0)
            {
                ViewBag.Model = obj.order;
                ViewBag.ClientID = obj.order.clientID;
            }
            else
            {
                ViewBag.Model=new OrderEntity();
            }
            return View();
        }

        public JsonResult CreateOrder(string entity,string clientid,string userid="")
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            AddResult result = OrderBusiness.BaseBusiness.CreateOrder(entity, clientid, userid);
            JsonDictionary.Add("id", result.id);
            JsonDictionary.Add("err_msg", result.error_message);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductList(string clientid, int pageSize, int pageIndex)
        {
            OrderListResult item = OrderBusiness.BaseBusiness.GetOrdersByYXClientCode(CurrentUser.Client.ClientCode, pageSize, pageIndex, clientid);
            JsonDictionary.Add("items", item.orders);
            JsonDictionary.Add("totalCount", item.totalCount);
            JsonDictionary.Add("pageCount", item.pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetZNGCCategorys(string categoryid)
        { 
            var obj = ClientBusiness.BaseBusiness.GetCategoryByID(categoryid);
            JsonDictionary.Add("items", obj); 
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderDetailByID(string clientid, string orderid, string categoryid)
        {
            var obj = OrderBusiness.BaseBusiness.GetOrderDetailByID(orderid, clientid);
            if (obj.error_code == 0)
            {
                JsonDictionary.Add("items", obj.order); 
            }
            else
            {
                JsonDictionary.Add("items", ""); 
                JsonDictionary.Add("errMsg", obj.error_message);
            }
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CreateOrderEDJ(string entity,decimal totalFee=0)
        {
            var ord= JsonConvert.DeserializeObject<OrderEntity>(entity);
             
            //1.判断产品是否存ZNGCAddProduct在 与明细 不存在则插入
            string dids = "";
             
            string pid=OrderBusiness.BaseBusiness.ZNGCAddProduct(ord,CurrentUser.AgentID,CurrentUser.ClientID,CurrentUser.UserID,ref dids);
            if (string.IsNullOrEmpty(pid) || string.IsNullOrEmpty(dids))
            {
                JsonDictionary.Add("result", 0);
                JsonDictionary.Add("errMsg", "获取产品失败，请稍后重试");
                return new JsonResult
                {
                    Data = JsonDictionary,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            } 
            //2.生成采购单据 
            string purid = StockBusiness.AddPurchaseDoc(pid, dids, ord.clientID, totalFee, "", "", 2,CurrentUser.UserID,
                CurrentUser.AgentID, CurrentUser.ClientID);
            if (string.IsNullOrEmpty(purid))
            {
                JsonDictionary.Add("result", 0);
                JsonDictionary.Add("errMsg", "采购单生成失败，请稍后重试");
                return new JsonResult
                {
                    Data = JsonDictionary,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            JsonDictionary.Add("PurchaseID", purid);
            //3.生成智能工厂单据
            var result = OrderBusiness.BaseBusiness.CreateDHOrder(ord.orderID, ord.finalPrice, ord.details,
                    ord.clientID, purid);
            if (result.error_code == 0)
            {
                JsonDictionary.Add("OtherOrderID", result.id);
                JsonDictionary.Add("result", 1);
            } 
            else
            {
                JsonDictionary.Add("result", 0);
                JsonDictionary.Add("errMsg", result.error_message);
            }
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
