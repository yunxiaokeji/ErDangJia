using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXApp.Controllers
{
    public class MyAccountController : BaseController
    {
        //
        // GET: /MyAccount/

        public ActionResult Index()
        {
            ViewBag.Title = "个人中心";
            return View();
        }

        public ActionResult HistoryOrder()
        {
            ViewBag.Title = "历史订单";
            return View();
        }

        public ActionResult OrderDetail()
        {
            ViewBag.Title = "订单详情";
            return View();
        }
    }
}
