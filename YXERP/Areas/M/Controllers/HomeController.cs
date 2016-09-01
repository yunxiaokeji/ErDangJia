using CloudSalesBusiness;
using IntFactory.Sdk;
using IntFactory.Sdk.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Areas.M.Controllers
{
    public class HomeController : YXERP.Controllers.BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID); 

            return View();
        }

        public ActionResult Detail(string orderid, string clientid)
        {
            var obj = OrderBusiness.BaseBusiness.GetOrderDetailByID(orderid, clientid);
            if (obj.error_code == 0)
            {
                ViewBag.Order = obj.order;
                ViewBag.ClientID = obj.order.clientID;
            }
            else
            {
                ViewBag.Model = new OrderEntity();
            }

            return View();
        }


    }
}
