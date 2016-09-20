using CloudSalesBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Areas.M.Controllers
{
    public class AccountController : YXERP.Controllers.BaseController
    {
        //
        // GET: /M/Account/

        public ActionResult Index()
        {
            ViewBag.providerID = CurrentUser.CurrentStoreID; ;
            ViewBag.baseUser = CurrentUser;
            ViewBag.index = 2;

            return View();
        }

    }
}
