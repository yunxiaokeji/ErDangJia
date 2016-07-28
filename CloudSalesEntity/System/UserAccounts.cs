using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity
{
    public class UserAccounts
    {
        public int AutoID { get; set; }
       
        public string AccountName { get; set; }
         
        public string AgentID { get; set; }
         
        public string ClientID { get; set; }

        public string UserID { get; set; }

        public string ProjectID { get; set; }

        public int AccountType { get; set; }
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
