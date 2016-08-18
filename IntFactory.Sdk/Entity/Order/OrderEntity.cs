using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactory.Sdk
{
    public class OrderEntity
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string orderID;

        public string clientID;

        public string goodsName;

        public string intGoodsCode;

        public decimal finalPrice;

        public string categoryID;

        public string categoryName;

        public string platemaking;

        public string goodsID;

        /// <summary>
        /// 订单样图缩约图
        /// </summary>
        public string orderImage;

        /// <summary>
        /// 订单样图列表
        /// </summary>
        public string orderImages;

        public DateTime createTime;

        public DateTime endTime;

        public List<PlateMakingEntity> plateMakings;

        public List<ProductDetailEntity> details;

        public List<ProductAttrEntity> AttrLists;

        public List<ProductAttrEntity> SaleAttrs;
    }
}
