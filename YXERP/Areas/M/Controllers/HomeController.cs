using CloudSalesBusiness;
using IntFactory.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YunXiaoService;

namespace YXERP.Areas.M.Controllers
{
    public class HomeController : YXERP.Controllers.BaseController
    {
        public ActionResult Index(string providerID)
        {
            if (string.IsNullOrEmpty(providerID))
            {
                return Redirect("/M/Home/ChooseProvider");
            }
            ViewBag.providerID = providerID;
   
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
            var obj = ProductService.GetProductByIDForDetails(orderid);
            CloudSalesEntity.Users users = new CloudSalesEntity.Users();
            users.Name = CurrentUser.Client.ContactName;
            users.MobilePhone = CurrentUser.Client.MobilePhone;
            users.CityCode = CurrentUser.Client.CityCode;
            users.Address = CurrentUser.Client.Address;
            ViewBag.baseUser = users;
            ViewBag.Model = obj;
            ViewBag.ClientID = obj.ClientID;

            return View();
        }


    }
}
