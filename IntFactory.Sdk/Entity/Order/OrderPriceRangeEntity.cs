using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk
{
    public class OrderPriceRangeEntity
    {
        public string rangeID { get; set; }

        public int minQuantity { get; set; }

        public decimal price { get; set; }
    }
}
