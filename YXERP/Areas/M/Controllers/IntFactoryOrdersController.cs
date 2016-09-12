using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudSalesEntity;
using IntFactory.Sdk;

namespace YXERP.Areas.M.Controllers
{
    public class IntFactoryOrdersController : YXERP.Controllers.BaseController
    {

        #region ajax
        public JsonResult GetOrderDetail(string orderID, string clientID)
        {
            var obj = OrderBusiness.BaseBusiness.GetOrderDetailByID(orderID, clientID);
            JsonDictionary.Add("result", obj);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion
    }
}
