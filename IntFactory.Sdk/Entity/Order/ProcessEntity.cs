using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IntFactory.Sdk
{
    public class ProcessEntity
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        public string processID { get; set; }

        /// <summary>
        /// 类型 1打样 2大货
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string processName { get; set; }

        
        
    }
}
