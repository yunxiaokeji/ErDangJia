﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesBusiness;
using CloudSalesEnum;
using CloudSalesEntity;

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
        public static string AddProviders(string name, string contact, string mobile, string email, string cityCode, string address, string remark, string cmClientID, string cmClientCode, string operateID, string agentid, string clientID, int type)
        {
            return ProductsBusiness.BaseBusiness.AddProviders(name, contact, mobile, email, cityCode, address, remark, cmClientID, cmClientCode, operateID, agentid, clientID, type);
        }

        public static Products GetProductByIDForDetails(string productid)
        {
            return ProductsBusiness.BaseBusiness.GetProductByIDForDetails(productid);
        }

        public static List<Category> GetEdjCateGory(string clientid)
        {
            var result = new ProductsBusiness().GetCategorys(clientid);
            result = result.Where(x => string.IsNullOrEmpty(x.PID)).ToList();
            return result;
        }

        public List<StorageDoc> GetPurchases(string keyWords, int pageIndex, string userID, string clientID, string agentID, ref int totalCount, ref int pageCount, int status = -1, int type = 1, string begintime = "", string endtime = "", string wareid = "",
            string providerid = "", int sourcetype = -1, int pageSize = 10, int progressStatus = -1)
        {
            List<StorageDoc> list = StockBusiness.GetPurchases(type == 3 ? string.Empty : userID, (EnumDocStatus)status, keyWords, begintime, endtime, wareid,
                providerid, sourcetype, pageSize, pageIndex, ref totalCount, ref pageCount, agentID, clientID, (EnumProgressStatus)progressStatus);
            return list;   
        }

        public static bool IsExistsProvider(string cmClientID, string clientid)
        {
            object count = CommonBusiness.Select("Providers", "count(0)", " ClientID='" + clientid + "' and CMClientID ='" + cmClientID + "' and Status<>9 ");
            return Convert.ToInt32(count) > 0;
        }
    }
}
