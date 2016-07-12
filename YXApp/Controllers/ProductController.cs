using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using IntFactory.Sdk.Business;
using IntFactory.Sdk;
using CloudSalesEntity;
using YunXiaoService;
namespace YXApp.Controllers
{
    public class ProductController : BaseController
    {
        //
        // GET: /Product/

        public ActionResult ProductList(string zngcClientID)
        {
            ViewBag.Title = "样衣中心";
            return View();
        }

        public ActionResult Detail()
        {
            ViewBag.Title = "样衣详情";
            return View();
        }

        public JsonResult GetProductList(string zngcClientID,int pageSize,int pageIndex)
        {
            OrderListResult item = OrderBusiness.BaseBusiness.GetOrdersByYXClientCode(CurrentUser.Client.ClientCode, pageSize, pageIndex, zngcClientID);
            JsonDictionary.Add("item",item);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
