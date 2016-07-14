using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXApp.Controllers
{
    public class CustomerController : Controller
    {
        //
        // GET: /Customer/

        public ActionResult List()
        {
            ViewBag.Title = "客户列表";
            return View();
        }

        public ActionResult Detail()
        {
            ViewBag.Title = "客户详情";
            return View();
        }

    }
}
