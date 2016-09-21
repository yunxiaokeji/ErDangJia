using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class StockDAL : BaseDAL
    {
        public static StockDAL BaseProvider = new StockDAL();

        #region 查询

        public static DataTable GetPurchasesByExcel(string userid, int status, string keywords, string begintime, string endtime, string wareid, string providerid, int doctype, string clientid)
        {
            SqlParameter[] paras = {   new SqlParameter("@UserID", userid),
                                       new SqlParameter("@DocType", doctype),
                                       new SqlParameter("@Status", status),
                                       new SqlParameter("@BeginTime", begintime),
                                       new SqlParameter("@EndTime", endtime),
                                       new SqlParameter("@KeyWords",keywords),
                                       new SqlParameter("@WareID", wareid),
                                       new SqlParameter("@ProviderID", providerid), 
                                       new SqlParameter("@ClientID",clientid)
                                   }; 
            DataTable dt = GetDataTable("P_GetStorageDocForExcel", paras, CommandType.StoredProcedure);
            return dt;
        }


        public static DataSet GetPurchases(string userid, int status, string keywords, string begintime, string endtime, string wareid, string providerid,int sourcetype, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@TotalCount",SqlDbType.Int),
                                       new SqlParameter("@PageCount",SqlDbType.Int),
                                       new SqlParameter("@UserID", userid),
                                       new SqlParameter("@Status", status),
                                       new SqlParameter("@BeginTime", begintime),
                                       new SqlParameter("@EndTime", endtime),
                                       new SqlParameter("@KeyWords",keywords),
                                       new SqlParameter("@WareID", wareid),
                                       new SqlParameter("@SourceType", sourcetype),
                                       new SqlParameter("@ProviderID", providerid),
                                       new SqlParameter("@PageSize",pageSize),
                                       new SqlParameter("@PageIndex",pageIndex),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetPurchases", paras, CommandType.StoredProcedure, "Doc|Details");
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public static DataSet GetStorageDocList(string userid, int type, int status, string keywords, string begintime, string endtime, string wareid, string providerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@TotalCount",SqlDbType.Int),
                                       new SqlParameter("@PageCount",SqlDbType.Int),
                                       new SqlParameter("@UserID", userid),
                                       new SqlParameter("@DocType", type),
                                       new SqlParameter("@Status", status),
                                       new SqlParameter("@BeginTime", begintime),
                                       new SqlParameter("@EndTime", endtime),
                                       new SqlParameter("@KeyWords",keywords),
                                       new SqlParameter("@WareID", wareid),
                                       new SqlParameter("@ProviderID", providerid),
                                       new SqlParameter("@PageSize",pageSize),
                                       new SqlParameter("@PageIndex",pageIndex),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetStorageDocList", paras, CommandType.StoredProcedure, "Doc|Details");
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public static DataSet GetStorageDetail(string docid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@DocID",docid),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataSet ds = GetDataSet("P_GetStorageDetail", paras, CommandType.StoredProcedure, "Doc|Details");
            return ds;
        }

        public DataSet GetProductStocks(string keywords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@TotalCount",SqlDbType.Int),
                                       new SqlParameter("@PageCount",SqlDbType.Int),
                                       new SqlParameter("@KeyWords",keywords),
                                       new SqlParameter("@PageSize",pageSize),
                                       new SqlParameter("@PageIndex",pageIndex),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetProductStocks", paras, CommandType.StoredProcedure, "Products");
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public DataTable GetProductDetailStocks(string productid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ProductID",productid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            return GetDataTable("select * from ProductDetail where ProductID=@ProductID and ClientID=@ClientID and Status<>9 ", paras, CommandType.Text);
        }

        public DataSet GetDetailStocks(string wareid, string keywords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@TotalCount",SqlDbType.Int),
                                       new SqlParameter("@PageCount",SqlDbType.Int),
                                       new SqlParameter("@WareID",wareid),
                                       new SqlParameter("@KeyWords",keywords),
                                       new SqlParameter("@PageSize",pageSize),
                                       new SqlParameter("@PageIndex",pageIndex),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetDetailStocks", paras, CommandType.StoredProcedure, "Products");
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public DataSet GetProductsByKeywords(string wareid, string keywords, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@WareID",wareid),
                                       new SqlParameter("@KeyWords",keywords),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataSet ds = GetDataSet("P_GetProductsByKeywords_Stock", paras, CommandType.StoredProcedure, "Products");
            return ds;
        }

        public DataSet GetStorageDocDetails(string docid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@DocID",docid)
                                   };
            return GetDataSet("P_GetStorageDocDetails", paras, CommandType.StoredProcedure, "Doc|Details");
        }

        #endregion

        #region 添加

        public static bool AddStorageDoc(string docid, int doctype, string ids, string remark, string wareid, string userid, string operateip, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@DocID",docid),
                                     new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@DocType",doctype),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@WareID" , wareid),
                                     new SqlParameter("@AutoIDs" , ids),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@OperateIP" , operateip),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery("P_AddStorageDoc", paras, CommandType.StoredProcedure) > 0;
        }

        public static bool SubmitDamagedDoc(string docid, int doctype, decimal totalmoney, string remark, string ids, string userid, string operateip, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@DocID",docid),
                                     new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@DocType",doctype),
                                     new SqlParameter("@TotalMoney" , totalmoney),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@AutoIDs" , ids),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@OperateIP" , operateip),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery("P_SubmitDamagedDoc", paras, CommandType.StoredProcedure) > 0;
        }

        public static bool SubmitOverflowDoc(string docid, int doctype, string ids, string remark, string wareid, string userid, string operateip, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@DocID",docid),
                                     new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@DocType",doctype),
                                     new SqlParameter("@AutoIDs" , ids),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@WareID" , wareid),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@OperateIP" , operateip),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery("P_SubmitOverflowDoc", paras, CommandType.StoredProcedure) > 0;
        }

        public static bool AddPurchaseDoc(string docid, string productid, int doctype, string ids, string cmClientID, decimal totalfee, string remark, string wareid,
            int sourcetype, string userid, string clientid,string agentid,string personname,string mobilephone,string address,string citycode )
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@DocID",docid),
                                     new SqlParameter("@ProductID",productid),
                                     new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@DocType",doctype),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@WareID" , wareid),
                                     new SqlParameter("@SourceType",sourcetype), 
                                     new SqlParameter("@ProductDetails" , ids),
                                     new SqlParameter("@CMClientID" , cmClientID),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Address" , address), 
                                     new SqlParameter("@TotalMoney" , totalfee),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@PersonName" , personname),
                                     new SqlParameter("@MobilePhone" , mobilephone),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery("P_AddPurchaseDoc", paras, CommandType.StoredProcedure) > 0;
        }

        public static int AddDocPart(string orderid, string remarks, string nums, string userid,string zngcAutoID, ref string errinfo)
        {
            SqlParameter[] paras = {  
                                     new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                     new SqlParameter("@OrderID",orderid), 
                                     new SqlParameter("@BillingCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")), 
                                     new SqlParameter("@Nums",nums),
                                     new SqlParameter("@AutoID",zngcAutoID),
                                     new SqlParameter("@Remarks",remarks),
                                     new SqlParameter("@UserID",userid)
                                   };
            paras[0].Value = errinfo;
            paras[0].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_InsertStoreDocPart", paras, CommandType.StoredProcedure) ;
            errinfo = paras[0].Value.ToString();
            return bl;
        }
        public static bool AuditDocPart(string docid, string originid, int isover, string details, string remark, string userid, string operateip, string agentid, string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",SqlDbType.Int),
                                     new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                     new SqlParameter("@DocID",docid), 
                                     new SqlParameter("@OriginID",originid),
                                     new SqlParameter("@IsOver",isover),
                                     new SqlParameter("@ProductsDetails",details),
                                     new SqlParameter("@Remark",remark),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@OperateIP",operateip),
                                     new SqlParameter("@AgentID",agentid),
                                     new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;
            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_AuditStoreDocPart", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }

        public static bool AddIntfactoryPurchaseDoc(string goodsID,string goodsCode,string goodsName,string price,string productDetails,string cmClientID,string docID,
                                                    int docType,int sourceType,decimal totalMoney,string userID,string agentID,string clientID,
            string saleAttrStr = "", string productImage = "", string personName = "", string mobilePhone = "", string cityCode = "", string address = "", string remark = "",string wareID = "")
        {
            SqlParameter[] paras = {
                                      new SqlParameter("@GoodsID",goodsID),
                                      new SqlParameter("@GoodsCode",goodsCode),
                                      new SqlParameter("@GoodsName",goodsName),
                                      new SqlParameter("@Price",price),
                                      new SqlParameter("@SaleAttrStr",saleAttrStr),
                                      new SqlParameter("@ProductImage",productImage),
                                      new SqlParameter("@ProductDetails",productDetails),
                                      new SqlParameter("@CMClientID",cmClientID),
                                      new SqlParameter("@DocID",docID),
                                      new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                      new SqlParameter("@DocType",docType),
                                      new SqlParameter("@SourceType",sourceType),
                                      new SqlParameter("@TotalMoney",totalMoney),
                                      new SqlParameter("@PersonName",personName),
                                      new SqlParameter("@MobilePhone",mobilePhone),
                                      new SqlParameter("@CityCode",cityCode),
                                      new SqlParameter("@Address",address),
                                      new SqlParameter("@Remark",remark),
                                      new SqlParameter("@WareID", wareID),
                                      new SqlParameter("@UserID", userID),
                                      new SqlParameter("@AgentID",agentID),
                                      new SqlParameter("@ClientID",clientID)
                                   };
            return ExecuteNonQuery("P_AddIntfactoryPurchaseDoc", paras, CommandType.StoredProcedure) > 0;
        }
        #endregion

        #region 编辑、删除

        public bool UpdateStorageDetailWare(string docid, string autoid, string wareid, string depotid)
        {
            string sql = "update StorageDetail set WareID=@WareID,DepotID=@DepotID where DocID=@DocID and AutoID=@AutoID and Status=0";
            SqlParameter[] paras = { 
                                         new SqlParameter("@DocID",docid),
                                         new SqlParameter("@WareID",wareid),
                                         new SqlParameter("@DepotID",depotid),
                                         new SqlParameter("@AutoID",autoid)
                                   };
            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool UpdateStorageDetailBatch(string docid, string autoid, string batch)
        {
            string sql = "update StorageDetail set BatchCode=@BatchCode where DocID=@DocID and AutoID=@AutoID and Status=0";
            SqlParameter[] paras = { 
                                         new SqlParameter("@DocID",docid),
                                         new SqlParameter("@BatchCode",batch),
                                         new SqlParameter("@AutoID",autoid)
                                   };
            return ExecuteNonQuery(sql, paras, CommandType.Text) > 0;
        }

        public bool UpdateStorageStatus(string docid, int status, string remark, string userid, string operateip, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@DocID",docid),
                                     new SqlParameter("@Status",status),
                                     new SqlParameter("@Remark",remark),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@OperateIP",operateip),
                                     new SqlParameter("@ClientID",clientid)
                                   };
            return ExecuteNonQuery("P_UpdateStorageStatus", paras, CommandType.StoredProcedure) > 0;
        }

        public bool AuditStorageIn(string docid, int doctype, int isover, string details, string remark, string userid, string operateip, string agentid, string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",SqlDbType.Int),
                                     new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                     new SqlParameter("@DocID",docid),
                                     new SqlParameter("@BillingCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@DocType",doctype),
                                     new SqlParameter("@IsOver",isover),
                                     new SqlParameter("@ProductsDetails",details),
                                     new SqlParameter("@Remark",remark),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@OperateIP",operateip),
                                     new SqlParameter("@AgentID",agentid),
                                     new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;
            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_AuditStorageIn", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }

        public bool AuditReturnIn(string docid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                       new SqlParameter("@DocID",docid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_AuditReturnIn", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }

        public bool AuditDamagedDoc(string docid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                       new SqlParameter("@DocID",docid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_AuditDamagedDoc", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }

        public bool AuditOverflowDoc(string docid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ErrInfo",SqlDbType.NVarChar,500),
                                       new SqlParameter("@DocID",docid),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = result;
            paras[1].Value = errinfo;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            var bl = ExecuteNonQuery("P_AuditOverflowDoc", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            errinfo = paras[1].Value.ToString();
            return bl;
        }

        #endregion

    }
}
