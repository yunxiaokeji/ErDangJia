using CloudSalesBusiness;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CloudSalesEntity;

namespace IntFactory.Sdk
{
    public class OrderBusiness
    {
        public static OrderBusiness BaseBusiness = new OrderBusiness();

        public OrderListResult GetOrdersByYXClientCode(string yxClientCode, int pageSize, int pageIndex, string zngcClientID = "", string keyWords = "", string categoryID = "", string orderby = "", string beginPrice = "", string endPrice = "")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("yxClientCode", yxClientCode);
            paras.Add("clientID", zngcClientID);
            paras.Add("keywords", keyWords); 
            paras.Add("pageSize", pageSize);
            paras.Add("pageIndex", pageIndex);
            paras.Add("categoryID", categoryID);
            paras.Add("orderby", orderby);
            paras.Add("beginPrice", beginPrice);
            paras.Add("endPrice", endPrice);

            return HttpRequest.RequestServer<OrderListResult>(ApiOption.GetOrdersByYXClientCode, paras);
        }

        public OrderResult GetOrderDetailByID(string zngcOrderID, string zngcClientID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("orderID", zngcOrderID);
            paras.Add("clientID", zngcClientID);

            return HttpRequest.RequestServer<OrderResult>(ApiOption.GetOrderDetailByID, paras);
        }

        public OrderAttrResult GetOrdersAttrsList(string zngcOrderID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("goodsID", zngcOrderID);
            return HttpRequest.RequestServer<OrderAttrResult>(ApiOption.GetOrderAttrsList, paras);
        }
        public AddResult CreateDHOrder(string zngcOrderID, decimal price, List<ProductDetailEntity> details, string zngcClientID, string yxOrderID,string clientid="",string personname="",string mobiletele="",string citycode="",string address="")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("orderID", zngcOrderID);
            paras.Add("price", price);
            paras.Add("details", JsonConvert.SerializeObject(details).ToString());
            paras.Add("clientID", zngcClientID);
            paras.Add("yxOrderID", yxOrderID);
            paras.Add("yxClientID", clientid);
            paras.Add("personname", personname);
            paras.Add("mobiletele", mobiletele);
            paras.Add("cityCode", citycode);
            paras.Add("address", address);

            return HttpRequest.RequestServer<AddResult>(ApiOption.CreateDHOrder, paras);
        }

        public AddResult CreateOrder(string entity, string clientid, string userid = "")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("entity", entity);
            paras.Add("clientid", clientid);
            paras.Add("opearid", userid);
            return HttpRequest.RequestServer<AddResult>(ApiOption.CreateOrder, paras);
        }

        public string ZNGCAddProduct(OrderEntity order, string categoryid, string provideid, string agentid, string clientid, string userid)
        {
            int result = 0;
            string pid = ProductsBusiness.BaseBusiness.IsExistCMProduct(order.intGoodsCode, order.goodsID, clientid);
            if (string.IsNullOrEmpty(pid))
            {
                //var cmCategory = Sdk.ClientBusiness.BaseBusiness.GetCategoryByID(order.categoryID);
                string[] attrs = new string[] { "颜色", "尺码" };

                var list = new List<ProductDetail>();
                order.OrderAttrs.Where(y => y.AttrType == 2).ToList().ForEach(y =>
                {
                    order.OrderAttrs.Where(z => z.AttrType == 1).ToList().ForEach(z =>
                    {
                        ProductDetail detail = new ProductDetail();
                        
                        detail.ClientID = clientid;
                        detail.ProductName = "";
                        detail.ProductID = pid;
                        detail.Price = order.finalPrice;
                        detail.BigPrice = order.finalPrice;
                        detail.SaleAttr = attrs[0] + "," + attrs[1];
                        detail.AttrValue = y.AttrName.Replace("【", "").Replace("】", "") + "," + z.AttrName.Replace("【", "").Replace("】", "");
                        detail.SaleAttrValue = attrs[0] + ":" + y.AttrName.Replace("【", "").Replace("】", "") + "," +
                                               attrs[1] + ":" + z.AttrName.Replace("【", "").Replace("】", "");
                        detail.Remark = "[" + attrs[0] + "：" + y.AttrName.Replace("【", "").Replace("】", "") + "][" +
                                              attrs[1] + "：" + z.AttrName.Replace("【", "").Replace("】", "") + "]";
                        detail.CreateUserID = userid;
                        list.Add(detail);

                    });
                });
                pid = ProductsBusiness.BaseBusiness.AddProduct(CloudSalesEnum.EnumProductSourceType.IntFactory, order.intGoodsCode, order.goodsName, "", false, provideid, "", "件", "件", 1, categoryid, 1, "", "", "", "", "颜色,尺码",
                     order.finalPrice, order.finalPrice, (decimal)0.00, true, false, 1, 0, 0, (decimal)0.00, 0, order.orderImage, "", "", list, order.goodsID, order.intGoodsCode, userid, agentid, clientid, out result);
            } 
            return pid;
        }
    }
}
