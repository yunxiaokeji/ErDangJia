using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CloudSalesEntity
{
    public class ClientsIndustry
    {
        public int AutoID { get; set; }

        public string ClientIndustryID { get; set; }
        
        public string AgentID { get; set; }

        public string ClientID { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public string Description { get; set; }

        public DateTime CreateTime { get; set; }

        public Users CreateUser { get; set; }

        public string CreateUserID { get; set; }

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
