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
            paras.Add("zngcClientID", zngcClientID);
            paras.Add("pageSize", pageSize);
            paras.Add("pageIndex", pageIndex);

            return HttpRequest.RequestServer<OrderListResult>(ApiOption.GetOrdersByYXClientCode, paras);
        }
    }
}
