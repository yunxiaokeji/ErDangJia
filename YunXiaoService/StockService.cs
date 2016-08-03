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
        public static List<StorageDoc> GetPurchases(string userid, int status, string keywords, string begintime, string endtime, string wareid, string providerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
           EnumDocStatus statustype= Enum.Parse(typeof (EnumDocStatus), status.ToString());

            return StockBusiness.GetPurchases(userid, Enum.Parse(typeof(EnumDocStatus), status.ToString()), keywords, begintime, endtime, wareid, providerid, pageSize,
                pageIndex, ref totalCount, ref pageCount, agentid, clientid);
        }

    }
}
