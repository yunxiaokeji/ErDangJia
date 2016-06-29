using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class CustomerRPTDAL : BaseDAL
    {
        public static CustomerRPTDAL BaseProvider = new CustomerRPTDAL();

        public DataTable GetCustomerSourceScale(string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataTable dt = GetDataTable("R_GetCustomerSourceScale", paras, CommandType.StoredProcedure);
            return dt;
        } 
        public DataSet GetCustomerSourceDate(int type, string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@DateType",type),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@UserID",UserID),
                                       new SqlParameter("@TeamID",TeamID),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("R_GetCustomerSourceDate", paras, CommandType.StoredProcedure, "SourceData|DateName");
            return ds;
        }

        public DataSet GetCustomerStageRate(string begintime, string endtime, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("R_GetCustomerStageRate", paras, CommandType.StoredProcedure, "Data");
            return ds;
        }
        public DataSet GetCustomerStageRPT(string begintime, string endtime, int type, string clientid,string ownerid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@Type", type),
                                       new SqlParameter("@ClientID",clientid),
                                       new SqlParameter("@OwnerID",ownerid)
                                   };
            DataSet ds = GetDataSet("R_GetCustomerStageRPT", paras, CommandType.StoredProcedure, "Data|Source");
            return ds;
        }

        public int GetCustomerCountByTime(string begintime, string endtime, string clientid, string ownerid)
        {
            string sqlWhere = @" status<>9 ";
            if (!string.IsNullOrEmpty(begintime))
            {
                sqlWhere += " and CreateTime>='" + begintime + "'";
            }
            if (!string.IsNullOrEmpty(endtime))
            {
                sqlWhere += " and CreateTime<'" + endtime + " 23:59:59'";
            }
            if (!string.IsNullOrEmpty(endtime))
            {
                sqlWhere += " and ClientID='" + clientid + "'";
            }
            if (!string.IsNullOrEmpty(ownerid))
            {
                sqlWhere += " and OwnerID='" + ownerid + "'";
            }
            return (int) CommonDAL.Select("Customer", "count(1)", sqlWhere);

        }

        public DataTable GetCustomerReport(int type, string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
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
            DataTable dt = GetDataTable("R_GetCustomerReport", paras, CommandType.StoredProcedure);
            return dt;
        }

        public DataSet GetUserCustomers(string userid, string teamid, string begintime, string endtime, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@TeamID",teamid),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("R_GetUserCustomers", paras, CommandType.StoredProcedure, "Users");
            return ds;
        }

        public DataTable GetOpportunityStage(string clientID,string beginTime,string endTime,int  type,string ownerID)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ClientID",clientID), 
                                       new SqlParameter("@BeginTime",beginTime),
                                       new SqlParameter("@EndTime",endTime),
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@OwnerID",ownerID), 
                                   };
            DataTable ds = GetDataTable("R_GetOpportunityStateRPT", paras, CommandType.StoredProcedure);
            return ds;
            
        }
    }
}
