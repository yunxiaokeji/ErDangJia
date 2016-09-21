using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
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
            var obj = IntFactory.Sdk.OrderBusiness.BaseBusiness.GetOrderDetailByID(orderID, clientID);
            JsonDictionary.Add("result", obj.order);
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

        public JsonResult AddIntfactoryPurchaseDoc(string goodsID, string goodsCode, string goodsName, decimal price, string productDetails, string cmClientID,
                                             decimal totalMoney,string zngcOrderID,string zngcClientID,string zngcProductEntity, string saleAttrStr = "", string productImage = "", string personName = "",
                                            string mobilePhone = "", string cityCode = "", string address = "")
        {
            string id = new StockService().AddIntfactoryPurchaseDoc(goodsID, goodsCode, goodsName, price.ToString(), productDetails, cmClientID,
                 (int)CloudSalesEnum.EnumDocType.RK, (int)CloudSalesEnum.EnumProductSourceType.IntFactory, totalMoney,
                 CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, saleAttrStr, productImage, personName, mobilePhone, cityCode, address);
            if (!string.IsNullOrEmpty(id))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<IntFactory.Sdk.ProductDetailEntity> details = serializer.Deserialize<List<IntFactory.Sdk.ProductDetailEntity>>(zngcProductEntity);
                IntFactory.Sdk.AddResult result = IntFactory.Sdk.OrderBusiness.BaseBusiness.CreateDHOrder(zngcOrderID, price, details, zngcClientID, id, CurrentUser.ClientID, personName, mobilePhone, cityCode, address);
                JsonDictionary.Add("result", result);
            }
            else
            {
                JsonDictionary.Add("result", "");
            }
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion
    }
}
