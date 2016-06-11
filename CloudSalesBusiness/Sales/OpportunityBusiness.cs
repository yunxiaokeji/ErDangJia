using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesEnum;
using CloudSalesDAL;
using CloudSalesEntity;


namespace CloudSalesBusiness
{
    public class OpportunityBusiness
    {
        public static OpportunityBusiness BaseBusiness = new OpportunityBusiness();

        #region 查询

        public List<OpportunityEntity> GetOpportunitys(EnumSearchType searchtype, string typeid, int status, string stageid, string searchuserid, string searchteamid, string searchagentid,
                                  string begintime, string endtime, string keyWords, string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OpportunityEntity> list = new List<OpportunityEntity>();
            DataSet ds = OpportunityDAL.BaseProvider.GetOpportunitys((int)searchtype, typeid, status, stageid, searchuserid, searchteamid, searchagentid, begintime, endtime, keyWords, orderBy, pageSize, pageIndex, ref totalCount, ref pageCount, userid, agentid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OpportunityEntity model = new OpportunityEntity();
                model.FillData(dr);
                model.OrderType = SystemBusiness.BaseBusiness.GetOrderTypeByID(model.TypeID, model.AgentID, model.ClientID);
                model.Stage = SystemBusiness.BaseBusiness.GetOpportunityStageByID(model.StageID, model.AgentID, model.ClientID);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                list.Add(model);
            }
            return list;
        }

        public List<OpportunityEntity> GetOpportunityaByCustomerID(string customerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OpportunityEntity> list = new List<OpportunityEntity>();
            DataTable dt = CommonBusiness.GetPagerData("Opportunity", "*", "CustomerID='" + customerid + "' and Status <> 9 ", "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
            foreach (DataRow dr in dt.Rows)
            {
                OpportunityEntity model = new OpportunityEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.Stage = SystemBusiness.BaseBusiness.GetOpportunityStageByID(model.StageID, model.AgentID, model.ClientID);

                list.Add(model);
            }
            return list;
        }

        public OpportunityEntity GetOpportunityByID(string opportunityid, string agentid, string clientid)
        {
            DataSet ds = OpportunityDAL.BaseProvider.GetOpportunityByID(opportunityid, agentid, clientid);
            OpportunityEntity model = new OpportunityEntity();
            if (ds.Tables["Opportunity"].Rows.Count > 0)
            {

                model.FillData(ds.Tables["Opportunity"].Rows[0]);
                model.OrderType = SystemBusiness.BaseBusiness.GetOrderTypeByID(model.TypeID, model.AgentID, model.ClientID);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStatus)model.Status);

                model.Stage = SystemBusiness.BaseBusiness.GetOpportunityStageByID(model.StageID, agentid, clientid);

                model.City = CommonBusiness.GetCityByCode(model.CityCode);

                if (ds.Tables["Customer"].Rows.Count > 0)
                {
                    model.Customer = new CustomerEntity();
                    model.Customer.FillData(ds.Tables["Customer"].Rows[0]);
                }
                model.Details = new List<OrderDetail>();
                if (ds.Tables["Details"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Details"].Rows)
                    {
                        OrderDetail detail = new OrderDetail();
                        detail.FillData(dr);
                        model.Details.Add(detail);
                    }
                }
            }
            return model;
        }

        public static List<ReplyEntity> GetReplys(string guid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<ReplyEntity> list = new List<ReplyEntity>();
            string whereSql = " Status<>9 and GUID='" + guid + "' ";
            DataTable dt = CommonBusiness.GetPagerData("OpportunityReply", "*", whereSql, "AutoID", "CreateTime desc ", pageSize, pageIndex, out totalCount, out pageCount, false);

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

        #region 添加、编辑、删除

        public string CreateOpportunity(string customerid, string typeid, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            string code = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            bool bl = OpportunityDAL.BaseProvider.CreateOpportunity(id, code, customerid, typeid, operateid, agentid, clientid);
            if (!bl)
            {
                return "";
            }
            else
            {
                //日志
                LogBusiness.AddActionLog(CloudSalesEnum.EnumSystemType.Client, CloudSalesEnum.EnumLogObjectType.Opportunity, EnumLogType.Create, "", operateid, agentid, clientid);
            }
            return id;
        }

        public static string CreateReply(string guid, string content, string userID, string agentID, string fromReplyID, string fromReplyUserID, string fromReplyAgentID)
        {
            return OpportunityDAL.BaseProvider.CreateReply(guid, content, userID, agentID, fromReplyID, fromReplyUserID, fromReplyAgentID);
        }

        public bool UpdateOpportunity(string opportunityid, string personName, string mobileTele, string cityCode, string address, string postalcode, string typeid, string remark, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OpportunityDAL.BaseProvider.UpdateOpportunity(opportunityid, personName, mobileTele, cityCode, address, postalcode, typeid, remark, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "编辑机会信息";
                LogBusiness.AddLog(opportunityid, EnumLogObjectType.Opportunity, msg, operateid, ip, operateid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOpportunityProductPrice(string opportunityid, string productid, string name, decimal price, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OpportunityDAL.BaseProvider.UpdateOpportunityProductPrice(opportunityid, productid, price, operateid, agentid, clientid);
            if (bl)
            {
                string msg = name + "的价格调整为：" + price;
                LogBusiness.AddLog(opportunityid, EnumLogObjectType.Opportunity, msg, operateid, ip, productid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOpportunityProductQuantity(string opportunityid, string productid, string name, int quantity, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OpportunityDAL.BaseProvider.UpdateOpportunityProductQuantity(opportunityid, productid, quantity, operateid, agentid, clientid);
            if (bl)
            {
                string msg = name + "的数量调整为：" + quantity;
                LogBusiness.AddLog(opportunityid, EnumLogObjectType.Opportunity, msg, operateid, ip, productid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOpportunityOwner(string opportunityid, string userid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OpportunityDAL.BaseProvider.UpdateOpportunityOwner(opportunityid, userid, operateid, agentid, clientid);
            if (bl)
            {
                var model = OrganizationBusiness.GetUserByUserID(userid, agentid);
                string msg = "拥有者更换为：" + model.Name;
                LogBusiness.AddLog(opportunityid, EnumLogObjectType.Opportunity, msg, operateid, ip, userid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOpportunityStage(string opportunityid, string stageid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OpportunityDAL.BaseProvider.UpdateOpportunityStage(opportunityid, stageid, operateid, agentid, clientid);
            if (bl)
            {
                var model = SystemBusiness.BaseBusiness.GetOpportunityStageByID(stageid, agentid, clientid);
                string msg = "机会阶段更换为：" + model.StageName;
                LogBusiness.AddLog(opportunityid, EnumLogObjectType.Opportunity, msg, operateid, ip, stageid, agentid, clientid);
            }
            return bl;
        }

        public bool SubmitOrder(string opportunityid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OpportunityDAL.BaseProvider.SubmitOrder(opportunityid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "转为订单";
                LogBusiness.AddLog(opportunityid, EnumLogObjectType.Opportunity, msg, operateid, ip, operateid, agentid, clientid);

                //日志
                LogBusiness.AddActionLog(CloudSalesEnum.EnumSystemType.Client, CloudSalesEnum.EnumLogObjectType.Orders, EnumLogType.Create, ip, operateid, agentid, clientid);
            }
            return bl;
        }

        public bool CloseOpportunity(string opportunityid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OpportunityDAL.BaseProvider.CloseOpportunity(opportunityid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "关闭机会";
                LogBusiness.AddLog(opportunityid, EnumLogObjectType.Opportunity, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        #endregion

    }
}
