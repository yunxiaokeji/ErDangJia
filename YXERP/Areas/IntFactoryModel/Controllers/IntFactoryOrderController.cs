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

namespace YXERP.Areas.IntFactoryModel.Controllers
{
    public class IntFactoryOrderController : Controller
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
        public ActionResult Orders(string id,int status=0)
        {
            ViewBag.ClientID = id;
            ViewBag.LogStatus = status;
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders("-1"); 
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
            //ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders("-1");
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
            var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(clientid);
            ViewBag.ClientID = clientid;
            ViewBag.OrderID = orderid;
            ViewBag.CityCode = client==null?"":client.CityCode;
            ViewBag.ContactName = client == null ? "" : client.ContactName;
            ViewBag.MobilePhone = client == null ? "" : client.MobilePhone;
            ViewBag.Address = client == null ? "" : client.Address;
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
        public JsonResult GetAllCateGory()
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            var result = ClientBusiness.BaseBusiness.GetAllCategory(1,EnumCategoryType.Order);
          
            JsonDictionary.Add("items", result); 
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult GetProductList(string clientid, string keyWords, int pageSize, int pageIndex)
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            OrderListResult item = OrderBusiness.BaseBusiness.GetOrdersByYXClientCode("", pageSize, pageIndex, clientid, keyWords);
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
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
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
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
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
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            var ord= JsonConvert.DeserializeObject<OrderEntity>(entity);
            //1.判断产品是否存ZNGCAddProduct在 与明细 不存在则插入
            string dids = "";
            CloudSalesEntity.Users user = (CloudSalesEntity.Users) Session["ClientManager"];
            string pid = OrderBusiness.BaseBusiness.ZNGCAddProduct(ord, user.AgentID, user.ClientID, user.UserID, ref dids);
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
            string purid = StockBusiness.AddPurchaseDoc(pid, dids, ord.clientID, totalFee, "", "", 2, user.UserID,
                user.AgentID, user.ClientID);
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
            ord.details.ForEach(x =>
            { 
                x.remark = x.remark.Replace("[", "【").Replace("]", "】");
            });
            var result = OrderBusiness.BaseBusiness.CreateDHOrder(ord.orderID, ord.finalPrice, ord.details,
                    ord.clientID, purid, user.ClientID,ord.personName, ord.mobileTele, ord.cityCode, ord.address);
            if (!string.IsNullOrEmpty(result.id))
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
