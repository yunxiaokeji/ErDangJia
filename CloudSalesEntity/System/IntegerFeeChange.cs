using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity
{
    public class IntegerFeeChange
    {
        public int AutoID { get; set; }

        public int ChangeType { get; set; }

        public decimal ChangeFee { get; set; }

        public decimal OldChangeFee { get; set; }

        public string AgentID { get; set; }

        public string ClientID { get; set; }

        public DateTime CreateTime { get; set; }

        public Users CreateUser { get; set; }

        public string Reamrk { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }
        // <summary>
        /// 填充数据
        /// </summary>
        /// <param name="dr"></param>
        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}
