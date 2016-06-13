using CloudSalesEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSalesDAL.Custom;
using CloudSalesEntity;

namespace CloudSalesBusiness.Custom
{
    public class CustomerColorBusiness
    {
        public static List<CustomerColorEntity> GetCustomerColors(  string agentid, string clientid)
        {
            int totalCount = 0;
            List<CustomerColorEntity> list = new List<CustomerColorEntity>();
            string whereSql = " status<>9 "; 
            if (!string.IsNullOrEmpty(agentid))
            {
                whereSql += " and agentid='" + agentid + "' ";
            }
            if (!string.IsNullOrEmpty(clientid))
            {
                whereSql += " and clientid='" + clientid + "' ";
            }
            DataTable dt = CommonBusiness.GetPagerData("CustomerColor", "*", whereSql, "AutoID", "CreateTime desc ", int.MaxValue, 1, out totalCount, out totalCount, false);

            foreach (DataRow dr in dt.Rows)
            {
                CustomerColorEntity model = new CustomerColorEntity();
                model.FillData(dr);
                //model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);
                //if (!string.IsNullOrEmpty(model.CreateUserID))
                //{
                //    model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);
                //}
                list.Add(model);
            }

            return list;

        }
        #region 新增 编辑 删除 已移动到SystemBusiness中
        /*
        public static int CreateCustomerColor(string colorName,   string colorValue, string customerid, string agentid, string clientid, string userid, int status=0)
        {
            return CustomerColorDAL.BaseProvider.InsertCustomerColor(colorName,   colorValue, customerid, agentid,
                clientid, userid, status);
        }

      
        public static bool UpdateCustomerColor(string agentid, string clientid, int colorid, string colorName, string colorValue,string updateuserid)
        {
            return CustomerColorDAL.BaseProvider.UpdateCustomerColor(  agentid, clientid, colorid, colorName, colorValue, updateuserid);
        }

        public static bool UpdateStatus(int status,int colorid,  string agentid, string clientid, string updateuserid)
        {
            return CustomerColorDAL.BaseProvider.UpdateStatus(status, colorid, agentid, clientid, updateuserid);
        }
        */
        #endregion
    }
}
