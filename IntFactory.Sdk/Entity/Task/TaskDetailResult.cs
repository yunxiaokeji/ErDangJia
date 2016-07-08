using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk
{
    public class TaskDetailResult
    {
        public TaskDetailEntity task;

        public string domainUrl;

        /// <summary>
        /// 材料列表
        /// </summary>
        public List<ProductBaseEntity> materialList;

        public int error_code = 0;

        public string error_message;

    }
}
