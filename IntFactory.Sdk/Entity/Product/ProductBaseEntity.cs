using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntFactory.Sdk
{
    public class ProductBaseEntity
    {
        /// <summary>
        /// 产品编码
        /// </summary>
        public string code;

        /// <summary>
        /// 产品图片
        /// </summary>
        public string productImage;

        /// <summary>
        /// 名称
        /// </summary>
        public string productName;

        /// <summary>
        /// 规格说明
        /// </summary>
        public string remark;

        /// <summary>
        /// 单位名称
        /// </summary>
        public string unitName;

        /// <summary>
        /// 价格
        /// </summary>
        public decimal price;

        /// <summary>
        /// 消耗量
        /// </summary>
        public long quantity;

        /// <summary>
        /// 损耗量
        /// </summary>
        public long loss;

        /// <summary>
        /// 金额
        /// </summary>
        public decimal totalMoney;
    }
}
