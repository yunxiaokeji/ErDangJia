﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CloudSalesDAL;
using CloudSalesEntity;
using System.Data;
using CloudSalesEnum;

namespace CloudSalesBusiness
{
    public class CustomBusiness
    {
        public static CustomBusiness BaseBusiness = new CustomBusiness();

        #region 查询

        public static List<ExtentEntity> GetExtents()
        {
            List<ExtentEntity> list = new List<ExtentEntity>();
            list.Add(new ExtentEntity() { ExtentID = "1", ExtentName = "0-49人" });
            list.Add(new ExtentEntity() { ExtentID = "2", ExtentName = "50-99人" });
            list.Add(new ExtentEntity() { ExtentID = "3", ExtentName = "100-199人" });
            list.Add(new ExtentEntity() { ExtentID = "4", ExtentName = "200-499人" });
            list.Add(new ExtentEntity() { ExtentID = "5", ExtentName = "500-999人" });
            list.Add(new ExtentEntity() { ExtentID = "6", ExtentName = "1000人以上" });
            return list;
        }

        public List<CustomerEntity> GetCustomers(EnumSearchType searchtype, int type, string sourceid, string stageid, int status, int mark, string activityid, string searchuserid, string searchteamid, string searchagentid,
                                                 string begintime, string endtime, string keyWords, string orderby, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<CustomerEntity> list = new List<CustomerEntity>();
            DataTable dt = GetCustomersDatable(searchtype, type, sourceid, stageid, status, mark, activityid, searchuserid, searchteamid, searchagentid, begintime, endtime, 
                                                                keyWords, orderby, pageSize, pageIndex, ref totalCount, ref pageCount, userid, agentid, clientid);
            foreach (DataRow dr in dt.Rows)
            {
                CustomerEntity model = new CustomerEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.Source = SystemBusiness.BaseBusiness.GetCustomSourcesByID(model.SourceID, model.AgentID, model.ClientID);
                model.StageStatusStr = CommonBusiness.GetEnumDesc<EnumCustomStageStatus>((EnumCustomStageStatus)model.StageStatus);
                model.City = CommonBusiness.GetCityByCode(model.CityCode);
                list.Add(model);
            }
            return list;
        }
        
        public DataTable GetCustomersDatable(EnumSearchType searchtype, int type, string sourceid, string stageid, int status, int mark, string activityid, string searchuserid, string searchteamid, string searchagentid,
                                                string begintime, string endtime, string keyWords, string orderby, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid,int excelType=0)
        {
            DataTable dt=new DataTable();
            DataSet ds = CustomDAL.BaseProvider.GetCustomers((int)searchtype, type, sourceid, stageid, status, mark, activityid, searchuserid, searchteamid, searchagentid, begintime, endtime,
                                                                keyWords, orderby, pageSize, pageIndex, ref totalCount, ref pageCount, userid, agentid, clientid);
            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return dt;
        }
        
        public List<CustomerEntity> GetCustomersByActivityID(string activityid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount)
        {
            List<CustomerEntity> list = new List<CustomerEntity>();
            string sqlWhere = " ActivityID='" + activityid + "' and status<>9";

            DataTable dt = CommonBusiness.GetPagerData("Customer", "*", sqlWhere, "CustomerID", pageSize, pageIndex, out totalCount, out pageCount);
            foreach (DataRow dr in dt.Rows)
            {
                CustomerEntity model = new CustomerEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.Source = SystemBusiness.BaseBusiness.GetCustomSourcesByID(model.SourceID, model.AgentID, model.ClientID);
                model.StageStatusStr = CommonBusiness.GetEnumDesc<EnumCustomStageStatus>((EnumCustomStageStatus)model.StageStatus);
                //model.Stage = SystemBusiness.BaseBusiness.GetCustomStageByID(model.StageID, model.AgentID, model.ClientID);
                list.Add(model);
            }
            return list;
        }

        public List<CustomerEntity> GetCustomersByKeywords(string keywords, string userid,string agentid,string clientid)
        {
            List<CustomerEntity> list = new List<CustomerEntity>();
            DataSet ds = CustomDAL.BaseProvider.GetCustomersByKeywords(keywords, userid, agentid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CustomerEntity model = new CustomerEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.Source = SystemBusiness.BaseBusiness.GetCustomSourcesByID(model.SourceID, model.AgentID, model.ClientID);
                model.StageStatusStr = CommonBusiness.GetEnumDesc<EnumCustomStageStatus>((EnumCustomStageStatus)model.StageStatus);
                //model.Stage = SystemBusiness.BaseBusiness.GetCustomStageByID(model.StageID, model.AgentID, model.ClientID);
                list.Add(model);
            }
            return list;
        }

        public CustomerEntity GetCustomerByID(string customerid, string agentid, string clientid)
        {
            DataSet ds = CustomDAL.BaseProvider.GetCustomerByID(customerid, agentid, clientid);
            CustomerEntity model = new CustomerEntity();
            if (ds.Tables["Customer"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Customer"].Rows[0]);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.Source = SystemBusiness.BaseBusiness.GetCustomSourcesByID(model.SourceID, model.AgentID, model.ClientID);
                model.StageStatusStr = CommonBusiness.GetEnumDesc<EnumCustomStageStatus>((EnumCustomStageStatus)model.StageStatus);

                if (model.Extent > 0)
                {
                    model.ExtentStr = GetExtents().Where(m => m.ExtentID == model.Extent.ToString()).FirstOrDefault().ExtentName;
                }

                model.City = CommonBusiness.Citys.Where(m => m.CityCode == model.CityCode).FirstOrDefault();

                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);

                if (!string.IsNullOrEmpty(model.IndustryID))
                {
                    model.Industry = SystemBusiness.BaseBusiness.GetClientIndustryByID(model.IndustryID, model.AgentID, model.ClientID);
                }
                if (ds.Tables["Activity"].Rows.Count > 0)
                {
                    model.Activity = new ActivityEntity();
                    model.Activity.FillData(ds.Tables["Activity"].Rows[0]);
                }

                //if (ds.Tables["Contact"].Rows.Count > 0)
                //{
                //    model.Contacts = new List<ContactEntity>();
                //    foreach (DataRow dr in ds.Tables["Contact"].Rows)
                //    {
                //        ContactEntity con = new ContactEntity();
                //        con.FillData(dr);
                //        model.Contacts.Add(con);
                //    }
                    
                //}
            }
            return model;
        }

        public List<ContactEntity> GetContactsByCustomerID(string customerid, string agentid)
        {
            List<ContactEntity> list = new List<ContactEntity>();

            DataTable dt = CustomDAL.BaseProvider.GetContactsByCustomerID(customerid);
            foreach (DataRow dr in dt.Rows)
            {
                ContactEntity model = new ContactEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);
                model.City = CommonBusiness.Citys.Where(m => m.CityCode == model.CityCode).FirstOrDefault();
                list.Add(model);
            }

            return list;
        }

        public List<ContactEntity> GetContactsByCustomerID(string ownerID, string customerid = "", string agentid = "")
        {
            int total = 0;
            List<ContactEntity> list = new List<ContactEntity>(); 
            string cloumns = " status<>9 and OwnerID='" + ownerID + "'";
            if (!string.IsNullOrEmpty(agentid))
            {
                cloumns += " and agentid='" + agentid + "'";
            }
            if (!string.IsNullOrEmpty(customerid))
            {
                cloumns += " and customerid='" + customerid + "'";
            }
            DataTable dt = CommonBusiness.GetPagerData("Contact", "*", cloumns, "CustomerID", int.MaxValue, 1, out total, out total);
            foreach (DataRow dr in dt.Rows)
            {
                ContactEntity model = new ContactEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public ContactEntity GetContactByID(string contactid)
        {
            ContactEntity model = new ContactEntity();
            DataTable dt = CustomDAL.BaseProvider.GetContactByID(contactid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }
            return model;
        }

        public List<CustomerEntity> GetCustomerByOwneID(string ownerID, string agentid="", string clientid="")
        {
            int total = 0;
            List<CustomerEntity> list=new List<CustomerEntity>();
            string cloumns = " status<>9 and OwnerID='" + ownerID + "'";
            if (!string.IsNullOrEmpty(agentid))
            {
                cloumns += " and agentid='" +agentid+ "'";
            } 
            if (!string.IsNullOrEmpty(clientid))
            {
                cloumns += " and clientid='" + clientid + "'";
            }

           DataTable dt = CommonBusiness.GetPagerData("Customer", "*", cloumns, "CustomerID", int.MaxValue, 1, out total, out total);
           foreach (DataRow dr in dt.Rows)
           {
               CustomerEntity model = new CustomerEntity();
               model.FillData(dr); 
               list.Add(model);
           }
            return list;
        }

        #endregion

        #region 添加

        public string CreateCustomer(string name, int type, string sourceid, string activityid, string industryid, int extent, string citycode, string address, 
                                     string contactname, string mobile, string officephone, string email, string jobs, string desc, string ownerid, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            bool bl = CustomDAL.BaseProvider.CreateCustomer(id, name, type, sourceid, activityid, industryid, extent, citycode, address, contactname, mobile, officephone, email, jobs, desc, ownerid, operateid, agentid, clientid);
            if (!bl)
            {
                id = "";
            }
            else
            {
                //日志
                LogBusiness.AddActionLog(CloudSalesEnum.EnumSystemType.Client, CloudSalesEnum.EnumLogObjectType.Customer, EnumLogType.Create, "", operateid, agentid, clientid);
            }
            return id;
        }

        public string CreateContact(string customerid,string name, string citycode, string address, string mobile, string officephone, string email, string jobs, string desc, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            bool bl = CustomDAL.BaseProvider.CreateContact(id, customerid, name, citycode, address, mobile, officephone, email, jobs, desc, operateid, agentid, clientid);
            if (!bl)
            {
                id = "";
            }
            return id;
        }

        #endregion

        #region 编辑/删除

        public bool UpdateCustomer(string customerid, string name, int type, string industryid, int extent, string citycode, string address, string contactName, string mobile, string officephone, string email, string jobs, string desc, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomer(customerid, name, type, industryid, extent, citycode, address, contactName, mobile, officephone, email, jobs, desc, operateid, agentid, clientid);
            if (!bl)
            {
                string msg = "编辑客户信息";
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateCustomerOwner(string customerid, string userid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomerOwner(customerid, userid, operateid, agentid, clientid);
            if (bl)
            {
                var model = OrganizationBusiness.GetUserByUserID(userid, agentid);
                string msg = "客户负责人更换为：" + model.Name;
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, userid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateCustomerAgent(string customerid, string newagentid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomerAgent(customerid, newagentid, operateid, agentid, clientid);
            return bl;
        }

        public bool UpdateCustomerStatus(string customerid, EnumCustomStatus status, string operateid, string ip, string agentid, string clientid)
        {

            bool bl = CommonBusiness.Update("Customer", "Status", (int)status, "CustomerID='" + customerid + "'");
            if (bl)
            {
                var model = CommonBusiness.GetEnumDesc(status);
                string msg = "客户状态更换为：" + model;
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, status.ToString(), agentid, clientid);
            }
            return bl;
        }

        public bool UpdateCustomerMark(string customerid, int mark, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CommonBusiness.Update("Customer", "Mark", mark, "CustomerID='" + customerid + "'");
            if (bl)
            {
                string msg = "客户标记为：" + SystemBusiness.BaseBusiness.GetCustomerColorsColorID(clientid, mark).ColorName;
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, mark.ToString(), agentid, clientid);
            }
            return bl;
        }

        public bool UpdateContact(string contactid, string customerid, string name, string citycode, string address, string mobile, string officephone, string email, string jobs, string desc, string operateid, string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateContact(contactid, customerid, name, citycode, address, mobile, officephone, email, jobs, desc, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "联系人名称变更为：" + name + "，联系电话：" + mobile;
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, "", contactid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateContactDefault(string contactid, string customerid, string name, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateContactDefault(contactid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = name + "设为默认联系人";
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, operateid, ip, contactid, agentid, clientid);
            }
            return bl;
        }

        public bool DeleteContact(string contactid, string name, string customerid, string ip, string userid, string agentid, string clientid)
        {
            bool bl = CommonBusiness.Update("Contact", "Status", 9, "ContactID='" + contactid + "' and [Type]<>1");
            if (bl)
            {
                string msg = "删除联系人：" + name;
                LogBusiness.AddLog(customerid, EnumLogObjectType.Customer, msg, userid, ip, contactid, agentid, clientid);
            }
            return bl;
        }

        #endregion
    }
}
