using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXApp.Controllers
{
    public class OrderController : Controller
    {
        //
        // GET: /Order/

        public ActionResult OrderList()
        {
            ViewBag.Title = "订单列表";
            return View();
        }

    }
}
