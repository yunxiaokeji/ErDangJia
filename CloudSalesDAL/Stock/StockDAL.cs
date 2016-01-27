﻿using System;
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

        public DataTable GetProviders(string clientID)
        {
            SqlParameter[] paras = { new SqlParameter("@ClientID", clientID) };
            DataTable dt = GetDataTable("select ProviderID,Name from Providers where ClientID=@ClientID and Status<>9", paras, CommandType.Text);
            return dt;

        }

        public DataTable GetProviderByID(string ProviderID)
        {
            SqlParameter[] paras = { new SqlParameter("@ProviderID", ProviderID) };
            DataTable dt = GetDataTable("select * from Providers where ProviderID=@ProviderID", paras, CommandType.Text);
            return dt;
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
            DataSet ds = GetDataSet("P_GetStorageDocList", paras, CommandType.StoredProcedure, "Doc");
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

        #endregion

        #region 添加

        public string AddProviders(string name, string contact, string mobile, string email, string cityCode, string address, string remark, string operateID, string agentid, string clientID)
        {
            string id = Guid.NewGuid().ToString();
            string sqlText = "insert into Providers(ProviderID,Name,Contact,MobileTele,Email,Website,CityCode,Address,Remark,CreateTime,CreateUserID,AgentID,ClientID)"
                                      + "values(@ProviderID ,@Name,@Contact ,@MobileTele,@Email,'',@CityCode,@Address,@Remark,getdate(),@CreateUserID,@AgentID,@ClientID)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ProviderID" , id),
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@Contact" , contact),
                                     new SqlParameter("@MobileTele" , mobile),
                                     new SqlParameter("@Email" , email),
                                     new SqlParameter("@CityCode" , cityCode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@CreateUserID" , operateID),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientID)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0 ? id : "";

        }

        public static bool AddStorageDoc(string docid, int doctype, decimal totalmoney, string providerid, string remark, string wareid, string userid, string operateip, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@DocID",docid),
                                     new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@DocType",doctype),
                                     new SqlParameter("@TotalMoney" , totalmoney),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@WareID" , wareid),
                                     new SqlParameter("@ProviderID" , providerid),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@OperateIP" , operateip),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery("P_AddStorageDoc", paras, CommandType.StoredProcedure) > 0;
        }

        public static bool SubmitDamagedDoc(string docid, int doctype, decimal totalmoney, string remark, string wareid, string userid, string operateip, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@DocID",docid),
                                     new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@DocType",doctype),
                                     new SqlParameter("@TotalMoney" , totalmoney),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@WareID" , wareid),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@OperateIP" , operateip),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery("P_SubmitDamagedDoc", paras, CommandType.StoredProcedure) > 0;
        }

        public static bool SubmitOverflowDoc(string docid, int doctype, decimal totalmoney, string remark, string wareid, string userid, string operateip, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@DocID",docid),
                                     new SqlParameter("@DocCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@DocType",doctype),
                                     new SqlParameter("@TotalMoney" , totalmoney),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@WareID" , wareid),
                                     new SqlParameter("@UserID" , userid),
                                     new SqlParameter("@OperateIP" , operateip),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery("P_SubmitOverflowDoc", paras, CommandType.StoredProcedure) > 0;
        }


        #endregion

        #region 编辑、删除

        public bool UpdateProvider(string providerid, string name, string contact, string mobile, string email, string cityCode, string address, string remark, string operateID, string agentid, string clientID)
        {
            string sqlText = "Update Providers set [Name]=@Name,[Contact]=@Contact ,[MobileTele]=@MobileTele,[CityCode]=@CityCode," +
                "[Address]=@Address,[Remark]=@Remark where [ProviderID]=@ProviderID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@Contact" , contact),
                                     new SqlParameter("@MobileTele" , mobile),
                                     new SqlParameter("@CityCode" , cityCode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@ProviderID" , providerid),
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

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

        public bool AuditStorageIn(string autoid, string userid, string operateip, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@AutoID",autoid),
                                     new SqlParameter("@BillingCode",DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@OperateIP",operateip),
                                     new SqlParameter("@AgentID",agentid),
                                     new SqlParameter("@ClientID",clientid)
                                   };
            return ExecuteNonQuery("P_AuditStorageIn", paras, CommandType.StoredProcedure) > 0;
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