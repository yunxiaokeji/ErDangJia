using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using CloudSalesEntity;
using YXERP.Models;
using CloudSalesBusiness;
using CloudSalesEnum;

namespace YXERP.Controllers
{
    public class OrdersController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyOrder()
        {
            ViewBag.Title = "我的订单";
            ViewBag.Type = (int)EnumSearchType.Myself;
            return View("Orders");
        }

        public ActionResult BranchOrder()
        {
            ViewBag.Title = "下属订单";
            ViewBag.Type = (int)EnumSearchType.Branch;
            return View("Orders");
        }

        public ActionResult Orders()
        {
            ViewBag.Title = "所有订单";
            ViewBag.Type = (int)EnumSearchType.All;
            return View("Orders");
        }

        public ActionResult ChooseProducts(string id, string pids = "")
        {
            ViewBag.Type = (int)EnumDocType.Order;
            ViewBag.GUID = id;
            ViewBag.Pids = pids;
            ViewBag.Title = "选择产品";
            return View("FilterProducts");
        }

        public ActionResult Detail(string id)
        {
            var model = OrdersBusiness.BaseBusiness.GetOrderByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.OrderID))
            {
                return Redirect("/Orders/MyOrder");
            }
            string pids = "";
            model.Details.ForEach(x => { if (pids.IndexOf(x.ProductDetailID) == -1) pids += x.ProductDetailID + ","; });
            ViewBag.Pids = pids;
            ViewBag.Model = model;
            return View();
        }

        public ActionResult ApplyReturn(string id)
        {
            var model = OrdersBusiness.BaseBusiness.GetOrderByID(id, CurrentUser.AgentID, CurrentUser.ClientID);

            if (model == null || string.IsNullOrEmpty(model.OrderID))
            {
                return Redirect("/Orders/MyOrder");
            }

            if (model.Status != 2 || model.SendStatus == 0 || model.ReturnStatus == 1)
            {
                return Redirect("/Orders/Detail/" + id);
            }

            ViewBag.Model = model;
            return View();
        }

        #region Ajax

        public JsonResult GetOrders(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterOrders model = serializer.Deserialize<FilterOrders>(filter);
            int totalCount = 0;
            int pageCount = 0;

            var list = OrdersBusiness.BaseBusiness.GetOrders(model.SearchType, model.TypeID, model.Status, model.PayStatus, model.InvoiceStatus, model.ReturnStatus, model.UserID, model.TeamID, model.AgentID, model.BeginTime, model.EndTime, 
                                                             model.Keywords, model.OrderBy, model.PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderByID(string orderid)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var model = OrdersBusiness.BaseBusiness.GetOrderByID(orderid, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult Create(string customerid, string typeid, string name, string mobile, string cityCode, string address, string remark)
        {
            string orderid = OrdersBusiness.BaseBusiness.CreateOrder(customerid, typeid, name, mobile, cityCode, address, remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("id", orderid);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrdersByCustomerID(string customerid, int pagesize, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = OrdersBusiness.BaseBusiness.GetOrdersByCustomerID(customerid, pagesize, pageindex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderOwner(string ids, string userid)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && OrdersBusiness.BaseBusiness.UpdateOrderOwner(id, userid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }

            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult EditOrder(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderEntity model = serializer.Deserialize<OrderEntity>(entity);


            var bl = OrdersBusiness.BaseBusiness.EditOrder(model.OrderID, model.PersonName, model.MobileTele, model.CityCode, model.Address, model.PostalCode, model.TypeID, model.ExpressType, model.Remark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderProductQuantity(string orderid, string productid, string name, int quantity)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderProductQuantity(orderid, productid, name, quantity, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateOrderProductPrice(string orderid, string productid, string name, decimal price)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateOrderProductPrice(orderid, productid, name, price, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteOrder(string orderid)
        {
            var bl = OrdersBusiness.BaseBusiness.DeleteOrder(orderid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult EffectiveOrder(string orderid)
        {
            int result = 0;
            var bl = OrdersBusiness.BaseBusiness.EffectiveOrder(orderid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            if (bl)
            {
                //var order = OrdersBusiness.BaseBusiness.GetOrderByIDFor(orderid, CurrentUser.AgentID, CurrentUser.ClientID);
                //if (order.SourceType == 1)
                //{
                //    IntFactory.Sdk.AddResult addResult = IntFactory.Sdk.OrderBusiness.BaseBusiness.CreateDHOrder(,null, CurrentUser.ClientID, CurrentUser.UserID);
                //}
            }
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ApplyReturnOrder(string orderid)
        {
            int result = 0;
            var bl = OrdersBusiness.BaseBusiness.ApplyReturnOrder(orderid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateReturnQuantity(string orderid, string autoid, string name, int quantity)
        {
            var bl = OrdersBusiness.BaseBusiness.UpdateReturnQuantity(orderid, autoid, name, quantity, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ApplyReturnProduct(string orderid)
        {
            int result = 0;
            var bl = OrdersBusiness.BaseBusiness.ApplyReturnProduct(orderid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
