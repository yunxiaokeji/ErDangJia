using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXApp.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/

        public ActionResult ProductList()
        {
            ViewBag.Title = "样衣中心";
            return View();
        }

        public ActionResult Detail()
        {
            ViewBag.Title = "样衣详情";
            return View();
        }

    }
}
