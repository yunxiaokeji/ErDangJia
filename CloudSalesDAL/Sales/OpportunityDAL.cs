using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class OpportunityDAL : BaseDAL
    {
        public static OpportunityDAL BaseProvider = new OpportunityDAL();

        public DataSet GetOpportunitys(int searchtype, string typeid, string stageid, string searchuserid, string searchteamid, string searchagentid, string begintime, string endtime,
                string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@SearchType",searchtype),
                                       new SqlParameter("@TypeID",typeid),
                                       new SqlParameter("@StageID",stageid),
                                       new SqlParameter("@SearchUserID",searchuserid),
                                       new SqlParameter("@SearchTeamID",searchteamid),
                                       new SqlParameter("@SearchAgentID",searchagentid),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@Keywords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetOpportunitys", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }

        public bool CreateOpportunity(string opportunityid, string opportunitycode, string customerid, string typeid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@OpportunityID",opportunityid),
                                     new SqlParameter("@OpportunityCode",opportunitycode),
                                     new SqlParameter("@TypeID",typeid),
                                     new SqlParameter("@CustomerID" , customerid),
                                     new SqlParameter("@UserID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery("P_CreateOpportunity", paras, CommandType.StoredProcedure) > 0;
        }

        public string CreateReply(string guid, string content, string userID, string agentID, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            string replyID = Guid.NewGuid().ToString();

            SqlParameter[] paras = { 
                                     new SqlParameter("@ReplyID",replyID),
                                     new SqlParameter("@GUID",guid),
                                     new SqlParameter("@Content",content),
                                     new SqlParameter("@FromReplyID",fromReplyID),
                                     new SqlParameter("@CreateUserID" , userID),
                                     new SqlParameter("@AgentID" , agentID),
                                     new SqlParameter("@FromReplyUserID" , fromReplyUserID),
                                     new SqlParameter("@FromReplyAgentID" , fromReplyAgentID),
                                   };

            return ExecuteNonQuery("P_CreateOpportunityReply", paras, CommandType.StoredProcedure) > 0 ? replyID : string.Empty;
        }
    }
}
