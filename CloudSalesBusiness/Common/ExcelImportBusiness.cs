using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSalesDAL;
using CloudSalesEntity;

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

    }
}
