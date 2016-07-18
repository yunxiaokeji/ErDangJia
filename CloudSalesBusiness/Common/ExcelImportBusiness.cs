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

        public static string AddProduct(List<Products> list, string agentid = "")
        {
            string mes = "";
            list.ForEach(x =>
            {
                int result = 0;
                string pid
                    = ProductsBusiness.BaseBusiness.AddProduct(x.ProductCode, x.ProductName, x.GeneralName, (x.IsCombineProduct == 1), x.ProviderID, x.BrandID,
                    x.BigUnitID, x.UnitID, x.BigSmallMultiple.Value, x.CategoryID, x.Status.Value, x.AttrList, x.ValueList, x.AttrValueList,
                    x.CommonPrice.Value, x.Price, x.Weight.Value, (x.IsNew == 1), (x.IsRecommend == 1), x.IsAllow, x.IsAutoSend, x.EffectiveDays.Value,
                    x.DiscountValue.Value, x.WarnCount, x.ProductImage, x.ShapeCode, x.Description, x.ProductDetails, x.CreateUserID, agentid, x.ClientID, out result);
                if (result != 1)
                {
                    mes += result == 3 ? "编码" + x.ProductCode + "已存在," : result == 2 ? "条形码" + x.ShapeCode + "已存在," : "";
                    if (result == 2 || result == 3)
                    {
                        pid=ProductsBusiness.BaseBusiness.GetProductCode(x.ProductCode,(result == 3?"":x.ShapeCode), x.ClientID);
                        if (!string.IsNullOrEmpty(pid))
                        {
                            foreach (var model in x.ProductDetails)
                            {
                                ProductsBusiness.BaseBusiness.AddProductDetails(pid, model.DetailsCode, model.ShapeCode,
                                    model.SaleAttr, model.AttrValue, model.SaleAttrValue, model.Price, model.Weight,
                                    model.BigPrice, model.ImgS, model.Remark, model.Description, x.CreateUserID,
                                    x.ClientID, out result);
                            }
                        }
                    }
                }
            });
            return  mes;
        }

        public static string AddCategoryList(List<Category> list)
        {
            string mes = "";
           list.ForEach(x =>
           {
               AddCategory(x, ref mes);
           });
            if (!string.IsNullOrEmpty(mes))
            {
                mes += "类别编码已存在";
            }
            return mes;
        }

        public static string AddCategory(Category category,ref string mes)
        {
            int result = 0;
            Category newCategory= ProductsBusiness.BaseBusiness.AddCategory(category.CategoryCode, category.CategoryName, category.PID,
                category.Status.Value, "", "", "", category.CreateUserID, category.ClientID, out result);
            if (result != 1 )
            {
                if (mes.IndexOf(category.CategoryCode) == -1)
                {
                    mes += category.CategoryCode + ",";
                }
            }
            else
            {
                string tempmes = "";
                category.ChildCategorys.ForEach(x =>
                {
                    string refmes = "";
                    x.PID = newCategory.CategoryID;
                    tempmes = AddCategory(x, ref refmes);
                });
                if (!string.IsNullOrEmpty(tempmes) && mes.IndexOf(tempmes)==-1)
                {
                    mes += tempmes + "";
                }
            }
            return mes;
        }
    }
}
