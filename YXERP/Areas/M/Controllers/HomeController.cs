using CloudSalesBusiness;
using CloudSalesEntity.Manage;
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

            var providerClient = new CloudSalesEntity.Manage.Clients();
            if (!string.IsNullOrEmpty(providerID))
            {
                CurrentUser.CurrentClientID = providerID;
                providerClient = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(providerID);
                var agent = AgentsBusiness.GetAgentDetail(providerClient.AgentID);
                CurrentUser.CurrentCMClientID = agent.CMClientID;
            }
            else 
            {
                providerClient = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(CurrentUser.CurrentClientID);
            }
            
            ViewBag.index = 0;
            ViewBag.EDJProviderID = CurrentUser.CurrentClientID;
            ViewBag.providerID = CurrentUser.CurrentCMClientID;
            ViewBag.ProviderClient = providerClient;
            ViewBag.CurrentClient = CurrentUser.Client;

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

        public ActionResult Detail(string orderid, string clientid)
        {
            var providerClient = new Clients();
            if (!string.IsNullOrEmpty(clientid))
            {
                CurrentUser.CurrentClientID = clientid;
                providerClient = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(clientid);
                var agent = AgentsBusiness.GetAgentDetail(providerClient.AgentID);
                CurrentUser.CurrentCMClientID = agent.CMClientID;
            }
            else {
                providerClient = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(CurrentUser.CurrentClientID);
            }
            var obj = IntFactory.Sdk.OrderBusiness.BaseBusiness.GetOrderDetailByID(orderid, CurrentUser.CurrentCMClientID);
            ViewBag.EDJProviderID = CurrentUser.CurrentClientID;
            ViewBag.Order = obj.order;
            ViewBag.CurrentClient = CurrentUser.Client;
            ViewBag.ProviderClient = providerClient;

            return View();
        }

        public JsonResult SelectStore(string id)
        {
            var providerClient = CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(id);
            CurrentUser.CurrentClientID = id;
            var agent = AgentsBusiness.GetAgentDetail(providerClient.AgentID);
            CurrentUser.CurrentCMClientID = agent.CMClientID;
            JsonDictionary.Add("status",true);

            return new JsonResult { 
                Data=JsonDictionary,
                JsonRequestBehavior=JsonRequestBehavior.AllowGet
            };
        }
    }
}
