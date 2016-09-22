using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CloudSalesEntity;
using YunXiaoService;
using CloudSalesEnum;

namespace YXERP.Areas.M.Controllers
{
    public class IntFactoryOrdersController : YXERP.Controllers.BaseController
    {

        #region ajax
        public JsonResult GetProducts(string keyWords, string categoryID, string beginPrice, string endPrice, bool isAsc, string orderby, int pageSize, int pageIndex)
        {
            IntFactory.Sdk.OrderListResult list = IntFactory.Sdk.OrderBusiness.BaseBusiness.GetOrdersByYXClientCode(keyWords, CurrentUser.CurrentCMClientID, pageSize, pageIndex,
                categoryID, beginPrice, endPrice, isAsc, orderby);
            JsonDictionary.Add("items", list.orders);
            JsonDictionary.Add("pageCount", list.pageCount);
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

        public JsonResult GetPurchases(string keyWords, int pageIndex, int status = -1, int type = 1, string begintime = "", string endtime = "", string wareid = "", string providerid = "", int sourcetype = -1, int pageSize = 10)
        {
            int pageCount = 0;
            int totalCount = 0;
            List<StorageDoc> list = new ProductService().GetPurchases(keyWords, pageIndex, 
                type == 3 ? string.Empty : CurrentUser.UserID, CurrentUser.ClientID, CurrentUser.AgentID, ref totalCount, ref  pageCount,
                status, type, begintime, endtime, wareid, providerid, (int)EnumProductSourceType.IntFactory, pageSize);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
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
