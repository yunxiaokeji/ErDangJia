using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesEntity;
using CloudSalesEnum;
using CloudSalesDAL;

namespace CloudSalesBusiness
{
    public class ShoppingCartBusiness
    {
        public static int GetShoppingCartCount(EnumDocType ordertype, string guid)
        {
            object obj = 0;
            if (ordertype == EnumDocType.Opportunity)
            {
                obj = CommonBusiness.Select("OpportunityProduct", "count(0)", "OpportunityID='" + guid + "'");
            }
            else if (ordertype == EnumDocType.Order)
            {
                obj = CommonBusiness.Select("OrderDetail", "count(0)", "OrderID='" + guid + "'");
            }
            else
            {
                obj = CommonBusiness.Select("ShoppingCart", "count(0)", "ordertype=" + (int)ordertype + " and [GUID]='" + guid + "'");
            }
            return Convert.ToInt32(obj);
        }

        public static List<ProductDetail> GetShoppingCart(EnumDocType ordertype, string guid, string userid)
        {
            DataTable dt = ShoppingCartDAL.GetShoppingCart((int)ordertype, guid, userid);
            List<ProductDetail> list = new List<ProductDetail>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductDetail model = new ProductDetail();
                model.FillData(dr);

                list.Add(model);
            }
            return list;
        }

        public static bool AddShoppingCart(EnumDocType ordertype, string guid, string productid, string detailsid, string name, string unitid, int quantity, string remark, string userid, string operateip, string agentid, string clientid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = userid;
            }
            bool bl = ShoppingCartDAL.AddShoppingCart((int)ordertype, guid, productid, detailsid, quantity, remark, userid, operateip);
            if (bl)
            {
                string msg = "添加产品：" + name + " " + remark + " " + quantity + ProductsBusiness.BaseBusiness.GetUnitByID(unitid, clientid).UnitName;
                if (ordertype == EnumDocType.Opportunity)
                {
                    LogBusiness.AddLog(guid, EnumLogObjectType.Opportunity, msg, userid, operateip, userid, agentid, clientid);
                }
                else if (ordertype == EnumDocType.Order)
                {
                    LogBusiness.AddLog(guid, EnumLogObjectType.Orders, msg, userid, operateip, userid, agentid, clientid);
                }
            }
            return bl;
        }

        public static bool AddShoppingCartBatchOut(string productid, string detailsid, int quantity, string batchcode, string depotid, EnumDocType ordertype, string remark, string guid, string userid, string operateip)
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = userid;
            }
            return ShoppingCartDAL.AddShoppingCartBatchOut(productid, detailsid, quantity, (int)ordertype, batchcode, depotid, remark, guid, userid, operateip);
        }

        public static bool AddShoppingCartBatchIn(string productid, string detailsid, int quantity, EnumDocType ordertype, string remark, string guid, string userid, string operateip)
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = userid;
            }
            return ShoppingCartDAL.AddShoppingCartBatchIn(productid, detailsid, quantity, (int)ordertype, remark, guid, userid, operateip);
        }

        public static bool UpdateCartQuantity(string autoid, string guid, int quantity, string userid)
        {
            return CommonBusiness.Update("ShoppingCart", "Quantity", quantity, "AutoID=" + autoid + " and [GUID]='" + guid + "'");
        }

        public static bool UpdateCartBatch(string autoid, string guid, string batch, string userid)
        {
            return CommonBusiness.Update("ShoppingCart", "BatchCode", batch, "AutoID=" + autoid + " and [GUID]='" + guid + "'");
        }

        public static bool UpdateCartPrice(string autoid, string guid, decimal price, string userid)
        {
            return CommonBusiness.Update("ShoppingCart", "Price", price, "AutoID=" + autoid + " and [GUID]='" + guid + "'");
        }

        public static bool DeleteCart(EnumDocType ordertype, string guid, string productid, string name, string userid, string ip, string agentid, string clientid)
        {
            bool bl = ShoppingCartDAL.DeleteCart(guid, productid, (int)ordertype, userid);
            if (bl)
            {
                string msg = "移除产品：" + name;
                if (ordertype == EnumDocType.Opportunity)
                {
                    LogBusiness.AddLog(guid, EnumLogObjectType.Opportunity, msg, userid, ip, userid, agentid, clientid);
                }
                else if (ordertype == EnumDocType.Order)
                {
                    LogBusiness.AddLog(guid, EnumLogObjectType.Orders, msg, userid, ip, userid, agentid, clientid);
                }
            }
            return bl;
        }
    }
}
