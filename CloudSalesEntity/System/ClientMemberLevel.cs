using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity
{
    public class ClientMemberLevel
    {
        public int AutoID { get; set; }

        [Property("Lower")]
        public string LevelID { get; set; }

        [Property("Lower")]
        public string AgentID { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        public string Name { get; set; }

        public decimal IntegFeeMore { get; set; }

        public int Origin { get; set; }

        public decimal DiscountFee { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public Users CreateUser { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        public string ImgUrl { get; set; }

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
