using IntFactory.Sdk; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
namespace YXApp.Controllers
{
    public class MyAccountController : BaseController
    {
        //
        // GET: /MyAccount/

        public ActionResult Index()
        {
            ViewBag.Title = "个人中心";
            return View();
        }

        public ActionResult HistoryOrder()
        {
            ViewBag.Title = "历史订单";
            return View();
        }

        public ActionResult OrderStatusList()
        {
            ViewBag.Title = "订单列表";
            return View();
        }

        public ActionResult OrderDetail(string orderID,string zngcClientID)
        {
            ViewBag.Title = "订单详情";
            OrderResult item = OrderBusiness.BaseBusiness.GetOrderDetailByID(orderID, zngcClientID);
            ViewBag.DetailItem = item;
            return View();
        }

        //创建订单
        public JsonResult CreateOrder(string zngcOrderID,decimal price,string detailsEntity,string zngcClientID)
        {
            JavaScriptSerializer serializer=new JavaScriptSerializer();
            List<ProductDetailEntity> details = serializer.Deserialize<List<ProductDetailEntity>>(detailsEntity);
            string yxOrderID="";
            AddResult result = OrderBusiness.BaseBusiness.CreateDHOrder(zngcOrderID, price, details, zngcClientID, yxOrderID);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
