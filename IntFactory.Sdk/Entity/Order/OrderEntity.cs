using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public string clientCode;

        public string clientName;

        public string clientCityCode;

        public string clientAdress; 

        public string clientContactName;

        public string clientMobile;

        public string clientUserLables;

        public string clientUserNum;

        public string clientCity;

        public string goodsName;

        public string intGoodsCode;

        public decimal finalPrice;

        public string categoryID;

        public string categoryName;

        public string processCategoryName;

        public string platemaking;

        public string goodsID;

        public string logo;

        public string personName;

        public string mobileTele;

        public string cityCode;

        public string address;
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

        public List<OrderAttrEntity> OrderAttrs;

        public List<OrderPriceRangeEntity> orderPriceRanges;
    }
}
