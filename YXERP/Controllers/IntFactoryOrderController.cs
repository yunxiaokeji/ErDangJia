using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IntFactory.Sdk;
using IntFactory.Sdk.Business;
using CloudSalesBusiness;

namespace YXERP.Controllers
{
    public class IntFactoryOrderController : Controller
    {
        //
        // GET: /IntFactoryOrder/

        public ActionResult DownOrder(string id)
        {
           // ViewBag.Model = ClientBusiness.BaseBusiness.GetClientInfo(clientID);
            ViewBag.ClientID = id;
            ViewBag.Items = ClientBusiness.BaseBusiness.GetClientCategorys("", EnumCategoryType.Order);
            ViewBag.Categorys = ClientBusiness.BaseBusiness.GetProcessCategorys(id);
            return View();
        }
       
        public JsonResult GetCityByPCode(string cityCode)
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            var list = CommonBusiness.Citys.Where(c => c.PCode == cityCode);
            JsonDictionary.Add("Items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult CreateOrder(string entity,string clientid,string userid="")
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            AddResult result = OrderBusiness.BaseBusiness.CreateOrder(entity, clientid, userid);
            JsonDictionary.Add("id", result.id);
            JsonDictionary.Add("err_msg", result.error_message);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
