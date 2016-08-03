using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPOI.HSSF.Model;

namespace YXERP.Areas.Api.Models
{
    [Serializable]
    public class FilterPurchase
    {
        /// <summary>
        /// 采购单ID
        /// </summary>
        public string DocID { get; set; }
        /// <summary>
        /// 采购单入库单据类型  
        /// </summary>
        public int DocType { get; set; }
        /// <summary>
        /// 是否结束采购单 0不结束 1结束
        /// </summary>
        public int IsOver { get; set; }
        /// <summary>
        /// 采购单入库明细
        /// </summary>
        public List<FilterPurchaseDetail> Details { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
    [Serializable]
    public class FilterPurchaseDetail
    {
        /// <summary>
        /// 采购单明细ID
        /// </summary>
        public string AutoID { get; set; }
        /// <summary>
        /// 对应入库数量
        /// </summary>
        public int Num { get; set; } 
        /// <summary>
        /// 仓库库位
        /// </summary>
        public int DepotID { get; set; } 
    }
}