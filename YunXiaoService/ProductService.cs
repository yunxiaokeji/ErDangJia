using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesBusiness;
using CloudSalesEnum;

namespace YunXiaoService
{
    public class ProductService
    {
        /// <summary>
        /// 添加供应商
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="contact">联系人姓名</param>
        /// <param name="mobile">联系电话</param>
        /// <param name="email">邮箱</param>
        /// <param name="cityCode">城市编码</param>
        /// <param name="address">地址</param>
        /// <param name="remark">备注</param>
        /// <param name="cmClientID">智能工厂ID</param>
        /// <param name="cmClientCode">智能工厂Code</param>
        /// <param name="operateID">操作人</param>
        /// <param name="agentid">代理商ID</param>
        /// <param name="clientID">客户端ID</param>
        /// <returns></returns>
        public static string AddProviders(string name, string contact, string mobile, string email, string cityCode, string address, string remark, string cmClientID, string cmClientCode, string operateID, string agentid, string clientID)
        {
            return ProductsBusiness.BaseBusiness.AddProviders(name, contact, mobile, email, cityCode, address, remark, cmClientID, cmClientCode, operateID, agentid, clientID);
        }
    }
}
