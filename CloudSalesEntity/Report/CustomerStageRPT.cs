using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity
{
    public class StageCustomerEntity
    {
        public string GUID { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 父级名称
        /// </summary>
        public string PName { get; set; }
        /// <summary>
        /// 客户总计
        /// </summary>
        public int TotalNum { get; set; }
        /// <summary>
        /// 新增客户总计
        /// </summary>
        public int NCSRNum { get; set; }
        /// <summary>
        /// 机会客户总计
        /// </summary>
        public int OCSRNum { get; set; }
        /// <summary>
        /// 成交客户总计
        /// </summary>
        public int SCSRNum { get; set; }
        /// <summary>
        /// 状态客户集合
        /// </summary>
        public List<StageCustomerItem> Stages { get; set; }
        /// <summary>
        /// 子集
        /// </summary>
        public List<StageCustomerEntity> ChildItems { get; set; }
    }

    public class StageCustomerItem
    {
        public string StageID { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }

        public decimal Money { get; set; }
    }
}
