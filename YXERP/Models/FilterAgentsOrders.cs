using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CloudSalesEnum;
using CloudSalesEntity;

namespace YXERP.Models
{
    [Serializable]
    public class FilterAgentsOrders
    {
        public EnumOrderStatus status { get; set; }

        public EnumOutStatus outstatus { get; set; }

        public EnumSendStatus sendstatus { get; set; }

        public EnumReturnStatus returnstatus { get; set; }

        public string keywords { get; set; }

        public string BeginTime { get; set; }

        public string EndTime { get; set; }

        public int pagesize { get; set; }

        public int pageindex { get; set; }

    }
}