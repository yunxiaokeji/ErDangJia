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

        public List<OpportunityEntity> GetOpportunitys(EnumSearchType searchtype, string typeid, string stageid, string searchuserid, string searchteamid, string searchagentid,
                                  string begintime, string endtime, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OpportunityEntity> list = new List<OpportunityEntity>();
            DataSet ds = OpportunityDAL.BaseProvider.GetOpportunitys((int)searchtype, typeid, stageid, searchuserid, searchteamid, searchagentid, begintime, endtime, keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, userid, agentid, clientid);
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

        #endregion

        #region 添加

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

        #endregion

    }
}
