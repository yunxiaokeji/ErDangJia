using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSalesDAL;
using CloudSalesEntity;
using CloudSalesEnum;

namespace CloudSalesBusiness
{
    public class ExcelImportBusiness
    {
        
        public static string InsertCustomer(List<CustomerEntity> list,int type,int overType)
        {
            int handleCount = 0;
            list.ForEach(x =>
            {
                bool result
                =new CustomDAL().ExeclInsertCustomer(x.CustomerID, x.Name, x.Type, x.SourceID, x.ActivityID, x.IndustryID,
                    x.Extent, x.CityCode, x.Address,
                    x.ContactName, x.MobilePhone, x.OfficePhone, x.Email, x.Jobs, x.Description, x.OwnerID,
                    x.CreateUserID, x.AgentID, x.ClientID, overType, type);
                if (result)
                {
                    handleCount++;
                }
            });
            return handleCount > 0 ? "" : "导入失败,请联系管理员";
        }
        public static string InsertContact(List<ContactEntity> list, int type, int overType)
        {
            int handleCount = 0;
            list.ForEach(x =>
            {
                bool result
                = new CustomDAL().ExcelInsertContact(x.ContactID,x.CustomerID, x.Name,x.CityCode, x.Address,
                    x.MobilePhone, x.OfficePhone, x.Email, x.Jobs, x.Description, x.OwnerID,
                    x.AgentID, x.ClientID, x.CompanyName,overType, type);
                if (result)
                {
                    handleCount++;
                }
            });
            return handleCount > 0 ? "" : "导入失败,请联系管理员";
        }

        public static string InsertProduct(List<Products> list)
        { 
            string mes ="";
            list.ForEach(x =>
            {
                string result = "";
                string pid
                    = new ProductsDAL().InsertProductExcel(x.ProductCode, x.ProductName, x.GeneralName, (x.IsCombineProduct == 1), x.BrandID,
                    x.BigUnitID,x.UnitID,x.BigSmallMultiple.Value,x.CategoryID,x.Status.Value,x.AttrList,x.ValueList,x.AttrValueList,
                    x.CommonPrice.Value,x.Price,x.Weight.Value,(x.IsNew==1),(x.IsRecommend==1),x.IsAllow,x.IsAutoSend,x.EffectiveDays.Value,
                    x.DiscountValue.Value, x.WarnCount, x.ProductImage, x.ShapeCode, x.Description, x.CreateUserID, x.ClientID, ref result);
                if (string.IsNullOrEmpty(result))
                {
                    LogBusiness.AddActionLog(CloudSalesEnum.EnumSystemType.Client,
                        CloudSalesEnum.EnumLogObjectType.Product, EnumLogType.Create, "", x.CreateUserID, "", x.ClientID);
                }
                else
                {
                    mes += result+",";
                }
            });
            return string.IsNullOrEmpty(mes)  ? "" : mes;
        }

    }
}
