using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class ShoppingCartDAL : BaseDAL
    {
        public static DataTable GetShoppingCart(int ordertype, string guid, string userid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderType",ordertype),
                                     new SqlParameter("@GUID" , guid),
                                     new SqlParameter("@UserID" , userid)
                                   };
            return GetDataTable("P_GetShoppingCart", paras, CommandType.StoredProcedure);
        }

        public static bool AddShoppingCart(int ordertype, string guid, string productid, string detailsid, int quantity, string remark, string userid, string operateip)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderType",ordertype),
                                     new SqlParameter("@GUID" , guid),
                                     new SqlParameter("@ProductDetailID",detailsid),
                                     new SqlParameter("@ProductID" , productid),
                                     new SqlParameter("@Quantity" , quantity),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@OperateIP" , operateip)
                                   };
            return ExecuteNonQuery("P_AddShoppingCart", paras, CommandType.StoredProcedure) > 0;
        }

        public static bool AddShoppingCartBatchOut(string productid, string detailsid, int quantity, int ordertype, string batch, string wareid, string depotid, string remark, string guid, string userid, string operateip)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OrderType",ordertype),
                                     new SqlParameter("@ProductDetailID",detailsid),
                                     new SqlParameter("@ProductID" , productid),
                                     new SqlParameter("@Quantity" , quantity),
                                     new SqlParameter("@BatchCode" , batch),
                                     new SqlParameter("@WareID" , wareid),
                                     new SqlParameter("@DepotID" , depotid),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@GUID" , guid),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@OperateIP" , operateip)
                                   };
            return ExecuteNonQuery("P_AddShoppingCartBatchOut", paras, CommandType.StoredProcedure) > 0;
        }

        public static bool DeleteCart(string guid, string productid, int ordertype, string userid, string depotid)
        {
            SqlParameter[] paras = {
                                       new SqlParameter("@GUID" , guid),
                                       new SqlParameter("@ProductID",productid),
                                       new SqlParameter("@Ordertype",ordertype),
                                       new SqlParameter("@DepotID",depotid),
                                       new SqlParameter("@UserID",userid)
                                   };
            return ExecuteNonQuery("P_DeleteCart", paras, CommandType.StoredProcedure) > 0;
        }

    }
}
