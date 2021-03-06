﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class CustomDAL : BaseDAL
    {
        public static CustomDAL BaseProvider = new CustomDAL();

        #region 查询

        public DataSet GetCustomers(int searchtype, int type, string sourceid, string stageid, int status, int mark, string activityid, string searchuserid, string searchteamid, string searchagentid,
                                    string begintime, string endtime, string keyWords, string orderby, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid,int excelType=0)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@SearchType",searchtype),
                                       new SqlParameter("@Type",type),
                                       new SqlParameter("@SourceID",sourceid),
                                       new SqlParameter("@StageID",stageid),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@Mark",mark),
                                       new SqlParameter("@ActivityID",activityid),
                                       new SqlParameter("@SearchUserID",searchuserid),
                                       new SqlParameter("@SearchTeamID",searchteamid),
                                       new SqlParameter("@SearchAgentID",searchagentid),
                                       new SqlParameter("@BeginTime",begintime),
                                       new SqlParameter("@EndTime",endtime),
                                       new SqlParameter("@Keywords",keyWords),
                                       new SqlParameter("@OrderBy",orderby),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid),
                                       new SqlParameter("@ExcelType",excelType)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetCustomers", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;
        }


        public DataSet GetCustomersByKeywords(string keyWords, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@Keywords",keyWords),
                                       new SqlParameter("@UserID",userid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            DataSet ds = GetDataSet("P_GetCustomersByKeywords", paras, CommandType.StoredProcedure);
            return ds;
        }

        public DataSet GetCustomerByID(string customerid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@CustomerID",customerid),
                                       new SqlParameter("@AgentID", agentid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            return GetDataSet("P_GetCustomerByID", paras, CommandType.StoredProcedure, "Customer|Contact|Activity");
        }
  
        public DataTable GetContactsByCustomerID(string customerid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@CustomerID",customerid),
                                   };
            return GetDataTable("select * from Contact where CustomerID=@CustomerID and Status<>9 ", paras, CommandType.Text);
        }

        public DataTable GetContactByID(string id)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ContactID",id),
                                   };
            return GetDataTable("select * from Contact where ContactID=@ContactID", paras, CommandType.Text);
        }

        #endregion

        #region 添加

        public bool CreateCustomer(string customerid, string name, int type, string sourceid, string activityid, string industryid, int extent, string citycode, string address, string contactname, 
                                   string mobile, string officephone, string email, string jobs, string desc, string ownerid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@Name",name),
                                     new SqlParameter("@Type",type),
                                     new SqlParameter("@SourceID",sourceid),
                                     new SqlParameter("@ActivityID",activityid),
                                     new SqlParameter("@IndustryID" , industryid),
                                     new SqlParameter("@Extent" , extent),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@ContactName" , contactname),
                                     new SqlParameter("@MobilePhone" , mobile),
                                     new SqlParameter("@OfficePhone" , officephone),
                                     new SqlParameter("@Email" , email),
                                     new SqlParameter("@Jobs" , jobs),
                                     new SqlParameter("@Description" , desc),
                                     new SqlParameter("@OwnerID" , ownerid),
                                     new SqlParameter("@CreateUserID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_CreateCustomer", paras, CommandType.StoredProcedure) > 0;
        }

        public bool ExeclInsertCustomer(string customerid, string name, int type, string sourceid, string activityid, string industryid, int extent, string citycode, string address, string contactname,
                                  string mobile, string officephone, string email, string jobs, string desc, string ownerid, string operateid, string agentid, string clientid, int overType, int checkType)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@Name",name),
                                     new SqlParameter("@Type",type),
                                     new SqlParameter("@SourceID",sourceid),
                                     new SqlParameter("@ActivityID",activityid),
                                     new SqlParameter("@IndustryID" , industryid),
                                     new SqlParameter("@Extent" , extent),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@ContactName" , contactname),
                                     new SqlParameter("@MobilePhone" , mobile),
                                     new SqlParameter("@OfficePhone" , officephone),
                                     new SqlParameter("@Email" , email),
                                     new SqlParameter("@Jobs" , jobs),
                                     new SqlParameter("@Description" , desc),
                                     new SqlParameter("@OwnerID" , ownerid),
                                     new SqlParameter("@CreateUserID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid),
                                     new SqlParameter("@OverType" , overType),
                                     new SqlParameter("@CheckType" , checkType)
                                   };

            return ExecuteNonQuery("E_ImportCustomer", paras, CommandType.StoredProcedure) > 0;
        }


        public bool CreateContact(string contactid, string customerid, string name, string citycode, string address, string mobile, string officephone, string email, string jobs, string desc, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@ContactID",contactid),
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@Name",name),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@MobilePhone" , mobile),
                                     new SqlParameter("@OfficePhone" , officephone),
                                     new SqlParameter("@Email" , email),
                                     new SqlParameter("@Jobs" , jobs),
                                     new SqlParameter("@Description" , desc),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_CreateContact", paras, CommandType.StoredProcedure) > 0;
        }
        public bool ExcelInsertContact(string contactid, string customerid, string name, string citycode, string address, string mobile, string officephone, string email, string jobs, string desc, string userid, string agentid, string clientid,string companyName,int checkType,int overType)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@ContactID",contactid),
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@Name",name),
                                     new SqlParameter("@CompanyName",companyName),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@MobilePhone" , mobile),
                                     new SqlParameter("@OfficePhone" , officephone),
                                     new SqlParameter("@Email" , email),
                                     new SqlParameter("@Jobs" , jobs),
                                     new SqlParameter("@Description" , desc),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid),
                                     new SqlParameter("@CheckType" , checkType),
                                     new SqlParameter("@OverType" , overType)
                                   };

            return ExecuteNonQuery("E_ImportContact", paras, CommandType.StoredProcedure) > 0;
        }

        #endregion

        #region 编辑/删除

        public bool UpdateCustomer(string customerid, string name, int type, string industryid, int extent, string citycode, string address, string contactName,
                                   string mobile, string officephone, string email, string jobs, string desc, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@Name",name),
                                     new SqlParameter("@Type",type),
                                     new SqlParameter("@IndustryID" , industryid),
                                     new SqlParameter("@Extent" , extent),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@MobilePhone" , mobile),
                                     new SqlParameter("@OfficePhone" , officephone),
                                     new SqlParameter("@Email" , email),
                                     new SqlParameter("@ContactName" , contactName),
                                     new SqlParameter("@Jobs" , jobs),
                                     new SqlParameter("@Description" , desc),
                                     new SqlParameter("@CreateUserID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateCustomer", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateCustomerStage(string customerid, string stageid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@StageID",stageid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateCustomerStage", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateCustomerOwner(string customerid, string userid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateCustomerOwner", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateCustomerAgent(string customerid, string newagentid, string operateid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@NewAgentID",newagentid),
                                     new SqlParameter("@OperateID" , operateid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateCustomerAgent", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateContact(string contactid, string customerid, string name, string citycode, string address, string mobile, string officephone, string email, string jobs, string desc, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@ContactID",contactid),
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@Name",name),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@MobilePhone" , mobile),
                                     new SqlParameter("@OfficePhone" , officephone),
                                     new SqlParameter("@Email" , email),
                                     new SqlParameter("@Jobs" , jobs),
                                     new SqlParameter("@Description" , desc),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateContact", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateContactDefault(string contactid, string userid, string agentid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@ContactID",contactid),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery("P_UpdateContactDefault", paras, CommandType.StoredProcedure) > 0;
        }

        #endregion
    }
}
