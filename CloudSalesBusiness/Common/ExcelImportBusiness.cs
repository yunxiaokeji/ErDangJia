using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CloudSalesDAL;
using CloudSalesEntity;
using CloudSalesEnum;

namespace CloudSalesBusiness
{
    public class ExcelImportBusiness
    {

        /// <summary>
        /// 文件默认存储路径
        /// </summary>
        public static string FILEPATH = CloudSalesTool.AppSettings.Settings["UploadFilePath"] + "Product/" + DateTime.Now.ToString("yyyyMM") + "/";
        public static string TempPath = CloudSalesTool.AppSettings.Settings["UploadTempPath"];

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
