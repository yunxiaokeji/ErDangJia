using CloudSalesBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Areas.M.Controllers
{
    public class OrdersController : YXERP.Controllers.BaseController
    {
        //
        // GET: /M/Orders/

        public ActionResult List()
        {
            CloudSalesEntity.Users users = new CloudSalesEntity.Users();
            users.Name = CurrentUser.Client.ContactName;
            users.MobilePhone = CurrentUser.Client.MobilePhone;
            users.CityCode = CurrentUser.Client.CityCode;
            users.Address = CurrentUser.Client.Address;
            ViewBag.baseUser = users;
            return View();
        }

        public ActionResult Detail(string id)
        {
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.AgentID, CurrentUser.ClientID);
            if (string.IsNullOrEmpty(id) || model == null)
            {
                return Redirect("List");
            }
            ViewBag.Model = model;
            return View();
        }

    }
}
