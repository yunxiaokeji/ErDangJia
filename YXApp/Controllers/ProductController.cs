using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactory.Sdk.Business;
using IntFactory.Sdk;
namespace YXApp.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/
        Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

        public ActionResult ProductList(string yxClientCode,string zngcClientID)
        {
            yxClientCode = "H28L56GK";
            zngcClientID = "a89cbb94-e32b-4f99-bab9-2db1d9cff607";
            ViewBag.yxClientCode = yxClientCode;
            ViewBag.zngcClientID = zngcClientID;
            ViewBag.Title = "样衣中心";
            return View();
        }

        public ActionResult Detail()
        {
            ViewBag.Title = "样衣详情";
            return View();
        }

        public JsonResult GetProductList(string yxClientCode, string zngcClientID,int pageSize,int pageIndex)
        {
            OrderListResult item = OrderBusiness.BaseBusiness.GetOrdersByYXClientCode(yxClientCode, pageSize, pageIndex, zngcClientID);
            JsonDictionary.Add("item",item);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
