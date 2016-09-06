﻿using System;
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
            CloudSalesEntity.Users users = new CloudSalesEntity.Users();
            users.Name = CurrentUser.Name;
            users.MobilePhone = CurrentUser.MobilePhone;
            users.Avatar = CurrentUser.Avatar;
            ViewBag.baseUser = users;
            return View();
        }

    }
}