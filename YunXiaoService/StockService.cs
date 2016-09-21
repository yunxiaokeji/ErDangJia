using CloudSalesEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSalesBusiness;
using CloudSalesEntity.Manage;
using CloudSalesEnum; 

namespace YunXiaoService
{
    public class StockService
    {
        #region 仓库 快递基本信息获取
        /// <summary>
        /// 获取代理商仓库信息
        /// </summary>
        /// <param name="clientid">公司ID</param>
        /// <returns></returns>
        public static List<WareHouse> GetAllWareHouses(string clientid)
        {
            return  new SystemBusiness().GetWareHouses(clientid);
        }

        /// <summary>
        /// 获取仓库库位信息
        /// </summary>
        /// <param name="wareid">仓库ID</param>
        /// <param name="agentid">代理商ID</param>
        /// <param name="clientid">公司ID</param>
        /// <returns>List</returns>
        public static List<DepotSeat> GetDepotSeatsByWareID(string wareid, string agentid, string clientid)
        {
            return new SystemBusiness().GetDepotSeatsByWareID(wareid, clientid);
        }
        /// <summary>
        /// 获取系统所有快递
        /// </summary>
        /// <returns></returns>
        public static List<ExpressCompany> GetExpress()
        {
            return  CloudSalesBusiness.Manage.ExpressCompanyBusiness.GetExpressCompanys();
        }

        #endregion

        #region 采购单

        //获取采购订单
        public static List<StorageDoc> GetPurchases(string userid, int status, string keywords, string begintime, string endtime, string wareid, string providerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            return StockBusiness.GetPurchases(userid, (EnumDocStatus)Enum.Parse(typeof(EnumDocStatus), status.ToString()), keywords, begintime, endtime, wareid, providerid,-1, pageSize,
                 pageIndex, ref totalCount, ref pageCount, agentid, clientid);
        }
        /// <summary>
        /// 入库采购单 支持 分批入库
        /// </summary>
        /// <param name="docid">采购单号</param>
        /// <param name="doctype">采购单入库类型 默认101</param>
        /// <param name="isover">是否完结采购单 0不完结 1 完结</param>
        /// <param name="details">采购单入库明细 格式: 明细AutoID-Num:DepotID,</param>
        /// <param name="remark">备注</param>
        /// <param name="userid">操作人</param>
        /// <param name="agentid">代理商ID</param>
        /// <param name="clientid">公司ID</param>
        /// <param name="errmsg">返回错误信息</param>
        /// <returns></returns>
        public static bool AuditPurchase(string docid, int doctype, int isover, string details, string remark,string opearip, string userid, string agentid, string clientid, ref string errmsg)
        {
            int result = 0;
            return new StockBusiness().AuditStorageIn(docid, doctype, isover, details, remark, userid, opearip, agentid, clientid, ref result, ref errmsg);
        }

        public string AddIntfactoryPurchaseDoc(string goodsID, string goodsCode, string goodsName, string price, string productDetails, string cmClientID,
                                            int docType, int sourceType, decimal totalMoney, string userID, string agentID, string clientID,
        string saleAttrStr = "", string productImage = "", string personName = "", string mobilePhone = "", string cityCode = "", string address = "")
        {
            return new StockBusiness().AddIntfactoryPurchaseDoc(goodsID, goodsCode, goodsName, price, productDetails, cmClientID, docType, sourceType,
                totalMoney, userID, agentID, clientID, saleAttrStr, productImage, personName, mobilePhone, cityCode, address);
        }

        #endregion

        #region 出库单

        /// <summary>
        /// 分页获取出库单（GetAgentOrders）
        /// </summary>
        /// <returns></returns>
        public static List<AgentOrderEntity> GetStockOut(int status, int outstatus, int sendstatus, int returnstatus, string keywords, string beginTime, string endTime, int pagesize, int pageindex, ref int totalCount, ref int pageCount, string clientid, string agentid = "")
        {
            return AgentOrderBusiness.BaseBusiness.GetAgentOrders("", (EnumOrderStatus)Enum.Parse(typeof(EnumOrderStatus), status.ToString()), (EnumOutStatus)Enum.Parse(typeof(EnumOutStatus), outstatus.ToString()), (EnumSendStatus)Enum.Parse(typeof(EnumSendStatus), sendstatus.ToString()), (EnumReturnStatus)Enum.Parse(typeof(EnumReturnStatus),returnstatus.ToString()),
                keywords, beginTime, endTime, pagesize, pageindex, ref totalCount, ref pageCount, agentid, clientid);
        }
        /// <summary>
        /// 出库
        /// </summary>
        /// <param name="expresscode"></param>
        /// <param name="expressid"></param>
        /// <param name="orderid"></param>
        /// <param name="wareid"></param>
        /// <param name="userid"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <param name="errmsg"></param>
        /// <param name="issend"></param>
        /// <returns></returns>
        public static bool ConfirmAgentOrderOut(string expresscode, string expressid, string orderid, string wareid, string userid,string agentid, string clientid, ref string errmsg,
            int issend = 0)
        {
            int result = 0;
            return AgentOrderBusiness.BaseBusiness.ConfirmAgentOrderOut(orderid, wareid, issend, expressid, expresscode, userid,
                agentid, clientid, ref result, ref errmsg);
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="expressid"></param>
        /// <param name="expresscode"></param>
        /// <param name="userid"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public static bool ConfirmAgentOrderSend(string orderid, string expressid, string expresscode, string userid, string agentid, string clientid, ref string errmsg)
        {
            int result = 0;
            return AgentOrderBusiness.BaseBusiness.ConfirmAgentOrderSend(orderid,expressid,expresscode, userid,
                agentid, clientid, ref result, ref errmsg);
        }

        public static bool AuditApplyReturnProduct(string orderid, string wareid,string userid,string agentid,string clientid,ref string errmsg)
        {
            int result = 0;
            return AgentOrderBusiness.BaseBusiness.AuditApplyReturnProduct(orderid, wareid, userid, agentid, clientid, ref result, ref errmsg);
        }

        #endregion

    }
}
