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
            if (string.IsNullOrEmpty(CurrentUser.CurrentClientID))
            {
                return Redirect("/M/Home/ChooseProvider");
            }

            ViewBag.baseUser = CurrentUser.Client;

            var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(CurrentUser.CurrentClientID);
            ViewBag.Client = client;
            ViewBag.index = 0;
            ViewBag.providerID = CurrentUser.CurrentCMClientID;
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
            ViewBag.baseUser = CurrentUser.Client;
            ViewBag.Model = obj;
            ViewBag.ClientID = obj.ClientID;

            return View();
        }

        public JsonResult SelectStore(string id) {
            var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(id);
            CurrentUser.CurrentClientID = id;
            var agent = AgentsBusiness.GetAgentDetail(client.AgentID);
            CurrentUser.CurrentCMClientID = agent.CMClientID;
            JsonDictionary.Add("status",true);

            return new JsonResult { 
                Data=JsonDictionary,
                JsonRequestBehavior=JsonRequestBehavior.AllowGet
            };
        }
    }
}
