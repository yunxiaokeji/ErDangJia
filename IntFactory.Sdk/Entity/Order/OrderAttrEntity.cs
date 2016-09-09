using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk
{
    public class OrderAttrEntity
    {
        public string OrderAttrID { get; set; }

        public string OrderID { get; set; }

        public string GoodsID { get; set; }

        public string AttrName { get; set; }

        public int AttrType { get; set; }

        public decimal Price { get; set; }

        public decimal FinalPrice { get; set; }
         
    }
}
