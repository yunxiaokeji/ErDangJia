﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CloudSalesDAL
{
    public class ProductsDAL : BaseDAL
    {
        public static ProductsDAL BaseProvider = new ProductsDAL();

        #region 品牌

        public DataSet GetBrandList(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@keyWords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@ClientID",clientID)
                                       
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetBrandList", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;

        }

        public DataTable GetBrandList(string clientID)
        {
            SqlParameter[] paras = { new SqlParameter("@clientID", clientID) };
            DataTable dt = GetDataTable("select BrandID,Name from Brand where ClientID=@clientID and Status<>9", paras, CommandType.Text);
            return dt;

        }

        public DataTable GetBrandByBrandID(string brandID)
        {
            SqlParameter[] paras = { new SqlParameter("@brandID", brandID) };
            DataTable dt = GetDataTable("select * from Brand where BrandID=@brandID", paras, CommandType.Text);
            return dt;
        }

        public string AddBrand(string name, string anotherName, string icoPath, string countryCode, string cityCode, int status, string remark, string brandStyle, string operateIP, string operateID, string clientID)
        {
            string brandID = Guid.NewGuid().ToString();
            string sqlText = "INSERT INTO Brand([BrandID] ,[Name],[AnotherName] ,[IcoPath],[CountryCode],[CityCode],[Status],[Remark],[BrandStyle],[OperateIP],[CreateUserID],ClientID) "
                                      + "values(@BrandID ,@Name,@AnotherName ,@IcoPath,@CountryCode,@CityCode,@Status,@Remark,@BrandStyle,@OperateIP,@CreateUserID,@ClientID)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@BrandID" , brandID),
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@AnotherName" , anotherName),
                                     new SqlParameter("@IcoPath" , icoPath),
                                     new SqlParameter("@CountryCode" , countryCode),
                                     new SqlParameter("@CityCode" , cityCode),
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@BrandStyle" , brandStyle),
                                     new SqlParameter("@OperateIP" , operateIP),
                                     new SqlParameter("@CreateUserID" , operateID),
                                     new SqlParameter("@ClientID" , clientID)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0 ? brandID : "";

        }

        public bool UpdateBrand(string brandid, string name, string anotherName, string countryCode, string cityCode, int status, string icopath, string remark, string brandStyle, string operateIP, string operateID)
        {
            string sqlText = "Update Brand set [Name]=@Name,[AnotherName]=@AnotherName ,[CountryCode]=@CountryCode,[CityCode]=@CityCode," +
                "[Status]=@Status,IcoPath=@IcoPath,[Remark]=@Remark,[BrandStyle]=@BrandStyle,[UpdateTime]=getdate() where BrandID=@BrandID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@AnotherName" , anotherName),
                                     new SqlParameter("@CountryCode" , countryCode),
                                     new SqlParameter("@CityCode" , cityCode),
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@IcoPath" , icopath),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@BrandStyle" , brandStyle),
                                     new SqlParameter("@BrandID" , brandid),
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool DeleteBrand(string brandid, string operateid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@BrandID",brandid),
                                       new SqlParameter("@OperateID",operateid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_DeleteBrand", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;

        }

        #endregion

        #region 单位

        public DataTable GetClientUnits(string clientid)
        {
            SqlParameter[] paras = { new SqlParameter("@ClientID", clientid) };
            DataTable dt = GetDataTable("select * from ProductUnit where ClientID=@ClientID and Status<>9", paras, CommandType.Text);
            return dt;
        }

        public DataTable GetUnitByUnitID(string unitid)
        {
            SqlParameter[] paras = { new SqlParameter("@UnitID", unitid) };
            DataTable dt = GetDataTable("select * from ProductUnit where UnitID=@UnitID", paras, CommandType.Text);
            return dt;
        }

        public string AddUnit(string unitName, string description, string operateid, string clientid)
        {
            string guid = Guid.NewGuid().ToString();
            string sqlText = "INSERT INTO ProductUnit([UnitID] ,[UnitName],[Description],CreateUserID,ClientID) "
                                            + "values(@UnitID ,@UnitName,@Description,@CreateUserID,@ClientID)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@UnitID" , guid),
                                     new SqlParameter("@UnitName" , unitName),
                                     new SqlParameter("@Description" , description),
                                     new SqlParameter("@CreateUserID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            if (ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0)
            {
                return guid;
            }
            return "";
        }

        public bool UpdateUnit(string unitid, string unitName, string description)
        {
            string sqlText = "Update ProductUnit set [UnitName]=@UnitName,[Description]=@Description,UpdateTime=getdate()  where [UnitID]=@UnitID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@UnitID",unitid),
                                     new SqlParameter("@UnitName" , unitName),
                                     new SqlParameter("@Description" , description)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool DeleteUnit(string unitid, string operateid, string clientid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@UnitID",unitid),
                                       new SqlParameter("@OperateID",operateid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_DeleteUnit", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;

        }

        #endregion

        #region 属性

        public DataSet GetAttrs(string clientid)
        {
            SqlParameter[] paras = { new SqlParameter("@ClientID", clientid) };
            DataSet ds = GetDataSet("P_GetAttrsByClientID", paras, CommandType.StoredProcedure, "Attrs|Values");
            return ds;
        }

        public DataSet GetAttrList(string categoryid, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@keyWords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@CategoryID", categoryid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetProductAttrList", paras, CommandType.StoredProcedure, "Attrs");
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;

        }

        public DataSet GetProductAttrByID(string attrID)
        {
            SqlParameter[] paras = { new SqlParameter("@AttrID", attrID) };
            DataSet ds = GetDataSet("P_GetProductAttrByID", paras, CommandType.StoredProcedure, "Attrs|Values");
            return ds;
        }

        public bool AddProductAttr(string attrID, string attrName, string description, string categoryID, int type, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@AttrID" , attrID),
                                     new SqlParameter("@AttrName" , attrName),
                                     new SqlParameter("@Description" , description),
                                     new SqlParameter("@CategoryID" , categoryID),
                                     new SqlParameter("@Type" , type),
                                     new SqlParameter("@CreateUserID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery("P_InsertAttr", paras, CommandType.StoredProcedure) > 0;
        }

        public bool AddAttrValue(string valueID, string valueName, string attrID, string operateid, string clientid)
        {
            string sqlText = "INSERT INTO AttrValue([ValueID] ,[ValueName],[Status],[AttrID],CreateUserID,ClientID) "
                                             + "values(@ValueID ,@ValueName,1,@AttrID,@CreateUserID,@ClientID) ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ValueID" , valueID),
                                     new SqlParameter("@ValueName" , valueName),
                                     new SqlParameter("@AttrID" , attrID),
                                     new SqlParameter("@CreateUserID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool UpdateProductAttr(string attrID, string attrName, string description)
        {
            string sqlText = "Update ProductAttr set [AttrName]=@AttrName,[Description]=@Description  where [AttrID]=@AttrID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@AttrID",attrID),
                                     new SqlParameter("@AttrName" , attrName),
                                     new SqlParameter("@Description" , description),
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool DeleteProductAttr(string attrid, string clientid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",result),
                                     new SqlParameter("@AttrID",attrid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_DeleteProductAttr", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        public bool UpdateAttrValue(string valueid, string ValueName)
        {
            string sqlText = "Update AttrValue set [ValueName]=@ValueName  where [ValueID]=@ValueID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ValueID",valueid),
                                     new SqlParameter("@ValueName" , ValueName),
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool DeleteAttrValue(string valueid, string clientid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",result),
                                     new SqlParameter("@ValueID",valueid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_DeleteAttrValue", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        public bool UpdateCategoryAttrStatus(string categoryid, string attrid, int status, int type)
        {
            string sqlText = "Update CategoryAttr set Status=@Status,UpdateTime=getdate()  where [AttrID]=@AttrID and CategoryID=@CategoryID and Type=@Type";
            SqlParameter[] paras = { 
                                     new SqlParameter("@CategoryID",categoryid),
                                     new SqlParameter("@AttrID",attrid),
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@Type" , type)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        #endregion

        #region 供应商

        public DataTable GetProviders(string clientid)
        {
            SqlParameter[] paras = { new SqlParameter("@ClientID", clientid) };
            DataTable dt = GetDataTable("select ProviderID,Name from Providers where ClientID=@ClientID and Status<>9", paras, CommandType.Text);
            return dt;

        }

        public DataTable GetProviderByID(string providerid)
        {
            SqlParameter[] paras = { new SqlParameter("@ProviderID", providerid) };
            DataTable dt = GetDataTable("select * from Providers where ProviderID=@ProviderID", paras, CommandType.Text);
            return dt;
        }

        public string AddProviders(string name, string contact, string mobile, string email, string cityCode, string address, string remark, string operateID, string agentid, string clientID)
        {
            string id = Guid.NewGuid().ToString();
            string sqlText = "insert into Providers(ProviderID,Name,Contact,MobileTele,Email,Website,CityCode,Address,Remark,CreateTime,CreateUserID,AgentID,ClientID)"
                                      + "values(@ProviderID ,@Name,@Contact ,@MobileTele,@Email,'',@CityCode,@Address,@Remark,getdate(),@CreateUserID,@AgentID,@ClientID)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ProviderID" , id),
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@Contact" , contact),
                                     new SqlParameter("@MobileTele" , mobile),
                                     new SqlParameter("@Email" , email),
                                     new SqlParameter("@CityCode" , cityCode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@CreateUserID" , operateID),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@ClientID" , clientID)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0 ? id : "";

        }
        
        public bool UpdateProvider(string providerid, string name, string contact, string mobile, string email, string cityCode, string address, string remark, string operateID, string agentid, string clientID)
        {
            string sqlText = "Update Providers set [Name]=@Name,[Contact]=@Contact ,[MobileTele]=@MobileTele,[CityCode]=@CityCode," +
                "[Address]=@Address,[Remark]=@Remark where [ProviderID]=@ProviderID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@Contact" , contact),
                                     new SqlParameter("@MobileTele" , mobile),
                                     new SqlParameter("@CityCode" , cityCode),
                                     new SqlParameter("@Address" , address),
                                     new SqlParameter("@Remark" , remark),
                                     new SqlParameter("@ProviderID" , providerid),
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool DeleteProvider(string providerid, string clientid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",result),
                                     new SqlParameter("@ProviderID",providerid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_DeleteProvider", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        #endregion

        #region 分类

        public DataTable GetCategorys(string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ClientID", clientid) 
                                   };
            DataTable dt = GetDataTable("select * from Category where ClientID=@ClientID and Status<>9 Order by CreateTime", paras, CommandType.Text);
            return dt;
        }

        public DataTable GetCategoryByID(string categoryid)
        {
            SqlParameter[] paras = { new SqlParameter("@CategoryID", categoryid) };
            DataTable dt = GetDataTable("select * from Category where CategoryID=@CategoryID", paras, CommandType.Text);
            return dt;
        }

        public DataSet GetCategoryDetailByID(string categoryid)
        {
            SqlParameter[] paras = { new SqlParameter("@CategoryID", categoryid) };
            DataSet ds = GetDataSet("P_GetCategoryDetailByID", paras, CommandType.StoredProcedure, "Category|Attrs|Values");
            return ds;
        }

        public string AddCategory(string categoryCode, string categoryName, string pid, int status, string attrlist, string saleattr, string description, string operateid, string clientid)
        {
            string id = "";
            SqlParameter[] paras = { 
                                       new SqlParameter("@CategoryID",SqlDbType.NVarChar,64),
                                       new SqlParameter("@CategoryCode",categoryCode),
                                       new SqlParameter("@CategoryName",categoryName),
                                       new SqlParameter("@PID",pid),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@AttrList",attrlist),
                                       new SqlParameter("@SaleAttr",saleattr),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = id;
            paras[0].Direction = ParameterDirection.InputOutput;

            ExecuteNonQuery("P_InsertCategory", paras, CommandType.StoredProcedure);
            id = paras[0].Value.ToString();
            return id;
        }

        public bool AddCategoryAttr(string categoryid, string attrid, int type, string operateid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@CategoryID",categoryid),
                                     new SqlParameter("@AttrID",attrid),
                                     new SqlParameter("@Type" , type),
                                     new SqlParameter("@CreateUserID" , operateid)
                                   };
            return ExecuteNonQuery("P_AddCategoryAttr", paras, CommandType.StoredProcedure) > 0;
        }

        public bool UpdateCategory(string categoryid, string categoryName, int status, string attrlist, string saleattr, string description, string operateid)
        {
            string sql = "P_UpdateCategory";
            SqlParameter[] paras = { 
                                       new SqlParameter("@CategoryID",categoryid),
                                       new SqlParameter("@CategoryName",categoryName),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@AttrList",attrlist),
                                       new SqlParameter("@SaleAttr",saleattr),
                                       new SqlParameter("@UserID",operateid),
                                       new SqlParameter("@Description",description)
                                   };

            return ExecuteNonQuery(sql, paras, CommandType.StoredProcedure) > 0;

        }

        public bool DeleteCategory(string categoryid, string operateid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@CategoryID",categoryid),
                                       new SqlParameter("@OperateID",operateid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            bool bl = ExecuteNonQuery("P_DeleteCategory", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            return bl;
        }

        #endregion

        #region 产品

        public DataSet GetProductList(string categoryid, string beginprice, string endprice, string keyWords, string orderby, int isasc, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@orderColumn",orderby),
                                       new SqlParameter("@isAsc",isasc),
                                       new SqlParameter("@BeginPrice",beginprice),
                                       new SqlParameter("@EndPrice",endprice),
                                       new SqlParameter("@CategoryID",categoryid),
                                       new SqlParameter("@keyWords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@ClientID",clientID)
                                       
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetProductList", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;

        }

        public DataSet GetProductByID(string productid)
        {
            SqlParameter[] paras = { new SqlParameter("@ProductID", productid) };
            DataSet ds = GetDataSet("P_GetProductByID", paras, CommandType.StoredProcedure, "Product|Details");
            return ds;
        }

        public DataSet GetFilterProducts(string categoryid, string attrwhere, string salewhere, int doctype, string beginprice, string endprice, string keyWords, string orderby, int isasc, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@orderColumn",orderby),
                                       new SqlParameter("@isAsc",isasc),
                                       new SqlParameter("@AttrWhere",attrwhere),
                                       new SqlParameter("@SaleWhere",salewhere),
                                       new SqlParameter("@DocType",doctype),
                                       new SqlParameter("@BeginPrice",beginprice),
                                       new SqlParameter("@EndPrice",endprice),
                                       new SqlParameter("@CategoryID",categoryid),
                                       new SqlParameter("@keyWords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@ClientID",clientID)
                                       
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetFilterProducts", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;

        }

        public DataSet GetProductByIDForDetails(string productid)
        {
            SqlParameter[] paras = { new SqlParameter("@ProductID", productid) };
            DataSet ds = GetDataSet("P_GetProductByIDForDetails", paras, CommandType.StoredProcedure, "Product|Details|Attrs");
            return ds;
        }

        public DataSet GetProductDetails(string wareid, string keywords, string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@WareID",wareid),
                                       new SqlParameter("@KeyWords",keywords),
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataSet ds = GetDataSet("P_GetProductDetails", paras, CommandType.StoredProcedure, "Products");
            return ds;
        }

        public string AddProduct(string productCode, string productName, string generalName, bool iscombineproduct, string brandid, string bigunitid, string UnitID, int bigSmallMultiple,
                         string categoryid, int status, string attrlist, string valuelist, string attrvaluelist, decimal commonprice, decimal price,
                         decimal weight, bool isnew, bool isRecommend, int isallow, int isautosend, int effectiveDays, decimal discountValue, int warnCount,
                         string productImg, string shapeCode, string description, string operateid, string clientid, out int result)
        {
            string id = "";
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@ProductID",SqlDbType.NVarChar,64),
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ProductCode",productCode),
                                       new SqlParameter("@ProductName",productName),
                                       new SqlParameter("@GeneralName",generalName),
                                       new SqlParameter("@IsCombineProduct",iscombineproduct),
                                       new SqlParameter("@BrandID",brandid),
                                       new SqlParameter("@BigUnitID",bigunitid),
                                       new SqlParameter("@UnitID",UnitID),
                                       new SqlParameter("@BigSmallMultiple",bigSmallMultiple),
                                       new SqlParameter("@CategoryID",categoryid),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@AttrList",attrlist),
                                       new SqlParameter("@ValueList",valuelist),
                                       new SqlParameter("@AttrValueList",attrvaluelist),
                                       new SqlParameter("@CommonPrice",commonprice),
                                       new SqlParameter("@Price",price),
                                       new SqlParameter("@Weight",weight),
                                       new SqlParameter("@Isnew",isnew ? 1 :0),
                                       new SqlParameter("@IsRecommend",isRecommend ? 1 : 0),
                                       new SqlParameter("@IsAllow",isallow),
                                       new SqlParameter("@IsAutoSend",isautosend),
                                       new SqlParameter("@EffectiveDays",effectiveDays),
                                       new SqlParameter("@DiscountValue",discountValue),
                                       new SqlParameter("@WarnCount",warnCount),
                                       new SqlParameter("@ProductImg",productImg),
                                       new SqlParameter("@ShapeCode",shapeCode),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = id;
            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Value = result;
            paras[1].Direction = ParameterDirection.InputOutput;

            ExecuteNonQuery("P_InsertProduct", paras, CommandType.StoredProcedure);
            id = paras[0].Value.ToString();
            result = Convert.ToInt32(paras[1].Value);
            return id;
        }

        public string InsertProductExcel(string productCode, string productName, string generalName, bool iscombineproduct, string brandid, string bigunitid, string UnitID, int bigSmallMultiple,
                        string categoryid, int status, string attrlist, string valuelist, string attrvaluelist, decimal commonprice, decimal price,
                        decimal weight, bool isnew, bool isRecommend, int isallow, int isautosend, int effectiveDays, decimal discountValue, int warnCount, string productImg, string shapeCode, string description, string operateid, string clientid, ref string result)
        {
            string id = ""; 
            SqlParameter[] paras = { 
                                       new SqlParameter("@ProductID",SqlDbType.NVarChar,64),
                                       new SqlParameter("@Result",SqlDbType.NVarChar,64),
                                       new SqlParameter("@ProductCode",productCode),
                                       new SqlParameter("@ProductName",productName),
                                       new SqlParameter("@GeneralName",generalName),
                                       new SqlParameter("@IsCombineProduct",iscombineproduct),
                                       new SqlParameter("@BrandID",brandid),
                                       new SqlParameter("@BigUnitID",bigunitid),
                                       new SqlParameter("@UnitID",UnitID),
                                       new SqlParameter("@BigSmallMultiple",bigSmallMultiple),
                                       new SqlParameter("@CategoryID",categoryid),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@AttrList",attrlist),
                                       new SqlParameter("@ValueList",valuelist),
                                       new SqlParameter("@AttrValueList",attrvaluelist),
                                       new SqlParameter("@CommonPrice",commonprice),
                                       new SqlParameter("@Price",price),
                                       new SqlParameter("@Weight",weight),
                                       new SqlParameter("@Isnew",isnew ? 1 :0),
                                       new SqlParameter("@IsRecommend",isRecommend ? 1 : 0),
                                       new SqlParameter("@IsAllow",isallow),
                                       new SqlParameter("@IsAutoSend",isautosend),
                                       new SqlParameter("@EffectiveDays",effectiveDays),
                                       new SqlParameter("@DiscountValue",discountValue),
                                       new SqlParameter("@WarnCount",warnCount),
                                       new SqlParameter("@ProductImg",productImg),
                                       new SqlParameter("@ShapeCode",shapeCode),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = id;
            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Value = result;
            paras[1].Direction = ParameterDirection.InputOutput;

            ExecuteNonQuery("P_InsertProductExcel", paras, CommandType.StoredProcedure);
            id = paras[0].Value.ToString();
            result = paras[1].Value.ToString();
            return id;
        }

        public string AddProductDetails(string productid, string productCode, string shapeCode, string attrlist, string valuelist, string attrvaluelist, decimal price, decimal weight, decimal bigprice, string productImg, string remark, string description, string operateid, string clientid, out int result)
        {
            string id = "";
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@DetailID",SqlDbType.NVarChar,64),
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@ProductCode",productCode),
                                       new SqlParameter("@BigPrice",bigprice),
                                       new SqlParameter("@ProductID",productid),
                                       new SqlParameter("@AttrList",attrlist),
                                       new SqlParameter("@ValueList",valuelist),
                                       new SqlParameter("@AttrValueList",attrvaluelist),
                                       new SqlParameter("@Price",price),
                                       new SqlParameter("@Weight",weight),
                                       new SqlParameter("@ProductImg",productImg),
                                       new SqlParameter("@ShapeCode",shapeCode),
                                       new SqlParameter("@Remark",remark),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Value = id;
            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Value = result;
            paras[1].Direction = ParameterDirection.InputOutput;

            ExecuteNonQuery("P_InsertProductDetail", paras, CommandType.StoredProcedure);
            id = paras[0].Value.ToString();
            result = Convert.ToInt32(paras[1].Value);
            return id;
        }

        public bool UpdateProduct(string productid, string productCode, string productName, string generalName, bool iscombineproduct, string brandid, string bigunitid, string UnitID, int bigSmallMultiple,
                            int status, string categoryid, string attrlist, string valuelist, string attrvaluelist, decimal commonprice, decimal price,
                            decimal weight, bool isnew, bool isRecommend, int isallow, int isautosend, int effectiveDays, decimal discountValue, int warnCount, 
                            string productImg, string shapeCode, string description, string operateid, string clientid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@ProductID",productid),
                                       new SqlParameter("@ProductCode",productCode),
                                       new SqlParameter("@ProductName",productName),
                                       new SqlParameter("@GeneralName",generalName),
                                       new SqlParameter("@IsCombineProduct",iscombineproduct),
                                       new SqlParameter("@BrandID",brandid),
                                       new SqlParameter("@BigUnitID",bigunitid),
                                       new SqlParameter("@UnitID",UnitID),
                                       new SqlParameter("@BigSmallMultiple",bigSmallMultiple),
                                       new SqlParameter("@Status",status),
                                       new SqlParameter("@CategoryID",categoryid),
                                       new SqlParameter("@AttrList",attrlist),
                                       new SqlParameter("@ValueList",valuelist),
                                       new SqlParameter("@AttrValueList",attrvaluelist),
                                       new SqlParameter("@CommonPrice",commonprice),
                                       new SqlParameter("@Price",price),
                                       new SqlParameter("@Weight",weight),
                                       new SqlParameter("@Isnew",isnew ? 1 :0),
                                       new SqlParameter("@IsRecommend",isRecommend ? 1 : 0),
                                       new SqlParameter("@IsAllow",isallow),
                                       new SqlParameter("@IsAutoSend",isautosend),
                                       new SqlParameter("@EffectiveDays",effectiveDays),
                                       new SqlParameter("@DiscountValue",discountValue),
                                       new SqlParameter("@WarnCount",warnCount),
                                       new SqlParameter("@ProductImg",productImg),
                                       new SqlParameter("@ShapeCode",shapeCode),
                                       new SqlParameter("@Description",description),
                                       new SqlParameter("@CreateUserID",operateid),
                                       new SqlParameter("@ClientID",clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_UpdateProduct", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;

        }

        public bool DeleteProduct(string productid, string operateid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@ProductID",productid),
                                       new SqlParameter("@OperateID",operateid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            bool bl = ExecuteNonQuery("P_DeleteProduct", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            return bl;
        }

        public bool UpdateProductDetails(string detailid, string productid, string productCode, string shapeCode, decimal bigPrice, string attrlist, string valuelist, string attrvaluelist,
                                         decimal price, decimal weight, string remark, string description, string image, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",SqlDbType.Int),
                                       new SqlParameter("@DetailID",detailid),
                                       new SqlParameter("@ProductID",productid),
                                       new SqlParameter("@BigPrice",bigPrice),
                                       new SqlParameter("@ProductCode",productCode),
                                       new SqlParameter("@AttrList",attrlist),
                                       new SqlParameter("@ValueList",valuelist),
                                       new SqlParameter("@AttrValueList",attrvaluelist),
                                       new SqlParameter("@Price",price),
                                       new SqlParameter("@Weight",weight),
                                       new SqlParameter("@ShapeCode",shapeCode),
                                       new SqlParameter("@ImgS",image),
                                       new SqlParameter("@Remark",remark),
                                       new SqlParameter("@Description",description)
                                   };
            paras[0].Value = result;
            paras[0].Direction = ParameterDirection.InputOutput;


            ExecuteNonQuery("P_UpdateProductDetail", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        public bool DeleteProductDetail(string productDetailID, string operateid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                       new SqlParameter("@Result",result),
                                       new SqlParameter("@ProductDetailID",productDetailID),
                                       new SqlParameter("@OperateID",operateid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            bool bl = ExecuteNonQuery("P_DeleteProductDetail", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            return bl;
        }

        #endregion
    }
}
