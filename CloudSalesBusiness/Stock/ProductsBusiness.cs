using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;


using CloudSalesDAL;
using CloudSalesEntity;
using CloudSalesEnum;

namespace CloudSalesBusiness
{
    public class ProductsBusiness
    {
        public static ProductsBusiness BaseBusiness = new ProductsBusiness();
        /// <summary>
        /// 文件默认存储路径
        /// </summary>
        public string FILEPATH = CloudSalesTool.AppSettings.Settings["UploadFilePath"] + "Product/" + DateTime.Now.ToString("yyyyMM") + "/";
        public string TempPath = CloudSalesTool.AppSettings.Settings["UploadTempPath"];

        public static object SingleLock = new object();

        #region Cache

        private static Dictionary<string, List<ProductAttr>> _attrs;

        private static Dictionary<string, List<ProductAttr>> ClientAttrs
        {
            get 
            {
                if (_attrs == null)
                {
                    _attrs = new Dictionary<string, List<ProductAttr>>();
                }
                return _attrs;
            }
            set { _attrs = value; }
        }

        private static Dictionary<string, List<ProductUnit>> _units;

        private static Dictionary<string, List<ProductUnit>> CacheUnits
        {
            get
            {
                if (_units == null)
                {
                    _units = new Dictionary<string, List<ProductUnit>>();
                }
                return _units;
            }
            set { _units = value; }
        }

        private static Dictionary<string, List<Category>> _categorys;

        private static Dictionary<string, List<Category>> CacheCategorys
        {
            get
            {
                if (_categorys == null)
                {
                    _categorys = new Dictionary<string, List<Category>>();
                }
                return _categorys;
            }
            set { _categorys = value; }
        }

        #endregion

        #region 品牌

        public List<Brand> GetBrandList(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetBrandList(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<Brand> list = new List<Brand>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Brand model = new Brand();
                model.FillData(dr);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                list.Add(model);
            }
            return list;
        }

        public List<Brand> GetBrandList(string clientID)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetBrandList(clientID);

            List<Brand> list = new List<Brand>();
            foreach (DataRow dr in dt.Rows)
            {
                Brand model = new Brand();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public Brand GetBrandByBrandID(string brandID)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetBrandByBrandID(brandID);

            Brand model = new Brand();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
            }
            return model;
        }

        public string AddBrand(string name, string anotherName, string icoPath, string countryCode, string cityCode, int status, string remark, string brandStyle, string operateIP, string operateID, string clientID)
        {
            if (!string.IsNullOrEmpty(icoPath))
            {
                if (icoPath.IndexOf("?") > 0)
                {
                    icoPath = icoPath.Substring(0, icoPath.IndexOf("?"));
                }

                DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath(FILEPATH));
                if (!directory.Exists)
                {
                    directory.Create();
                }

                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(icoPath));
                icoPath = FILEPATH + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(icoPath));
                }
            }

            return new ProductsDAL().AddBrand(name, anotherName, icoPath, countryCode, cityCode, status, remark, brandStyle, operateIP, operateID, clientID);
        }

        public bool UpdateBrandStatus(string brandID, EnumStatus status, string operateIP, string operateID)
        {
            bool bl = CommonBusiness.Update("Brand", "Status", ((int)status).ToString(), " BrandID='" + brandID + "'");

            return bl;
        }

        public bool UpdateBrand(string brandID, string name, string anotherName, string countryCode, string cityCode, string icopath, int status, string remark, string brandStyle, string operateIP, string operateID)
        {
            if (!string.IsNullOrEmpty(icopath) && icopath.IndexOf(TempPath) >= 0)
            {
                if (icopath.IndexOf("?") > 0)
                {
                    icopath = icopath.Substring(0, icopath.IndexOf("?"));
                }

                DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath(FILEPATH));
                if (!directory.Exists)
                {
                    directory.Create();
                }

                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(icopath));
                icopath = FILEPATH + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(icopath));
                }
            }
            var dal = new ProductsDAL();
            return dal.UpdateBrand(brandID, name, anotherName, countryCode, cityCode, status, icopath, remark, brandStyle, operateIP, operateID);
        }

        public bool DeleteBrand(string brandid, string operateip, string operateid, string agentid, string clientid, out int result)
        {
            var bl = ProductsDAL.BaseProvider.DeleteBrand(brandid, operateid, out result);
            return bl;
        }

        #endregion

        #region 单位

        public List<ProductUnit> GetClientUnits(string clientid)
        {
            var dal = new ProductsDAL();

            if (CacheUnits.ContainsKey(clientid))
            {
                return CacheUnits[clientid];
            }

            DataTable dt = dal.GetClientUnits(clientid);

            List<ProductUnit> list = new List<ProductUnit>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductUnit model = new ProductUnit();
                model.FillData(dr);
                list.Add(model);
            }
            CacheUnits.Add(clientid, list);
            return list;
        }

        public ProductUnit GetUnitByID(string unitid, string clientid)
        {
            var list = GetClientUnits(clientid);
            if (list.Where(m => m.UnitID.ToLower() == unitid.ToLower()).Count() > 0)
            {
                return list.Where(m => m.UnitID.ToLower() == unitid.ToLower()).FirstOrDefault();
            }
            ProductUnit model = new ProductUnit();
            DataTable dt = ProductsDAL.BaseProvider.GetUnitByUnitID(unitid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                list.Add(model);
            }
            return model;
        }

        public string AddUnit(string unitName, string description, string operateid, string clientid)
        {
            var dal = new ProductsDAL();
            string id = dal.AddUnit(unitName, description, operateid, clientid);
            if (!string.IsNullOrEmpty(id))
            {
                var list = GetClientUnits(clientid);
                list.Add(new ProductUnit()
                {
                    UnitID = id,
                    UnitName = unitName,
                    Description = description,
                    ClientID = clientid
                });
            }
            return id;
        }

        public bool UpdateUnit(string unitid, string unitName, string desciption, string operateID,string clientid)
        {
            var dal = new ProductsDAL();
            bool bl = dal.UpdateUnit(unitid, unitName, desciption);
            if (bl)
            {
                var model = GetUnitByID(unitid, clientid);
                model.UnitName = unitName;
            }
            return bl;
        }

        public bool DeleteUnit(string unitid, string operateIP, string operateID, string clientid,out int result)
        {
            bool bl = ProductsDAL.BaseProvider.DeleteUnit(unitid, operateID, clientid, out result);
            if (bl)
            {
                var list = GetClientUnits(clientid);
                var model = GetUnitByID(unitid, clientid);
                list.Remove(model);
            }
            return bl;
        }

        #endregion

        #region 属性

        public List<ProductAttr> GetAttrs(string clientid)
        {
            if (ClientAttrs.ContainsKey(clientid))
            {
                return ClientAttrs[clientid];
            }

            List<ProductAttr> list = new List<ProductAttr>();
            DataSet ds = new ProductsDAL().GetAttrs(clientid);
            foreach (DataRow dr in ds.Tables["Attrs"].Rows)
            {
                ProductAttr model = new ProductAttr();
                model.FillData(dr);
                model.AttrValues = new List<AttrValue>();
                foreach (DataRow item in ds.Tables["Values"].Select(" AttrID='" + model.AttrID + "' "))
                {
                    AttrValue attrValue = new AttrValue();
                    attrValue.FillData(item);
                    model.AttrValues.Add(attrValue);
                }
                list.Add(model);
            }
            ClientAttrs.Add(clientid, list);

            return list;
        }

        public List<ProductAttr> GetAttrList(string categoryid, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetAttrList(categoryid, keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientid);

            List<ProductAttr> list = new List<ProductAttr>();
            foreach (DataRow dr in ds.Tables["Attrs"].Rows)
            {
                ProductAttr model = new ProductAttr();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                list.Add(model);
            }
            return list;
        }

        public ProductAttr GetProductAttrByID(string attrid, string clientid)
        {
            var list = GetAttrs(clientid);
            if (list.Where(m => m.AttrID.ToLower() == attrid.ToLower()).Count() > 0)
            {
                return list.Where(m => m.AttrID.ToLower() == attrid.ToLower()).FirstOrDefault();
            }
            var dal = new ProductsDAL();
            DataSet ds = dal.GetProductAttrByID(attrid);

            ProductAttr model = new ProductAttr();
            if (ds.Tables.Contains("Attrs") && ds.Tables["Attrs"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Attrs"].Rows[0]);
                model.AttrValues = new List<AttrValue>();
                foreach (DataRow item in ds.Tables["Values"].Rows)
                {
                    AttrValue attrValue = new AttrValue();
                    attrValue.FillData(item);
                    model.AttrValues.Add(attrValue);
                }

                list.Add(model);
            }

            return model;
        }

        public string AddProductAttr(string attrName, string description, string categoryID, int type, string operateid, string clientid)
        {
            var attrid = Guid.NewGuid().ToString().ToLower();
            var dal = new ProductsDAL();
            if (dal.AddProductAttr(attrid, attrName, description, categoryID, type, operateid, clientid))
            {
                var list = GetAttrs(clientid);
                list.Add(new ProductAttr()
                {
                    AttrID = attrid,
                    AttrName = attrName,
                    Description = description,
                    CategoryID = categoryID,
                    ClientID = clientid,
                    CreateTime = DateTime.Now,
                    CreateUserID = operateid,
                    Status = 1,
                    AttrValues = new List<AttrValue>()
                });
                return attrid;
            }
            return string.Empty;
        }

        public string AddAttrValue(string valueName, string attrID, string operateid, string clientid)
        {
            var valueid = Guid.NewGuid().ToString().ToLower();
            var dal = new ProductsDAL();
            if (dal.AddAttrValue(valueid, valueName, attrID, operateid, clientid))
            {
                var model = GetProductAttrByID(attrID, clientid);
                model.AttrValues.Add(new AttrValue()
                {
                    ValueID = valueid,
                    ValueName = valueName,
                    Status = 1,
                    AttrID = attrID,
                    ClientID = clientid,
                    CreateTime = DateTime.Now
                });

                return valueid;
            }
            return string.Empty;
        }

        public bool UpdateProductAttr(string attrid, string attrName, string description, string operateIP, string operateID, string clientid)
        {
            var dal = new ProductsDAL();
            var bl = dal.UpdateProductAttr(attrid, attrName, description);
            if (bl)
            {
                var model = GetProductAttrByID(attrid, clientid);
                model.AttrName = attrName;
                model.Description = description;
            }
            return bl;
        }

        public bool UpdateAttrValue(string valueid, string attrid, string valueName, string operateIP, string operateID, string clientid)
        {
            var dal = new ProductsDAL();
            var bl = dal.UpdateAttrValue(valueid, valueName);
            if (bl)
            {
                var model = GetProductAttrByID(attrid, clientid);
                var value = model.AttrValues.Where(m => m.ValueID == valueid).FirstOrDefault();
                value.ValueName = valueName;
            }
            return bl;
        }

        public bool DeleteProductAttr(string attrid, string operateIP, string operateid, string clientid, out int result)
        {
            var dal = new ProductsDAL();
            bool bl = dal.DeleteProductAttr(attrid, clientid,out result);
            if (bl)
            {
                var list = GetAttrs(clientid);
                var model = GetProductAttrByID(attrid, clientid);
                list.Remove(model);
            }
            return bl;
        }

        public bool UpdateCategoryAttrStatus(string categoryid, string attrid, EnumStatus status, int type, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            return dal.UpdateCategoryAttrStatus(categoryid, attrid, (int)status, type);
        }

        public bool DeleteAttrValue(string valueid, string attrid, string operateIP, string operateID, string clientid, out int result)
        {
            var dal = new ProductsDAL();
            var bl = dal.DeleteAttrValue(valueid, clientid, out result);
            if (bl)
            {
                var model = GetProductAttrByID(attrid, clientid);
                var value = model.AttrValues.Where(m => m.ValueID == valueid).FirstOrDefault();
                model.AttrValues.Remove(value);
            }
            return bl;
        }

        #endregion

        #region 供应商

        public List<ProvidersEntity> GetProviders(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            var dal = new StockDAL();

            string where = " ClientID='" + clientID + "' and Status<>9";
            if (!string.IsNullOrEmpty(keyWords))
            {
                where += " and (Name like '%" + keyWords + "%' or Contact like '%" + keyWords + "%' or MobileTele like '%" + keyWords + "%') ";
            }

            DataTable dt = CommonBusiness.GetPagerData("Providers", "*", where, "AutoID", pageSize, pageIndex, out totalCount, out pageCount);

            List<ProvidersEntity> list = new List<ProvidersEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                ProvidersEntity model = new ProvidersEntity();
                model.FillData(dr);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                list.Add(model);
            }
            return list;
        }

        public List<ProvidersEntity> GetProviders(string clientid)
        {
            DataTable dt = ProductsDAL.BaseProvider.GetProviders(clientid);

            List<ProvidersEntity> list = new List<ProvidersEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                ProvidersEntity model = new ProvidersEntity();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public ProvidersEntity GetProviderByID(string providerid)
        {
            DataTable dt = ProductsDAL.BaseProvider.GetProviderByID(providerid);

            ProvidersEntity model = new ProvidersEntity();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
            }
            return model;
        }

        public string GetProviderIDByCMID(string clientid, string cmclientid)
        {
            object obj = CommonBusiness.Select("Providers", "  top 1 ProviderID ", "ClientID='" + clientid + "' and CMClientid='" + cmclientid + "' and Status<>9");
            return obj != null ? obj.ToString() : "";
        }
        public string AddProviders(string name, string contact, string mobile, string email, string cityCode, string address, string remark, string cmClientID, string cmClientCode, string operateID, string agentid, string clientID,int type=0)
        {
            return ProductsDAL.BaseProvider.AddProviders(name, contact, mobile, email, cityCode, address, remark, cmClientID, cmClientCode, operateID, agentid, clientID, type);
        }

        public bool UpdateProvider(string providerid, string name, string contact, string mobile, string email, string cityCode, string address, string remark, string operateID, string agentid, string clientID)
        {
            return ProductsDAL.BaseProvider.UpdateProvider(providerid, name, contact, mobile, email, cityCode, address, remark, operateID, agentid, operateID);
        }

        public bool DeleteProvider(string providerid, string ip, string operateid, string clientid, out int result)
        {
            return ProductsDAL.BaseProvider.DeleteProvider(providerid, clientid, out result);
        }

        #endregion

        #region 分类

        public List<Category> GetCategorys(string clientid)
        {
            if (CacheCategorys.ContainsKey(clientid))
            {
                return CacheCategorys[clientid];
            }

            DataTable dt = ProductsDAL.BaseProvider.GetCategorys(clientid);
            List<Category> list = new List<Category>();
            foreach (DataRow dr in dt.Rows)
            {
                Category model = new Category();
                model.FillData(dr);
                list.Add(model);
            }

            foreach (var model in list)
            {
                model.ChildCategorys = list.Where(m => m.PID == model.CategoryID).ToList();
            }

            CacheCategorys.Add(clientid, list);
            return list;
        }

        public DataTable GetCategorysByExcel(string clientid)
        {
            DataTable dt = ProductsDAL.BaseProvider.GetCategorysByExcel(clientid);
            return dt;
        }

        public List<Category> GetChildCategorysByID(string categoryid, string clientid)
        {
            var list = GetCategorys(clientid);
            return list.Where(m => m.PID == categoryid).ToList();
        }

        public Category GetCategoryByID(string categoryid,string clientid)
        {
            var list = GetCategorys(clientid);
            if (list.Where(m => m.CategoryID == categoryid).Count() > 0)
            {
                return list.Where(m => m.CategoryID == categoryid).FirstOrDefault();
            }

            DataTable dt = ProductsDAL.BaseProvider.GetCategoryByID(categoryid);

            Category model = new Category();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                list.Add(model);
            }

            return model;
        }
        public Category GetCategoryByName(string categoryname, string clientid,string pid="")
        {
            var list = GetCategorys(clientid);
            if (list.Where(m => m.CategoryName == categoryname).Count() > 0)
            {
                return list.Where(m => m.CategoryName == categoryname).OrderBy(x=>x.Layers).FirstOrDefault();
            }

            DataTable dt = ProductsDAL.BaseProvider.GetCategoryByName(categoryname, clientid, pid);

            Category model = new Category();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                list.Add(model);
            }

            return model;
        }
        public Category GetCategoryDetailByID(string categoryid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetCategoryDetailByID(categoryid);

            Category model = new Category();
            if (ds.Tables.Contains("Category") && ds.Tables["Category"].Rows.Count > 0)
            {
                model.SaleAttrs = new List<ProductAttr>();
                model.AttrLists = new List<ProductAttr>();
                model.FillData(ds.Tables["Category"].Rows[0]);

                foreach (DataRow attr in ds.Tables["Attrs"].Rows)
                {
                    ProductAttr modelattr = new ProductAttr();
                    modelattr.FillData(attr);
                    if (modelattr.Type == 1)
                    {
                        model.AttrLists.Add(GetProductAttrByID(modelattr.AttrID, model.ClientID));
                    }
                    else
                    {
                        model.SaleAttrs.Add(GetProductAttrByID(modelattr.AttrID, model.ClientID));
                    }
                }
            }
            return model;
        }

        public Category AddCategory(string categoryCode, string categoryName, string pid, int status, string attrlist, string saleattr, string description, string operateid, string clientid, out int result)
        {
            var id = ProductsDAL.BaseProvider.AddCategory(categoryCode, categoryName, pid, status, attrlist, saleattr, description, operateid, clientid, out result);

            if (!string.IsNullOrEmpty(id))
            {
                var model = GetCategoryByID(id, clientid);
                if (!string.IsNullOrEmpty(model.PID))
                {
                    var pModel = GetCategoryByID(model.PID, clientid);
                    if (pModel.ChildCategorys == null)
                    {
                        pModel.ChildCategorys = new List<Category>();
                    }
                    pModel.ChildCategorys.Add(model);
                }
                return model;
            }
            
            return null;
        }

        public bool AddCategoryAttr(string categoryid, string attrid, int type, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            return dal.AddCategoryAttr(categoryid, attrid, type, operateID);
        }

        public Category UpdateCategory(string categoryid, string categoryName, string categoryCode, int status, string attrlist, string saleattr, string description, string operateid, string clientid, out int result)
        {
            bool bl = ProductsDAL.BaseProvider.UpdateCategory(categoryid, categoryName, categoryCode, status, attrlist, saleattr, description, operateid, clientid, out result);
            if (bl)
            {
                var model = GetCategoryByID(categoryid, clientid);
                model.CategoryName = categoryName;
                model.CategoryCode = categoryCode;
                model.Status = status;
                model.SaleAttr = saleattr;
                model.AttrList = attrlist;
                model.Description = description;
                return model;
            }
            return null;
        }

        public bool DeleteCategory(string categoryid, string operateid, string ip, string agentid, string clientid, out int result)
        {
            bool bl = ProductsDAL.BaseProvider.DeleteCategory(categoryid, operateid, out result);
            if (bl)
            {
                var list = GetCategorys(clientid);
                var model = GetCategoryByID(categoryid, clientid);
                if (!string.IsNullOrEmpty(model.PID))
                {
                    var pModel = GetCategoryByID(model.PID, clientid);
                    pModel.ChildCategorys.Remove(model);
                }
                list.Remove(model);
            }
            return bl;
        }

        #endregion

        #region 产品

        public List<Products> GetProductList(string categoryid, string beginprice, string endprice, string keyWords, string orderby, bool isasc, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientid, int status=-1)
        {
            List<Products> list = new List<Products>();

            var dal = new ProductsDAL();
            DataTable dt = dal.GetProductList(categoryid, beginprice, endprice, keyWords, orderby, isasc ? 1 : 0, pageSize, pageIndex, ref totalCount, ref pageCount, clientid, status);

            foreach (DataRow dr in dt.Rows)
            {
                Products model = new Products();
                model.FillData(dr);
                if (!string.IsNullOrEmpty(model.CategoryID))
                {
                    model.CategoryName = GetCategoryByID(model.CategoryID, clientid).CategoryName;
                }
                list.Add(model);
            }
            return list;
        }

        public DataTable GetProductListDataTable(string categoryid, string beginprice, string endprice, string keyWords,
            string orderby, bool isasc, int pageSize, int pageIndex, ref int totalCount, ref int pageCount,
            string clientID)
        {
            DataTable dt=new DataTable();
            var dal = new ProductsDAL();
            DataSet ds = dal.GetProductExport(categoryid, beginprice, endprice, keyWords, orderby, isasc ? 1 : 0, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);
            if (ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        public int GetProductCount(string clientid)
        {
            var result=CommonBusiness.Select("Products", "count(1)", " Status!=9 and ClientID='" + clientid + "'");
            return Convert.ToInt32(result);
        }

        public Products GetProductByID(string productid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetProductByID(productid);

            Products model = new Products();
            if (ds.Tables.Contains("Product") && ds.Tables["Product"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Product"].Rows[0]);
                model.Category = GetCategoryDetailByID(model.CategoryID);

                model.SmallUnit = GetUnitByID(model.UnitID, model.ClientID);

                model.ProductDetails = new List<ProductDetail>();
                foreach (DataRow item in ds.Tables["Details"].Rows)
                {
                    //子产品
                    ProductDetail detail = new ProductDetail();
                    detail.FillData(item);
                    model.ProductDetails.Add(detail);
                }
            }

            return model;
        }
        public string GetProductCode(string code,string shapecode, string clientid)
        {
            object obj ;
            if (!string.IsNullOrEmpty(shapecode))
            {
                obj = CommonBusiness.Select("Products", " top 1 ProductID ",
                    "ClientID='" + clientid + "' and ShapeCode='" + shapecode + "' and ProductCode='" + code +
                    "' and Status<>9");
            }
            else
            {
                obj = CommonBusiness.Select("Products", " top 1 ProductID ", "ClientID='" + clientid + "' and ProductCode='" + code + "' and Status<>9");
            }
            return obj!=null?obj.ToString():"";

        }
        public bool IsExistProductCode(string code, string productid, string clientid)
        {
            if (string.IsNullOrEmpty(productid))
            {
                object obj = CommonBusiness.Select("Products", " Count(0) ", "ClientID='" + clientid + "' and ProductCode='" + code + "' and Status<>9");
                return Convert.ToInt32(obj) > 0;
            }
            else
            {
                object obj = CommonBusiness.Select("Products", " Count(0) ", "ClientID='" + clientid + "' and ProductCode='" + code + "' and Status<>9 and ProductID<>'" + productid + "'");
                return Convert.ToInt32(obj) > 0;
            }
        }
        public string IsExistCMProduct(string cmgoodscode, string cmgoodsid ,string clientid)
        {
            object obj = CommonBusiness.Select("Products", "  top 1 ProductID ", "CMGoodsID='" + cmgoodsid + "' and CMGoodscode='" + cmgoodscode + "' and Status<>9 and ClientID='" + clientid + "'");
            return obj != null ? obj.ToString() : "";
        }
        public string IsExistCMProductDetail(string remark, string cmgoodscode,string cmgoodsid,string clientid)
        {
            object obj = CommonBusiness.Select("Products a join ProductDetail b on a.ProductID=b.ProductID ", " top 1 b.ProductDetailID ", "a.CMGoodsID='" + cmgoodsid + "' and b.remark='" + remark + "' and a.CMGoodscode='" + cmgoodscode + "' and a.Status<>9  and b.Status<>9 and a.CLientID='" + clientid + "'");
            return obj != null ? obj.ToString() : "";
        }
        public bool IsExistShapeCode(string code, string productid, string clientid)
        {
            if (string.IsNullOrEmpty(productid))
            {
                object obj = CommonBusiness.Select("Products", " Count(0) ", "ClientID='" + clientid + "' and ShapeCode='" + code + "' and Status<>9");
                return Convert.ToInt32(obj) > 0;
            }
            else
            {
                object obj = CommonBusiness.Select("Products", " Count(0) ", "ClientID='" + clientid + "' and ShapeCode='" + code + "' and Status<>9 and ProductID<>'" + productid + "'");
                return Convert.ToInt32(obj) > 0;
            }
        }

        public List<Products> GetFilterProducts(string categoryid, List<FilterAttr> Attrs, int doctype, string beginprice, string endprice, string keyWords, string orderby, bool isasc, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            var dal = new ProductsDAL();
            StringBuilder attrbuild = new StringBuilder();
            StringBuilder salebuild = new StringBuilder();
            foreach (var attr in Attrs)
            {
                if (attr.Type == EnumAttrType.Parameter)
                {
                    attrbuild.Append(" and p.ValueList like '%" + attr.ValueID + "%'");
                }
                else if (attr.Type == EnumAttrType.Specification)
                {
                    salebuild.Append(" and AttrValue like '%" + attr.ValueID + "%'");
                }
            }

            DataSet ds = dal.GetFilterProducts(categoryid, attrbuild.ToString(), salebuild.ToString(), doctype, beginprice, endprice, keyWords, orderby, isasc ? 1 : 0, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<Products> list = new List<Products>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Products model = new Products();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<ProductDetail> GetProductDetails(string wareid, string keywords, string agentid, string clientid)
        {
            DataSet ds = ProductsDAL.BaseProvider.GetProductDetails(wareid, keywords, clientid);

            List<ProductDetail> list = new List<ProductDetail>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductDetail model = new ProductDetail();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public Products GetProductByIDForDetails(string productid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetProductByIDForDetails(productid);

            Products model = new Products();
            if (ds.Tables.Contains("Product") && ds.Tables["Product"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Product"].Rows[0]);

                model.SmallUnit = GetUnitByID(model.UnitID, model.ClientID);

                model.AttrLists = new List<ProductAttr>();
                model.SaleAttrs = new List<ProductAttr>();

                foreach (DataRow attrtr in ds.Tables["Attrs"].Rows)
                {
                    ProductAttr attrModel = new ProductAttr();
                    attrModel.FillData(attrtr);
                    attrModel.AttrValues = new List<AttrValue>();

                    //参数
                    if (attrModel.Type == (int)EnumAttrType.Parameter)
                    {
                        foreach (var value in GetProductAttrByID(attrModel.AttrID, model.ClientID).AttrValues)
                        {
                            if (model.AttrValueList.IndexOf(value.ValueID) >= 0)
                            {
                                attrModel.AttrValues.Add(value);
                                model.AttrLists.Add(attrModel);
                                break;
                            }
                        }
                       
                    }
                }

                model.ProductDetails = new List<ProductDetail>();
                foreach (DataRow item in ds.Tables["Details"].Rows)
                {
                    ProductDetail detail = new ProductDetail();
                    detail.FillData(item);

                    model.ProductDetails.Add(detail);
                }
            }

            return model;
        }

        public List<ProductDetail> GetProductDetailsByDids(string did)
        {
            string sqlWhere = " ProductDetailID in (" + did + ") and status<>9";
            int totalCount = 0, pageCount = 0;
            DataTable dt = CommonBusiness.GetPagerData("ProductDetail", "*", sqlWhere, "CustomerID", int.MaxValue, 1, out totalCount, out pageCount);

            List<ProductDetail> list = new List<ProductDetail>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductDetail model = new ProductDetail();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public string AddProduct(string productCode, string productName, string generalName, bool iscombineproduct, string providerid, string brandid, string bigunitid, string UnitID, int bigSmallMultiple,
                                 string categoryid, int status, string attrlist, string valuelist, string attrvaluelist, decimal commonprice, decimal price, decimal weight, bool isnew,
                                 bool isRecommend, int isallow, int isautosend, int effectiveDays, decimal discountValue, int warnCount, string productImg, string shapeCode, string description,
                                 List<ProductDetail> details,string cmgoodsid,string cmgoodscode, string operateid, string agentid, string clientid, out int result)
        {
            lock (SingleLock)
            {
                //if (!string.IsNullOrEmpty(productImg))
                //{
                //    if (productImg.IndexOf("?") > 0)
                //    {
                //        productImg = productImg.Substring(0, productImg.IndexOf("?"));
                //    }

                //    DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath(FILEPATH));
                //    if (!directory.Exists)
                //    {
                //        directory.Create();
                //    }

                //    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(productImg));
                //    productImg = FILEPATH + file.Name;

                //    if (file.Exists)
                //    {
                //        file.MoveTo(HttpContext.Current.Server.MapPath(productImg));
                //    }
                //}

                var dal = new ProductsDAL();
                string pid = dal.AddProduct(productCode, productName, generalName, iscombineproduct, providerid, brandid, bigunitid, UnitID, bigSmallMultiple, categoryid, status, attrlist,
                                        valuelist, attrvaluelist, commonprice, price, weight, isnew, isRecommend, isallow, isautosend, effectiveDays, discountValue, warnCount,
                                        productImg, shapeCode, cmgoodsid, cmgoodscode,description, operateid, clientid, out result);
                //产品添加成功添加子产品
                if (!string.IsNullOrEmpty(pid))
                {
                    //日志
                    LogBusiness.AddActionLog(CloudSalesEnum.EnumSystemType.Client, CloudSalesEnum.EnumLogObjectType.Product, EnumLogType.Create, "", operateid, agentid, clientid);
                    int r = 0;
                    foreach (var model in details)
                    {
                        AddProductDetails(pid, model.DetailsCode, model.ShapeCode, model.SaleAttr, model.AttrValue, model.SaleAttrValue, model.Price, model.Weight, model.Price, model.ImgS, model.Remark, model.Description, operateid, clientid, out r);
                    }
                }
                return pid;
            }
        }

        public string AddProductDetails(string productid, string productCode, string shapeCode, string attrlist, string valuelist, string attrvaluelist, decimal price, decimal weight, decimal bigprice, string productImg, string remark, string description, string operateid, string clientid, out int result)
        {
            //if (!string.IsNullOrEmpty(productImg))
            //{
            //    if (productImg.IndexOf("?") > 0)
            //    {
            //        productImg = productImg.Substring(0, productImg.IndexOf("?"));
            //    }

            //    DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath(FILEPATH));
            //    if (!directory.Exists)
            //    {
            //        directory.Create();
            //    }

            //    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(productImg));
            //    productImg = FILEPATH + file.Name;
            //    if (file.Exists)
            //    {
            //        file.MoveTo(HttpContext.Current.Server.MapPath(productImg));
            //    }
            //}

            var dal = new ProductsDAL();
            return dal.AddProductDetails(productid, productCode, shapeCode, attrlist, valuelist, attrvaluelist, price, weight, bigprice, productImg, remark, description, operateid, clientid, out result);
        }

        public bool UpdateProductStatus(string productid, EnumStatus status, string operateIP, string operateID)
        {
            return CommonBusiness.Update("Products", "Status", ((int)status).ToString(), " ProductID='" + productid + "' and Status<>9 ");
        }

        public bool DeleteProduct(string productid, string operateip, string operateid, out int result)
        {
            return ProductsDAL.BaseProvider.DeleteProduct(productid, operateid, out result);
        }

        public bool UpdateProductIsNew(string productid, bool isNew, string operateIP, string operateID)
        {
            return CommonBusiness.Update("Products", "IsNew", isNew ? "1" : "0", " ProductID='" + productid + "'");
        }

        public bool UpdateProductIsRecommend(string productid, bool isRecommend, string operateIP, string operateID)
        {
            return CommonBusiness.Update("Products", "IsRecommend", isRecommend ? "1" : "0", " ProductID='" + productid + "'");
        }

        public bool UpdateProduct(string productid, string productCode, string productName, string generalName, bool iscombineproduct, string providerid, string brandid, string bigunitid, string UnitID, int bigSmallMultiple,
                         int status, string categoryid, string attrlist, string valuelist, string attrvaluelist, decimal commonprice, decimal price, decimal weight, bool isnew,
                         bool isRecommend, int isallow, int isautosend, int effectiveDays, decimal discountValue, int warnCount, string productImg, string shapeCode, string description, string operateid, string clientid, out int result)
        {

            if (!string.IsNullOrEmpty(productImg) && productImg.IndexOf(TempPath) >= 0)
            {
                if (productImg.IndexOf("?") > 0)
                {
                    productImg = productImg.Substring(0, productImg.IndexOf("?"));
                }

                DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath(FILEPATH));
                if (!directory.Exists)
                {
                    directory.Create();
                }

                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(productImg));
                productImg = FILEPATH + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(productImg));
                }
            }

            var dal = new ProductsDAL();
            return dal.UpdateProduct(productid, productCode, productName, generalName, iscombineproduct, providerid, brandid, bigunitid, UnitID, bigSmallMultiple, status, categoryid, attrlist,
                                    valuelist, attrvaluelist, commonprice, price, weight, isnew, isRecommend, isallow, isautosend, effectiveDays, discountValue, warnCount, productImg,
                                    shapeCode, description, operateid, clientid, out result);
        }

        public bool UpdateProductDetailsStatus(string productdetailid, EnumStatus status, string operateIP, string operateID)
        {
            return CommonBusiness.Update("ProductDetail", "Status", (int)status, " ProductDetailID='" + productdetailid + "'");
        }

        public bool UpdateProductDetails(string detailid, string productid, string productCode, string shapeCode, decimal bigPrice, string attrlist, string valuelist, string attrvaluelist, decimal price,
                                         decimal weight, string remark, string description, string productImg, string operateid, string clientid, out int result)
        {
            lock (SingleLock)
            {
                if (!string.IsNullOrEmpty(productImg) && productImg.IndexOf(TempPath) >= 0)
                {
                    if (productImg.IndexOf("?") > 0)
                    {
                        productImg = productImg.Substring(0, productImg.IndexOf("?"));
                    }

                    DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath(FILEPATH));
                    if (!directory.Exists)
                    {
                        directory.Create();
                    }

                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(productImg));
                    productImg = FILEPATH + file.Name;
                    if (file.Exists)
                    {
                        file.MoveTo(HttpContext.Current.Server.MapPath(productImg));
                    }
                }
                var dal = new ProductsDAL();
                return dal.UpdateProductDetails(detailid, productid, productCode, shapeCode, bigPrice, attrlist, valuelist, attrvaluelist, price, weight, remark, description, productImg, out result);
            }
        }

        public bool DeleteProductDetail(string productDetailID, string operateip, string operateid, out int result)
        {
            return ProductsDAL.BaseProvider.DeleteProductDetail(productDetailID, operateid, out result);
        }

        #endregion
    }
}
