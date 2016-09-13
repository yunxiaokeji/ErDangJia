using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk
{
    public class OrderAttrResult
    {
        public OrderAttrEntity orderAttr;

        /// <summary>
        /// 规格列表
        /// </summary>
        public List<OrderAttrEntity> attrList;
        public int totalCount = 0;

        public int pageCount = 0;

        public int error_code = 0;

        public string error_message;
    }
}
