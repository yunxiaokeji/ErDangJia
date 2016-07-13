using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    }
}
