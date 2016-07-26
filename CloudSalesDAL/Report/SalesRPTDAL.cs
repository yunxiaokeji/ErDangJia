using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class SalesRPTDAL : BaseDAL
    {
        public static SalesRPTDAL BaseProvider = new SalesRPTDAL();

        public DataSet GetUserOrders(string userid, string teamid, string begintime, string endtime, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@TeamID",teamid),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("R_GetUserOrders", paras, CommandType.StoredProcedure, "Users");
            return ds;
        }

        public DataTable GetOrderMapReport(int type, string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataTable dt = GetDataTable("R_GetOrderMapReport", paras, CommandType.StoredProcedure);
            return dt;
        }

        public DataSet GetOpportunityStageRate(string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("R_GetOpportunityStageRate", paras, CommandType.StoredProcedure, "Data");
            return ds;
        }

        public DataSet GetUserOpportunitys(string userid, string teamid, string begintime, string endtime, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@TeamID",teamid),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("R_GetUserOpportunitys", paras, CommandType.StoredProcedure, "Users");
            return ds;
        }


        public DataTable GetaOrderDetailReport(int pageSize, int pageIndex, string clientid, string keyWords, string begintime, string endtime,string orderBy, string customerid, ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = {  
                                       new SqlParameter("@totalCount",DbType.Int32),
                                       new SqlParameter("@pageCount",DbType.Int32),
                                       new SqlParameter("@clientID",clientid),
                                       new SqlParameter("@keyWords",keyWords),
                                       new SqlParameter("@beginTime",begintime),
                                       new SqlParameter("@endTime",endtime),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@orderBy",orderBy),
                                       new SqlParameter("@customerID", customerid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;
            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataTable dt=  GetDataTable("R_GetOrderDetailReport", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return dt;
        }

        public DataTable GetaStockDetailReport(int pageSize, int pageIndex, string clientid, string keyWords, string begintime, string endtime,ref int totalCount, ref int pageCount)
        {
            SqlParameter[] paras = {  
                                       new SqlParameter("@totalCount",DbType.Int32),
                                       new SqlParameter("@pageCount",DbType.Int32),
                                       new SqlParameter("@clientID",clientid),
                                       new SqlParameter("@keyWords",keyWords),
                                       new SqlParameter("@beginTime",begintime),
                                       new SqlParameter("@endTime",endtime),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@pageSize",pageSize) 
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;
            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataTable dt = GetDataTable("R_StockInOutReport", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return dt;
        }
    }
}
