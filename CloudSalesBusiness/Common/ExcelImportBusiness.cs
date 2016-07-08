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

        public static string InsertProduct(List<Products> list,Dictionary<int, PicturesInfo> imgList=null)
        {
            int i = 0;
            string mes ="";
            list.ForEach(x =>
            {
                if (imgList != null && imgList.Any())
                {
                    if (imgList.ContainsKey(i))
                    {
                        DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath(FILEPATH));
                        if (!directory.Exists)
                        {
                            directory.Create();
                        }
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(imgList[i].PictureData);  
                        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                        string fileName = x.ProductCode+DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                        img.Save(HttpContext.Current.Server.MapPath(FILEPATH) + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        x.ProductImage = FILEPATH + fileName;
                    }
                }
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
                i++;
            });
            return string.IsNullOrEmpty(mes)  ? "" : mes;
        }

    }
}
