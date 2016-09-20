﻿using CloudSalesBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace YXERP.Areas.M.Controllers
{
    public class OrdersController : YXERP.Controllers.BaseController
    {
        //
        // GET: /M/Orders/

        public ActionResult List(string providerID)
        {
            CloudSalesEntity.Users users = new CloudSalesEntity.Users();
            users.Name = CurrentUser.Client.ContactName;
            users.MobilePhone = CurrentUser.Client.MobilePhone;
            users.CityCode = CurrentUser.Client.CityCode;
            users.Address = CurrentUser.Client.Address;
            ViewBag.baseUser = users;
            ViewBag.index = 1;
            ViewBag.providerID = CurrentUser.CurrentStoreID;

            return View();
        }

        public ActionResult Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/M/Orders/List");                
            }
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.Model = model;
            return View();
        }
    }
}
