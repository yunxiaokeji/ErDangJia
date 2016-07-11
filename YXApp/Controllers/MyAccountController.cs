using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXApp.Controllers
{
    public class MyAccountController : Controller
    {
        //
        // GET: /MyAccount/

        public ActionResult Index()
        {
            ViewBag.Title = "个人中心";
            return View();
        }

    }
}
