using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IntFactory.Sdk;
using IntFactory.Sdk.Business;
using CloudSalesBusiness;
using Newtonsoft.Json;

namespace YXERP.Controllers
{
    public class IntFactoryOrderController : BaseController
    {
        //
        // GET: /IntFactoryOrder/

        public ActionResult DownOrder(string id)
        { 
            ViewBag.ClientID = id;
            ViewBag.Items = ClientBusiness.BaseBusiness.GetClientCategorys("", EnumCategoryType.Order);
            ViewBag.Categorys = ClientBusiness.BaseBusiness.GetProcessCategorys(id);
            return View();
        }
        public ActionResult Orders(string id)
        {
            ViewBag.ClientID = id;
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            //ViewBag.Items = ClientBusiness.BaseBusiness.GetClientCategorys("", EnumCategoryType.Order);
            //ViewBag.Categorys = ClientBusiness.BaseBusiness.GetProcessCategorys(id);
            return View();
        }
        public ActionResult OrdersDetail(string orderid,string clientid)
        {
            ViewBag.ClientID = clientid;
            ViewBag.OrderID = orderid;
            var obj= OrderBusiness.BaseBusiness.GetOrderDetailByID(orderid, clientid);
            if (obj.error_code == 0)
            {
                ViewBag.Model = obj.order;
            }
            else
            {
                ViewBag.Model=new OrderEntity();
            }
            return View();
        }
        public JsonResult GetCityByPCode(string cityCode)
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            var list = CommonBusiness.Citys.Where(c => c.PCode == cityCode);
            JsonDictionary.Add("Items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
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

        public JsonResult CreateOrderEDJ(string entity)
        {
            var ord= JsonConvert.DeserializeObject<OrderEntity>(entity);
             
            //1.判断产品是否存ZNGCAddProduct在 与明细 不存在则插入
            CloudSalesEntity.Products pdt=OrderBusiness.BaseBusiness.ZNGCAddProduct(ord,CurrentUser.AgentID,CurrentUser.ClientID,CurrentUser.UserID);
            //2.生成采购单据

            string purid = "";
            //3.生成智能工厂单据
            var result=OrderBusiness.BaseBusiness.CreateDHOrder(ord.orderID, ord.finalPrice, ord.details, ord.clientID, purid);
            if (result.error_code == 0)
            {
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
