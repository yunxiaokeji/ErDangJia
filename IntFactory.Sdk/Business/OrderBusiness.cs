using CloudSalesBusiness;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSalesEntity;

namespace IntFactory.Sdk.Business
{
    public class OrderBusiness
    {
        public static OrderBusiness BaseBusiness = new OrderBusiness();

        public OrderListResult GetOrdersByYXClientCode(string yxClientCode,int pageSize,int pageIndex, string zngcClientID = "")
        {
            var paras = new Dictionary<string, object>();
            paras.Add("yxClientCode", yxClientCode);
            paras.Add("clientID", zngcClientID);
            paras.Add("pageSize", pageSize);
            paras.Add("pageIndex", pageIndex);

            return HttpRequest.RequestServer<OrderListResult>(ApiOption.GetOrdersByYXClientCode, paras);
        }

        public OrderResult GetOrderDetailByID(string zngcOrderID, string zngcClientID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("orderID", zngcOrderID);
            paras.Add("clientID", zngcClientID);

            return HttpRequest.RequestServer<OrderResult>(ApiOption.GetOrderDetailByID, paras);
        }
    
        public AddResult CreateDHOrder(string zngcOrderID, decimal price, List<ProductDetailEntity> details, string zngcClientID, string yxOrderID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("orderID", zngcOrderID);
            paras.Add("price", price);
            paras.Add("details", JsonConvert.SerializeObject(details).ToString());
            paras.Add("clientID", zngcClientID);
            paras.Add("yxOrderID", yxOrderID);

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
        public Products ZNGCAddProduct(OrderEntity ord, string agentid, string clientid, string userid)
        {
            int result = 0;
            string pid = ProductsBusiness.BaseBusiness.IsExistCMProduct(ord.intGoodsCode, ord.clientID);
            if (string.IsNullOrEmpty(pid)) 
            {
                pid = ProductsBusiness.BaseBusiness.AddProduct(ord.intGoodsCode,ord.goodsName,"",false,ord.clientID,"","","",0,"",1,"","",""
                    ,ord.finalPrice,ord.finalPrice,(decimal)0.00,true,false,1,0,0,(decimal)0.00,0,ord.orderImage,"","",null,userid,agentid,clientid,out result);
            }
            Products pdt= ProductsBusiness.BaseBusiness.GetProductByID(pid); 
            string detailids = "";
            ord.details.ForEach(x=>
            {
                string did = ProductsBusiness.BaseBusiness.IsExistCMProductDetail(x.remark,ord.intGoodsCode, ord.clientID);
                if (!string.IsNullOrEmpty(pid))
                {
                    ProductDetail detail = new ProductDetail();
                    detail.Remark = x.remark;
                    detail.ClientID = clientid;
                    detail.ProductName = ord.goodsName;
                    detail.ProductID = pid;
                    detail.Price = ord.finalPrice;
                    detail.BigPrice = ord.finalPrice;
                    detail.CreateUserID = userid;
                    did = ProductsBusiness.BaseBusiness.AddProductDetails(pid, "", "", "", "", "", ord.finalPrice,
                        (decimal) 0.00, ord.finalPrice, "", x.remark, "", userid, clientid, out result);
                    if (result == 0)
                    {
                        detailids += "'" + did + "',";
                    }
                }
                else
                {
                    detailids += "'" + did + "',";
                }
            }); 
            pdt.ProductDetails=ProductsBusiness.BaseBusiness.GetProductDetailsByDids(detailids);
            return pdt;
        }

    }
}
