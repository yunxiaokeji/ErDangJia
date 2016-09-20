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
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(CurrentUser.CurrentStoreID))
            {
                return Redirect("/M/Home/ChooseProvider");
            }

            CloudSalesEntity.Users users= new CloudSalesEntity.Users();
            users.Name = CurrentUser.Client.ContactName;
            users.MobilePhone = CurrentUser.Client.MobilePhone;
            users.CityCode = CurrentUser.Client.CityCode;
            users.Address = CurrentUser.Client.Address;
            ViewBag.baseUser = users;

            var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(CurrentUser.CurrentStoreID);
            ViewBag.Client = client;
            ViewBag.index = 0;
            ViewBag.providerID = CurrentUser.CurrentStoreID;
            return View();
        }

        public ActionResult ChooseProvider()
        {
            ViewBag.Providers = ProductsBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID).FindAll(m => !string.IsNullOrEmpty(m.CMClientID) && m.ProviderType == 2);

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

        public JsonResult SelectStore(string id) {
            CurrentUser.CurrentStoreID = id;
            JsonDictionary.Add("status",true);

            return new JsonResult { 
            Data=JsonDictionary,
            JsonRequestBehavior=JsonRequestBehavior.AllowGet
            };
        }
    }
}
