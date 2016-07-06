using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesEnum;
using CloudSalesDAL;
using CloudSalesEntity;
using System.Data;
using System.Data.SqlClient;

namespace CloudSalesBusiness
{
    public class OrdersBusiness
    {
        public static OrdersBusiness BaseBusiness = new OrdersBusiness();

        #region 查询

        public List<OrderEntity> GetOrders(EnumSearchType searchtype, string typeid, int status, int paystatus, int invoicestatus, int returnstatus, string searchuserid, string searchteamid, string searchagentid,
                                                string begintime, string endtime, string keyWords, string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataSet ds = OrdersDAL.BaseProvider.GetOrders((int)searchtype, typeid, status, paystatus, invoicestatus, returnstatus, searchuserid, searchteamid, searchagentid, begintime, endtime, keyWords, orderBy, pageSize, pageIndex, ref totalCount, ref pageCount, userid, agentid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);
                model.OrderType = SystemBusiness.BaseBusiness.GetOrderTypeByID(model.TypeID, model.AgentID, model.ClientID);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStatus)model.Status);
                if (model.Status == 2)
                {
                    model.SendStatusStr = CommonBusiness.GetEnumDesc((EnumSendStatus)model.SendStatus);
                }
                else if (model.Status < 2)
                {
                    model.SendStatusStr = "--";
                }
                list.Add(model);
            }
            return list;
        }


        public List<OrderEntity> GetOrdersByCustomerID(string customerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<OrderEntity> list = new List<OrderEntity>();
            DataTable dt = CommonBusiness.GetPagerData("Orders", "*", "CustomerID='" + customerid + "' and Status<>9 ", "AutoID", pageSize, pageIndex, out totalCount, out pageCount, false);
            foreach (DataRow dr in dt.Rows)
            {
                OrderEntity model = new OrderEntity();
                model.FillData(dr);
                model.OrderType = SystemBusiness.BaseBusiness.GetOrderTypeByID(model.TypeID, model.AgentID, model.ClientID);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStatus)model.Status);

                list.Add(model);
            }
            return list;
        }

        public OrderEntity GetOrderByID(string orderid, string agentid, string clientid)
        {
            DataSet ds = OrdersDAL.BaseProvider.GetOrderByID(orderid, agentid, clientid);
            OrderEntity model = new OrderEntity();
            if (ds.Tables["Order"].Rows.Count > 0)
            {
                
                model.FillData(ds.Tables["Order"].Rows[0]);
                model.OrderType = SystemBusiness.BaseBusiness.GetOrderTypeByID(model.TypeID, model.AgentID, model.ClientID);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);

                model.StatusStr = CommonBusiness.GetEnumDesc((EnumOrderStatus)model.Status);
                model.ExpressTypeStr = CommonBusiness.GetEnumDesc((EnumExpressType)model.ExpressType);

                if (model.Status == 2)
                {
                    model.SendStatusStr = CommonBusiness.GetEnumDesc((EnumSendStatus)model.SendStatus);
                }
                else if (model.Status < 2)
                {
                    model.SendStatusStr = "--";
                }

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

        #endregion

        #region 添加

        public string CreateOrder(string customerid, string typeid, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            string code = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            bool bl = OrdersDAL.BaseProvider.CreateOrder(id, code, customerid, typeid, operateid, agentid, clientid);
            if (!bl)
            {
                return "";
            }
            else
            {
                //日志
                LogBusiness.AddActionLog(CloudSalesEnum.EnumSystemType.Client, CloudSalesEnum.EnumLogObjectType.Orders, EnumLogType.Create, "", operateid, agentid, clientid);
            }
            return id;
        }

        #endregion

        #region 编辑、删除

        public bool UpdateOrderProductPrice(string orderid, string productid, string name, decimal price, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderProductPrice(orderid, productid, price, operateid, agentid, clientid);
            if (bl)
            {
                string msg = name + "的价格调整为：" + price;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, productid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderProductQuantity(string orderid, string productid, string name, int quantity, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderProductQuantity(orderid, productid, quantity, operateid, agentid, clientid);
            if (bl)
            {
                string msg = name + "的数量调整为：" + quantity;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, productid, agentid, clientid);
            }
            return bl;
        }

        public bool DeleteOrder(string orderid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.DeleteOrder(orderid, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "删除订单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateOrderOwner(string orderid, string userid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateOrderOwner(orderid, userid, operateid, agentid, clientid);
            if (bl)
            {
                var model = OrganizationBusiness.GetUserByUserID(userid, agentid);
                string msg = "负责人更换为：" + model.Name;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, userid, agentid, clientid);
            }
            return bl;
        }

        public bool EditOrder(string orderid, string personName, string mobileTele, string cityCode, string address, string postalcode, string typeid, int expresstype, string remark, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.EditOrder(orderid, personName, mobileTele, cityCode, address, postalcode, typeid, expresstype, remark, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "编辑订单信息";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, operateid, agentid, clientid);
            }
            return bl;
        }

        public bool EffectiveOrder(string orderid, string operateid, string ip, string agentid, string clientid,out int result)
        {
            bool bl = OrdersDAL.BaseProvider.EffectiveOrder(orderid, operateid, agentid, clientid, out result);
            if (bl)
            {
                string msg = "审核订单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool ApplyReturnOrder(string orderid, string operateid, string ip, string agentid, string clientid, out int result)
        {
            bool bl = OrdersDAL.BaseProvider.ApplyReturnOrder(orderid, operateid, agentid, clientid, out result);
            if (bl)
            {
                string msg = "申请退单";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateReturnQuantity(string orderid, string autoid, string name, int quantity, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = OrdersDAL.BaseProvider.UpdateReturnQuantity(orderid, autoid, quantity, operateid, agentid, clientid);
            if (bl)
            {
                string msg = "修改产品" + name + "退货数量：" + quantity;
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, autoid, agentid, clientid);
            }
            return bl;
        }

        public bool ApplyReturnProduct(string orderid, string operateid, string ip, string agentid, string clientid, out int result)
        {
            bool bl = OrdersDAL.BaseProvider.ApplyReturnProduct(orderid, operateid, agentid, clientid, out result);
            if (bl)
            {
                string msg = "申请退货";
                LogBusiness.AddLog(orderid, EnumLogObjectType.Orders, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        #endregion
    }
}
