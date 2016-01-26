using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CloudSalesEnum;
using CloudSalesEntity;

namespace YXERP.Models
{
    [Serializable]
    public class ShoppingCartProduct
    {

        public EnumDocType type { get; set; }

        public string guid { get; set; }

        public List<ProductStock> Products { get; set; }

    }
}