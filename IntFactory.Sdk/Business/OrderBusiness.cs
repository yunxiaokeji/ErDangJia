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

        public string ZNGCAddProduct(OrderEntity ord, string categoryid, string provideid, string agentid, string clientid, string userid)
        {
            int result = 0;
            string pid = ProductsBusiness.BaseBusiness.IsExistCMProduct(ord.intGoodsCode, ord.goodsID, clientid);
            if (string.IsNullOrEmpty(pid))
            {
                pid = ProductsBusiness.BaseBusiness.AddProduct(ord.intGoodsCode, ord.goodsName, "", false, provideid, "", "", "", 0, categoryid, 1, "", "", ""
                    , ord.finalPrice, ord.finalPrice, (decimal)0.00, true, false, 1, 0, 0, (decimal)0.00, 0, ord.orderImage, "", "", new List<ProductDetail>(), ord.goodsID, ord.intGoodsCode, userid, agentid, clientid, out result);

                CheckProductDetail(pid, ord.details, ord.finalPrice, clientid, userid, ord.goodsID, ord.intGoodsCode, ord.goodsName);
            } 
            return pid;
        }

        public string CheckProductDetail(string pid, List<ProductDetailEntity> details, decimal price, string clientid, string userid, string goodsID,string goodscode,string goodsname)
        {
            string dids="";
            details.ForEach(x =>
            {
                int result = 0;
                x.remark = x.remark.Replace("【", "[").Replace("】", "]");
                string did = ProductsBusiness.BaseBusiness.IsExistCMProductDetail(x.remark, goodscode, goodsID, clientid);
                if (string.IsNullOrEmpty(did))
                {
                    ProductDetail detail = new ProductDetail();
                    detail.Remark = x.remark;
                    detail.ClientID = clientid;
                    detail.ProductName = goodsname;
                    detail.ProductID = pid;
                    detail.Price = price;
                    detail.BigPrice = price;
                    detail.CreateUserID = userid;
                    did = ProductsBusiness.BaseBusiness.AddProductDetails(pid, "", "", x.saleAttr, x.attrValue, x.saleAttrValue, price,
                        (decimal)0.00, price, "", x.remark, "", userid, clientid, out result);
                    if (result == 1)
                    {
                        dids += "" + did + ":" + x.quantity + ",";
                    }
                }
                else
                {
                    dids += "" + did + ":" + x.quantity + ",";
                }
            });
            dids = dids.TrimEnd(',');
            return dids;
        }
    }
}
