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
            if (string.IsNullOrEmpty(providerID) && string.IsNullOrEmpty(CurrentUser.CurrentClientID))
            {
                return Redirect("/M/Home/ChooseProvider");
            }

            if (!string.IsNullOrEmpty(providerID)) {
                var c = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(providerID);
                CurrentUser.CurrentClientID = providerID;
                var agent = AgentsBusiness.GetAgentDetail(c.AgentID);
                CurrentUser.CurrentCMClientID = agent.CMClientID;
            }
            ViewBag.baseUser = CurrentUser.Client;

            var client = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(CurrentUser.CurrentClientID);
            ViewBag.Client = client;
            ViewBag.index = 0;
            ViewBag.EDJProviderID = CurrentUser.CurrentClientID;
            ViewBag.providerID = CurrentUser.CurrentCMClientID;
            return View();
        }

        public ActionResult ChooseProvider()
        {
            var providers = ProductsBusiness.BaseBusiness.GetProviders(CurrentUser.ClientID).FindAll(m => !string.IsNullOrEmpty(m.CMClientID) && m.ProviderType == 2);
            if (providers.Count==1)
            {
                return Redirect("/M/Home/Index?providerID=" + providers[0].CMClientID);
            }

            ViewBag.Providers = providers;
            return View();
        }

        public ActionResult Detail(string orderid)
        {
            var obj = IntFactory.Sdk.OrderBusiness.BaseBusiness.GetOrderDetailByID(orderid, CurrentUser.CurrentCMClientID);
            ViewBag.EDJProviderID = CurrentUser.CurrentClientID;
            ViewBag.baseUser = CurrentUser.Client;
            ViewBag.Model = obj.order;

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
