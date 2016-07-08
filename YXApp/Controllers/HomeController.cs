using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using YunXiaoService;
using IntFactory.Sdk;
namespace YXApp.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

        #region view
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        } 
        #endregion

        #region ajax
        public JsonResult GetCustomerBaseInfo(string customerID, string clientID) 
        {
            var item = CustomerBusiness.BaseBusiness.GetCustomerByID(customerID, clientID);
            JsonDictionary.Add("item", item);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
