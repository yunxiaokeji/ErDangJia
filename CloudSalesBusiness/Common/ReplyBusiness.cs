using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CloudSalesDAL;
using System.Threading.Tasks;
using CloudSalesEnum;
using CloudSalesEntity;
using System.Data;

namespace CloudSalesBusiness
{
    public class ReplyBusiness
    {
        public static ReplyBusiness BaseBusiness = new ReplyBusiness();

        #region 查询

        public static List<ReplyEntity> GetReplys(string guid, EnumLogObjectType type, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid)
        {
            string tablename = "";
            switch (type)
            {
                case EnumLogObjectType.Activity:
                    tablename = "ActivityReply";
                    break;
                case EnumLogObjectType.Customer:
                    tablename = "CustomerReply";
                    break;
                case EnumLogObjectType.Opportunity:
                    tablename = "OpportunityReply";
                    break;
                case EnumLogObjectType.Orders:
                    tablename = "OrderReply";
                    break;
            }

            List<ReplyEntity> list = new List<ReplyEntity>();
            string whereSql = " Status<>9 and GUID='" + guid + "' ";
            DataTable dt = CommonBusiness.GetPagerData(tablename, "*", whereSql, "AutoID", "CreateTime desc ", pageSize, pageIndex, out totalCount, out pageCount, false);

            foreach (DataRow dr in dt.Rows)
            {
                ReplyEntity model = new ReplyEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);
                if (!string.IsNullOrEmpty(model.FromReplyID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }
                list.Add(model);
            }

            return list;
        }

        #endregion

        #region 添加.删除

        public static string CreateReply(EnumLogObjectType type, string guid, string content, string userID, string agentID, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            switch (type)
            {
                case EnumLogObjectType.Activity:
                    return ReplyDAL.BaseProvider.CreateActivityReply(guid, content, userID, agentID, fromReplyID, fromReplyUserID, fromReplyAgentID);

                case EnumLogObjectType.Customer:
                    return ReplyDAL.BaseProvider.CreateCustomerReply(guid, content, userID, agentID, fromReplyID, fromReplyUserID, fromReplyAgentID);

                case EnumLogObjectType.Opportunity:
                    return ReplyDAL.BaseProvider.CreateOpportunityReply(guid, content, userID, agentID, fromReplyID, fromReplyUserID, fromReplyAgentID);

                case EnumLogObjectType.Orders:
                    return ReplyDAL.BaseProvider.CreateOrderReply(guid, content, userID, agentID, fromReplyID, fromReplyUserID, fromReplyAgentID);

            }
            return "";
            
        }

        public static bool DeleteReply(EnumLogObjectType type, string replyid)
        {
            string tablename = "";
            switch (type)
            {
                case EnumLogObjectType.Activity:
                    tablename = "ActivityReply";
                    break;
                case EnumLogObjectType.Customer:
                    tablename = "CustomerReply";
                    break;
                case EnumLogObjectType.Opportunity:
                    tablename = "OpportunityReply";
                    break;
                case EnumLogObjectType.Orders:
                    tablename = "OrderReply";
                    break;
            }

            bool bl = CommonBusiness.Update(tablename, "Status", 9, "ReplyID='" + replyid + "'");
            return bl;
        }

        #endregion
    }
}
