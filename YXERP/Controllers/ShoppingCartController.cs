using CloudSalesBusiness;
using CloudSalesEntity;
using CloudSalesEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YXERP.Models;

namespace YXERP.Controllers
{
    public class ShoppingCartController : BaseController
    {
        //
        // GET: /ShoppingCart/

        public ActionResult Index()
        {
            return View();
        }

        #region Ajax 订单和购物车相关

        public JsonResult GetProductListForShopping(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterProduct model = serializer.Deserialize<FilterProduct>(filter);
            int totalCount = 0;
            int pageCount = 0;

            List<Products> list = new ProductsBusiness().GetFilterProducts(model.CategoryID, model.Attrs, model.DocType, model.BeginPrice, model.EndPrice, model.Keywords, model.OrderBy, model.IsAsc, 20, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AddShoppingCart(string productid, string detailsid, int quantity, string unitid, int isBigUnit, EnumDocType ordertype, string remark = "", string guid = "")
        {
            var bl = ShoppingCartBusiness.AddShoppingCart(productid, detailsid, quantity, unitid, isBigUnit, ordertype, remark, guid, CurrentUser.UserID, OperateIP);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AddShoppingCartBatchOut(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ShoppingCartProduct>(entity);

            var bl = false;
            foreach (var product in model.Products)
            {
                if (ShoppingCartBusiness.AddShoppingCartBatchOut(product.ProductID, product.ProductDetailID, 1, product.BatchCode, product.DepotID, model.type, product.SaleAttrValueString, model.guid, CurrentUser.UserID, OperateIP))
                {
                    bl = true;
                }
            }
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AddShoppingCartBatchIn(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<ShoppingCartProduct>(entity);

            var bl = false;
            foreach (var product in model.Products)
            {
                if (ShoppingCartBusiness.AddShoppingCartBatchIn(product.ProductID, product.ProductDetailID, 1, model.type, product.SaleAttrValueString, model.guid, CurrentUser.UserID, OperateIP))
                {
                    bl = true;
                }
            }
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetShoppingCartCount(EnumDocType ordertype, string guid = "")
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = CurrentUser.UserID;
            }
            var count = ShoppingCartBusiness.GetShoppingCartCount(ordertype, guid);
            JsonDictionary.Add("Quantity", count);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetShoppingCart(EnumDocType ordertype, string guid = "")
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = CurrentUser.UserID;
            }

            var list = ShoppingCartBusiness.GetShoppingCart(ordertype, guid, "");
            JsonDictionary.Add("Items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateCartQuantity(string autoid, int quantity)
        {
            var bl = ShoppingCartBusiness.UpdateCartQuantity(autoid, quantity, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateCartBatch(string autoid, string batch)
        {
            var bl = ShoppingCartBusiness.UpdateCartBatch(autoid, batch, CurrentUser.UserID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateCartPrice(string autoid, decimal price)
        {
            var bl = ShoppingCartBusiness.UpdateCartPrice(autoid, price, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteCart(string productid, int ordertype, string guid)
        {
            var bl = ShoppingCartBusiness.DeleteCart(productid, ordertype, guid, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
