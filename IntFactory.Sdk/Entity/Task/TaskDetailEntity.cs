using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IntFactory.Sdk
{
    public class TaskDetailEntity
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public string taskID { get; set; }

        /// <summary>
        /// 任务编码
        /// </summary>
        public string taskCode { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string orderID { get; set; }

        /// <summary>
        /// 流程ID
        /// </summary>
        public string processID { get; set; }

        /// <summary>
        /// 阶段ID
        /// </summary>
        public string stageID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 标记  1：材料 2 制版 3大货材料
        /// </summary>
        public int mark { get; set; }

        /// <summary>
        /// 颜色标记   ["#FFF", "#FF7C7C", "#3BB3FF", "#9F74FF", "#FFC85D", "#FFF65F"]
        /// </summary>
        public int colorMark { get; set; }

        /// <summary>
        /// 完成状态 0：为开始；1：进行中，2：已结束
        /// </summary>
        public int finishStatus { get; set; }

        /// <summary>
        /// 订单类型 1：打样；2：大货
        /// </summary>
        public int orderType { get; set; }

        /// <summary>
        /// 订单图片
        /// </summary>
        public string orderImg { get; set; }

        /// <summary>
        /// 接受时间
        /// </summary>
        public string acceptTime { get; set; }

        /// <summary>
        /// 到期时间
        /// </summary>
        public string endTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public string completeTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string createTime { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public UserBase ownerUser { get; set; }

        public OrderBaseEntity order { get; set; }
    }
}
