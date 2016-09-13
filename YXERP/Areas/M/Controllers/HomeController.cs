using CloudSalesBusiness;
using IntFactory.Sdk;
using IntFactory.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Areas.M.Controllers
{
    public class HomeController : YXERP.Controllers.BaseController
    {
        public ActionResult Index(string providerID, string providerName)
        {
            if (string.IsNullOrEmpty(providerID))
            {
                return Redirect("/M/Home/ChooseProvider");
            }
            ViewBag.providerID = providerID;
            ViewBag.providerName = string.IsNullOrEmpty(providerName) ? "--" : providerName;
            
            CloudSalesEntity.Users users = new CloudSalesEntity.Users();
            users.Name = CurrentUser.Client.ContactName;
            users.MobilePhone = CurrentUser.Client.MobilePhone;
            users.CityCode = CurrentUser.Client.CityCode;
            users.Address = CurrentUser.Client.Address;
            ViewBag.baseUser = users;
            return View();
        }

        public ActionResult ChooseProvider()
        {
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID);
            return View();
        }

        public ActionResult Detail(string orderid, string clientid)
        {
            var obj = OrderBusiness.BaseBusiness.GetOrderDetailByID(orderid, clientid);
            CloudSalesEntity.Users users = new CloudSalesEntity.Users();
            users.Name = CurrentUser.Client.ContactName;
            users.MobilePhone = CurrentUser.Client.MobilePhone;
            users.CityCode = CurrentUser.Client.CityCode;
            users.Address = CurrentUser.Client.Address;
            ViewBag.baseUser = users;
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
