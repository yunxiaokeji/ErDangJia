using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IntFactory.Sdk
{
    public class TaskReplyEntity
    {
        /// <summary>
        /// 回复ID
        /// </summary>
        public string replyID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string orderID { get; set; }

        /// <summary>
        /// 阶段ID
        /// </summary>
        public string stageID { get; set; }

        /// <summary>
        /// 讨论标记 0：其他；1：材料 2 制版 3大货材料
        /// </summary>
        public int mark { get; set; }

        /// <summary>
        /// 讨论内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string createTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserBase createUser { get; set; }

        /// <summary>
        /// 来源讨论人
        /// </summary>
        public UserBase fromReplyUser { get; set; }

        public string fromReplyID { get; set; }

        public string fromReplyUserID { get; set; }

        public string fromReplyAgentID { get; set; }
    }
}
