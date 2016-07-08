using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IntFactory.Sdk
{
    public class TaskLogEntity
    {
        /// <summary>
        /// 日志描述
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string createTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserBase createUser { get; set; }

    }
}
