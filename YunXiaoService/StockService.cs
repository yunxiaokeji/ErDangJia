using CloudSalesEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSalesBusiness;
using CloudSalesEnum;

namespace YunXiaoService
{
    public class StockService
    {
        //获取采购订单
        public static List<StorageDoc> GetPurchases(string userid, int status, string keywords, string begintime, string endtime, string wareid, string providerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        { 
           return StockBusiness.GetPurchases(userid, (EnumDocStatus)Enum.Parse(typeof(EnumDocStatus), status.ToString()), keywords, begintime, endtime, wareid, providerid, pageSize,
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
        public static bool AuditPurchase(string docid,int doctype,int isover,string details,string remark,string userid, string agentid, string clientid,ref string errmsg)
        {
            int result = 0;
            return new StockBusiness().AuditStorageIn(docid, doctype, isover, details, remark, userid, "", agentid, clientid, ref result, ref errmsg);
        }
    }
}
