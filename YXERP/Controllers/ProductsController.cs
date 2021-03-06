﻿using CloudSalesBusiness;
using CloudSalesEntity;
using CloudSalesEnum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YXERP.Models;

namespace YXERP.Controllers
{
    public class ProductsController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Brand() 
        {
            return View();
        }

        public ActionResult Unit() 
        {
            ViewBag.Items = ProductsBusiness.BaseBusiness.GetClientUnits(CurrentUser.ClientID).Where(m => m.Status == 1).ToList();
            return View();
        }

        public ActionResult Category() 
        {
            var list = new ProductsBusiness().GetChildCategorysByID("", CurrentUser.ClientID);
            ViewBag.Items = list;
            return View();
        }

        public ActionResult Attr() 
        {
            return View();
        }

        public ActionResult ProductAdd(string id) 
        {
            ViewBag.Model = new ProductsBusiness().GetCategoryDetailByID(id);
            ViewBag.BrandList = new ProductsBusiness().GetBrandList(CurrentUser.ClientID);
            ViewBag.UnitList = new ProductsBusiness().GetClientUnits(CurrentUser.ClientID);
            return View();
        }

        public ActionResult ProductDetail(string id)
        {
            var model = new ProductsBusiness().GetProductByID(id);
            ViewBag.Model = model;
            ViewBag.BrandList = new ProductsBusiness().GetBrandList(CurrentUser.ClientID);
            ViewBag.UnitList = new ProductsBusiness().GetClientUnits(CurrentUser.ClientID);
            return View();
        }

        public ActionResult ProductDetails(string id)
        {
            var model = new ProductsBusiness().GetProductByID(id);
            ViewBag.Model = model;
            return View();
        }

        public ActionResult ProductList() 
        {
            return View();
        }

        public ActionResult ProductImport()
        {
            return View();
        }

        public ActionResult ChooseDetail(string pid, string did, int type = 0, string guid = "")
        {
            if (string.IsNullOrEmpty(pid))
            {
                return Redirect("ProductList");
            }
            var model = new ProductsBusiness().GetProductByIDForDetails(pid);
            if (model == null || string.IsNullOrEmpty(model.ProductID))
            {
                return Redirect("ProductList");
            }
            ViewBag.Model = model;
            ViewBag.DetailID = did;
            ViewBag.OrderType = type;
            ViewBag.GUID = guid;
            return View();
        }

        public ActionResult Providers()
        {
            return View();
        }

        #region Ajax

        #region 品牌

        public JsonResult SavaBrand(string brand)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Brand model = serializer.Deserialize<Brand>(brand);

            string brandID = "";
            if (string.IsNullOrEmpty(model.BrandID))
            {
                brandID = new ProductsBusiness().AddBrand(model.Name, model.AnotherName, model.IcoPath, model.CountryCode, model.CityCode, model.Status.Value, model.Remark, model.BrandStyle, OperateIP, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateBrand(model.BrandID, model.Name, model.AnotherName, model.CountryCode, model.CityCode, model.IcoPath, model.Status.Value, model.Remark, model.BrandStyle, OperateIP, CurrentUser.UserID);
                if (bl)
                {
                    brandID = model.BrandID;
                }
            }
            JsonDictionary.Add("ID", brandID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetBrandList(string keyWords, int pageSize, int pageIndex, int totalCount)
        {
            int pageCount = 0;
            List<Brand> list = new ProductsBusiness().GetBrandList(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateBrandStatus(string brandID, int status)
        {
            bool bl = new ProductsBusiness().UpdateBrandStatus(brandID, (EnumStatus)status, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteBrand(string brandid)
        {
            int result = 0;
            bool bl = ProductsBusiness.BaseBusiness.DeleteBrand(brandid, OperateIP, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetBrandDetail(string id)
        {
            Brand model = new ProductsBusiness().GetBrandByBrandID(id);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 单位

        public JsonResult SaveUnit(string unit)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ProductUnit model = serializer.Deserialize<ProductUnit>(unit);

            string UnitID = "";
            if (string.IsNullOrEmpty(model.UnitID))
            {
                UnitID = new ProductsBusiness().AddUnit(model.UnitName, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = ProductsBusiness.BaseBusiness.UpdateUnit(model.UnitID, model.UnitName, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
                if (bl)
                {
                    UnitID = model.UnitID;
                }
            }
            JsonDictionary.Add("ID", UnitID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteUnit(string unitid)
        {
            int result = 0;
            bool bl = ProductsBusiness.BaseBusiness.DeleteUnit(unitid, OperateIP, CurrentUser.UserID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 属性

        public JsonResult GetAttrList(int index, string keyWorks)
        {
            List<ProductAttr> list = new List<ProductAttr>();

            int totalCount = 0, pageCount = 0;
            list = new ProductsBusiness().GetAttrList("", keyWorks, PageSize, index, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);

            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetAttrs()
        {
            List<ProductAttr> list = new List<ProductAttr>();
            list = new ProductsBusiness().GetAttrs(CurrentUser.ClientID);

            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveAttr(string attr)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ProductAttr model = serializer.Deserialize<ProductAttr>(attr);

            string attrID = string.Empty;
            if (string.IsNullOrEmpty(model.AttrID))
            {
                attrID = new ProductsBusiness().AddProductAttr(model.AttrName, model.Description, model.CategoryID, model.Type, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else if (new ProductsBusiness().UpdateProductAttr(model.AttrID, model.AttrName, model.Description, OperateIP, CurrentUser.UserID, CurrentUser.ClientID))
            {
                attrID = model.AttrID.ToString();
            }


            JsonDictionary.Add("ID", attrID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetAttrByID(string attrID = "")
        {
            if (string.IsNullOrEmpty(attrID))
            {
                JsonDictionary.Add("Item", null);
            }
            else
            {
                var model = new ProductsBusiness().GetProductAttrByID(attrID, CurrentUser.ClientID);
                JsonDictionary.Add("Item", model);
            }
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveAttrValue(string value)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            AttrValue model = serializer.Deserialize<AttrValue>(value);

            string valueID = string.Empty;
            if (!string.IsNullOrEmpty(model.AttrID))
            {
                if (string.IsNullOrEmpty(model.ValueID))
                {
                    valueID = new ProductsBusiness().AddAttrValue(model.ValueName, model.AttrID, CurrentUser.UserID, CurrentUser.ClientID);
                }
                else if (new ProductsBusiness().UpdateAttrValue(model.ValueID, model.AttrID, model.ValueName, OperateIP, CurrentUser.UserID, CurrentUser.ClientID))
                {
                    valueID = model.ValueID.ToString();
                }
            }

            JsonDictionary.Add("ID", valueID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteCategoryAttr(string categoryid, string attrid, int type)
        {
            bool bl = new ProductsBusiness().UpdateCategoryAttrStatus(categoryid, attrid, EnumStatus.Delete, type, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AddCategoryAttr(string categoryid, string attrid, int type)
        {
            bool bl = new ProductsBusiness().AddCategoryAttr(categoryid, attrid, type, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        } 

        public JsonResult DeleteProductAttr(string attrid)
        {
            int result = 0;
            bool bl = new ProductsBusiness().DeleteProductAttr(attrid, OperateIP, CurrentUser.UserID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteAttrValue(string valueid, string attrid)
        {
            int result = 0;
            bool bl = new ProductsBusiness().DeleteAttrValue(valueid, attrid, OperateIP, CurrentUser.UserID, CurrentUser.ClientID,out result);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 供应商

        public JsonResult GetProviders(string keyWords, int pageIndex, int totalCount)
        {
            int pageCount = 0;
            var list = ProductsBusiness.BaseBusiness.GetProviders(keyWords, PageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProviderDetail(string id)
        {
            var model = ProductsBusiness.BaseBusiness.GetProviderByID(id);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SavaProviders(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ProvidersEntity model = serializer.Deserialize<ProvidersEntity>(entity);

            string id = "";
            if (string.IsNullOrEmpty(model.ProviderID))
            {
                id = ProductsBusiness.BaseBusiness.AddProviders(model.Name, model.Contact, model.MobileTele, "", model.CityCode, model.Address, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = ProductsBusiness.BaseBusiness.UpdateProvider(model.ProviderID, model.Name, model.Contact, model.MobileTele, "", model.CityCode, model.Address, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
                if (bl)
                {
                    id = model.ProviderID;
                }
            }
            JsonDictionary.Add("ID", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteProvider(string id)
        {
            int result = 0;
            bool bl = ProductsBusiness.BaseBusiness.DeleteProvider(id, OperateIP, CurrentUser.UserID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 分类

        public JsonResult SavaCategory(string category, string attrlist, string saleattr)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Category model = serializer.Deserialize<Category>(category);
            //参数
            if (!string.IsNullOrEmpty(attrlist))
            {
                attrlist = attrlist.Substring(0, attrlist.Length - 1);
            }
            //规格
            if (!string.IsNullOrEmpty(saleattr))
            {
                saleattr = saleattr.Substring(0, saleattr.Length - 1);
            }
            Category result;
            if (string.IsNullOrEmpty(model.CategoryID))
            {
                result = new ProductsBusiness().AddCategory(model.CategoryCode, model.CategoryName, model.PID, model.Status.Value, attrlist, saleattr, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else
            {
                result = new ProductsBusiness().UpdateCategory(model.CategoryID, model.CategoryName, model.Status.Value, attrlist, saleattr, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }

            JsonDictionary.Add("status", result != null);
            JsonDictionary.Add("model", result);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetChildCategorysByID(string categoryid)
        {
            var list = ProductsBusiness.BaseBusiness.GetChildCategorysByID(categoryid, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCategoryByID(string categoryid)
        {
            var model = new ProductsBusiness().GetCategoryByID(categoryid, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCategoryDetailsByID(string categoryid)
        {
            var model = new ProductsBusiness().GetCategoryDetailByID(categoryid);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteCategory(string id)
        {
            int result = 0;
            bool bl = new ProductsBusiness().DeleteCategory(id,CurrentUser.UserID,OperateIP,CurrentUser.AgentID,CurrentUser.ClientID,out result);
            JsonDictionary.Add("status", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 产品

        [ValidateInput(false)]
        public JsonResult SavaProduct(string product)
        {
            int result = 0;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Products model = serializer.Deserialize<Products>(product);

            if (!string.IsNullOrEmpty(model.AttrList))
            {
                model.AttrList = model.AttrList.Substring(0, model.AttrList.Length - 1);
            }
            if (!string.IsNullOrEmpty(model.ValueList))
            {
                model.ValueList = model.ValueList.Substring(0, model.ValueList.Length - 1);
            }
            if (!string.IsNullOrEmpty(model.AttrValueList))
            {
                model.AttrValueList = model.AttrValueList.Substring(0, model.AttrValueList.Length - 1);
            }

            string id = "";
            if (string.IsNullOrEmpty(model.ProductID))
            {
                id = new ProductsBusiness().AddProduct(model.ProductCode, model.ProductName, model.GeneralName, model.IsCombineProduct.Value == 1, model.BrandID, model.BigUnitID, model.UnitID,
                                                        model.BigSmallMultiple.Value, model.CategoryID, model.Status.Value, model.AttrList, model.ValueList, model.AttrValueList,
                                                        model.CommonPrice.Value, model.Price, model.Weight.Value, model.IsNew.Value == 1, model.IsRecommend.Value == 1, model.IsAllow, model.IsAutoSend, model.EffectiveDays.Value,
                                                        model.DiscountValue.Value, model.WarnCount, model.ProductImage, model.ShapeCode, model.Description, model.ProductDetails, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateProduct(model.ProductID,model.ProductCode, model.ProductName, model.GeneralName, model.IsCombineProduct.Value == 1, model.BrandID, model.BigUnitID, model.UnitID,
                                                        model.BigSmallMultiple.Value, model.Status.Value, model.CategoryID, model.AttrList, model.ValueList, model.AttrValueList,
                                                        model.CommonPrice.Value, model.Price, model.Weight.Value, model.IsNew.Value == 1, model.IsRecommend.Value == 1, model.IsAllow, model.IsAutoSend, model.EffectiveDays.Value,
                                                        model.DiscountValue.Value, model.WarnCount, model.ProductImage, model.ShapeCode, model.Description, CurrentUser.UserID, CurrentUser.ClientID, out result);
                if (bl)
                {
                    id = model.ProductID;
                }
            }
            JsonDictionary.Add("ID", id);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductList(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterProduct model = serializer.Deserialize<FilterProduct>(filter);
            int totalCount = 0;
            int pageCount = 0;

            List<Products> list = new ProductsBusiness().GetProductList(model.CategoryID, model.BeginPrice, model.EndPrice, model.Keywords, model.OrderBy, model.IsAsc, PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductByID(string productid) 
        {
            var model = new ProductsBusiness().GetProductByID(productid);
            JsonDictionary.Add("Item", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductByIDForDetails(string productid)
        {
            var model = new ProductsBusiness().GetProductByIDForDetails(productid);
            JsonDictionary.Add("Item", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateProductStatus(string productid, int status)
        {
            bool bl = new ProductsBusiness().UpdateProductStatus(productid, (EnumStatus)status, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateProductIsNew(string productid, bool isnew)
        {
            bool bl = new ProductsBusiness().UpdateProductIsNew(productid, isnew, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateProductIsRecommend(string productid, bool isRecommend)
        {
            bool bl = new ProductsBusiness().UpdateProductIsRecommend(productid, isRecommend, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteProduct(string productid)
        {
            int result = 0;
            bool bl = new ProductsBusiness().DeleteProduct(productid, OperateIP, CurrentUser.UserID, out result);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult IsExistsProductCode(string code, string productid)
        {
            bool bl = new ProductsBusiness().IsExistProductCode(code,productid, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult IsExistsShapeCode(string code, string productid)
        {
            bool bl = new ProductsBusiness().IsExistShapeCode(code, productid, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SavaProductDetail(string product)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ProductDetail model = serializer.Deserialize<ProductDetail>(product);

            if (!string.IsNullOrEmpty(model.SaleAttr))
            {
                model.SaleAttr = model.SaleAttr.Substring(0, model.SaleAttr.Length - 1);
            }
            if (!string.IsNullOrEmpty(model.AttrValue))
            {
                model.AttrValue = model.AttrValue.Substring(0, model.AttrValue.Length - 1);
            }
            if (!string.IsNullOrEmpty(model.SaleAttrValue))
            {
                model.SaleAttrValue = model.SaleAttrValue.Substring(0, model.SaleAttrValue.Length - 1);
            }


            int result = 0;
            string id = "";
            if (string.IsNullOrEmpty(model.ProductDetailID))
            {
                id = new ProductsBusiness().AddProductDetails(model.ProductID, model.DetailsCode, model.ShapeCode, model.SaleAttr, model.AttrValue, model.SaleAttrValue,
                                                              model.Price, model.Weight, model.BigPrice, model.ImgS, model.Remark, model.Description, CurrentUser.UserID, CurrentUser.ClientID, out result);
            }
            else
            {
                bool bl = new ProductsBusiness().UpdateProductDetails(model.ProductDetailID, model.ProductID, model.DetailsCode, model.ShapeCode, model.BigPrice, model.SaleAttr, model.AttrValue, model.SaleAttrValue,
                                                              model.Price, model.Weight, model.Remark, model.Description, model.ImgS, CurrentUser.UserID, CurrentUser.ClientID, out result); 
                if (bl)
                {
                    id = model.ProductDetailID;
                }
            }
            JsonDictionary.Add("ID", id);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateProductDetailsStatus(string productdetailid, int status)
        {
            bool bl = new ProductsBusiness().UpdateProductDetailsStatus(productdetailid, (EnumStatus)status, OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductDetails(string wareid, string keywords)
        {
            var list = new ProductsBusiness().GetProductDetails(wareid, keywords, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteProductDetail(string productDetailID)
        {
            int result = 0;
            bool bl = new ProductsBusiness().DeleteProductDetail(productDetailID, OperateIP, CurrentUser.UserID, out result);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 产品导入 导出

        public ActionResult ExportFromProduct(bool test = false, string model = "", string filleName = "产品", string filter = "")
        {
            JavaScriptSerializer serializer=new JavaScriptSerializer();
            Dictionary<string, ExcelFormatter> dic = new Dictionary<string, ExcelFormatter>();
            FilterProduct qicProduct =  serializer.Deserialize<FilterProduct>(filter);
            Dictionary<string, ExcelModel> listColumn = new Dictionary<string, ExcelModel>();
            if (string.IsNullOrEmpty(filter))
            {
                listColumn = GetColumnForJson("product", ref dic, model, test ? "testexport" : "export", CurrentUser.ClientID);
            }
            else
            {  
                qicProduct = serializer.Deserialize<FilterProduct>(filter);
                listColumn = GetColumnForJson("product", ref dic, !string.IsNullOrEmpty(model) ? model : "Item", test ? "testexport" : "export", CurrentUser.ClientID);
            }
            string ipPath = "";
            var excelWriter = new ExcelWriter();
            foreach (var key in listColumn)
            {
                excelWriter.Map(key.Key, key.Value.Title);
            }
            byte[] buffer;
            DataTable dt = new DataTable();
            //模版导出
            if (test)
            {
                DataRow dr = dt.NewRow();
                foreach (var key in listColumn)
                {
                    DataColumn dc1 = new DataColumn(key.Key, Type.GetType("System.String"));
                    dt.Columns.Add(dc1); 
                    dr[key.Key] = key.Value.DefaultText; 
                }
                dt.Rows.Add(dr);
            }
            else
            {
                int totalCount = 0;
                int pageCount = 0; 
                ipPath= Server.MapPath("~");
                dt = new ProductsBusiness().GetProductListDataTable(qicProduct.CategoryID, qicProduct.BeginPrice, qicProduct.EndPrice, qicProduct.Keywords, qicProduct.OrderBy, qicProduct.IsAsc, PageSize, qicProduct.PageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);

            }
            buffer = excelWriter.Write(dt, dic, ipPath);
            var fileName = filleName + (test ? "导入模版" : "")+DateTime.Now.ToString("yyyyMMdd");
            if (!Request.ServerVariables["http_user_agent"].ToLower().Contains("firefox"))
                fileName = HttpUtility.UrlEncode(fileName);
            this.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");
            return File(buffer, "application/ms-excel");

        }

        [HttpPost]
        public ActionResult ProductImport(HttpPostedFileBase file)
        {
            string mes = "";
            if (file == null) { return Content("请选择要导入的文件"); }
            if (file.ContentLength > 2097152)
            {
                return Content("导入文件超过规定（2M )大小,请修改后再操作.");
            }
            if (!file.FileName.Contains(".xls") && !file.FileName.Contains(".xlsx"))
            {
                return Content("文件格式类型不正确");
            }
            try
            {
                DataTable dt = ImportExcelToDataTable(file);
                if (dt.Columns.Count > 0)
                {
                    ///1.获取系统模版列 
                    Dictionary<string, ExcelFormatter> dic=new Dictionary<string, ExcelFormatter>();
                    Dictionary<string, ExcelModel> listColumn = GetColumnForJson("product", ref dic, "",  "import", CurrentUser.ClientID);
                    ;//GetColumnForJson("product", "Item");
                    ///2.上传Excel 与模板中列是否一致 不一致返回提示
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (default(KeyValuePair<string, ExcelModel>).Equals(listColumn.FirstOrDefault(x => x.Value.Title == dc.ColumnName)))
                        {
                            mes += dc.ColumnName + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(mes))
                    {
                        return Content("Excel模版与系统模版不一致,请重新下载模板,编辑后再上传.错误:缺少列 " + mes);
                    }
                    ///3.开始处理
                    int k = 1;
                    var excelWriter = new ExcelWriter();
                    List<Products> list = new List<Products>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            Products product=new Products();
                            excelWriter.GetProductByDataRow(dr, listColumn, product,dic,CurrentUser.ClientID);
                            product.CreateUserID = CurrentUser.UserID;
                            product.ClientID = CurrentUser.ClientID;
                            product.BigUnitID = "";
                            list.Add(product);
                        }
                        catch (Exception ex)
                        {
                            mes += k +" 原因:"+ex.Message+ ",";
                        }
                    }
                    try
                    {
                        if (list.Count > 0)
                        {
                            mes= ExcelImportBusiness.InsertProduct(list);
                        }
                        if (!string.IsNullOrEmpty(mes))
                        {
                            return Content("部分数据未导入成功,原因如下 ：" + mes);
                        }
                        else
                        {
                            return Content("操作成功");
                        }
                    }
                    catch (Exception ex)
                    {
                        return Content("系统异常:请联系管理员,错误原因" + ex.Message);
                    }

                }
                if (!string.IsNullOrEmpty(mes))
                {
                    return Content("部分数据未导入成功,Excel行位置" + mes);
                }
            }
            catch (Exception ex)
            {
                return Content("系统异常:请联系管理员,错误原因:" + ex.Message.ToString());
            }
            return Content(mes);
        } 

        #endregion

        #endregion
    }
}
