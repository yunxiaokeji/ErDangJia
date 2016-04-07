using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity.Manage
{
    public class ClientOrder
    {
        public int AutoID { get; set; }

        [Property("Lower")]
        public string OrderID { get; set; }

        public int UserQuantity { get; set; }

        public int Years { get; set; }

        public decimal Amount { get; set; }

        public decimal RealAmount { get; set; }

        public int PayType { get; set; }

        public int Type { get; set; }

        public int SystemType { get; set; }

        public int Status { get; set; }

        [Property("Lower")]
        public string AgentID { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        public Users CreateUser { get; set; }

        public DateTime CreateTime { get; set; }
        public List<ClientOrderDetail> Details { get; set; }
        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="dr"></param>
        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
