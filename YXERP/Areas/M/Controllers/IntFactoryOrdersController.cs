using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudSalesEntity;
using YunXiaoService;

namespace YXERP.Areas.M.Controllers
{
    public class IntFactoryOrdersController : YXERP.Controllers.BaseController
    {

        #region ajax
        public JsonResult GetProducts(string clientid, int pageSize, int pageIndex)
        {
            IntFactory.Sdk.OrderListResult list = IntFactory.Sdk.OrderBusiness.BaseBusiness.GetOrdersByYXClientCode("", pageSize, pageIndex, clientid);
            JsonDictionary.Add("items", list.orders);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderDetail(string orderID, string clientID)
        {
            var obj = ProductService.GetProductByIDForDetails(orderID);
            JsonDictionary.Add("result", obj);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetEdjCateGory(string clientID)
        {
            var obj = IntFactory.Sdk.ClientBusiness.BaseBusiness.GetAllCategory().FindAll(m => m.CategoryType == 2 && m.Layers == 1);
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
