using CloudSalesDAL;
using CloudSalesEntity;
using CloudSalesEnum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;

namespace CloudSalesBusiness
{
    public class StockBusiness
    {
        public static StockBusiness BaseBusiness = new StockBusiness();

        #region 查询

        public static List<StorageDoc> GetPurchases(string userid, EnumDocStatus status, string keywords, string begintime, string endtime, string wareid, string providerid,int sourcetype, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            DataSet ds = StockDAL.GetPurchases(userid, (int)status, keywords, begintime, endtime, wareid, providerid, sourcetype,pageSize, pageIndex, ref totalCount, ref pageCount, clientid);

            List<StorageDoc> list = new List<StorageDoc>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StorageDoc model = new StorageDoc();
                model.FillData(dr);
                var user = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                model.UserName = user != null ? user.Name : "";
                model.StatusStr = GetDocStatusStr(model.DocType, model.Status);
                var ware = SystemBusiness.BaseBusiness.GetWareByID(model.WareID, model.ClientID);
                model.WareName = ware != null ? ware.Name : "";

                if (ds.Tables.Contains("Details"))
                {
                    model.Details = new List<StorageDetail>();
                    foreach (DataRow ddr in ds.Tables["Details"].Select("DocID='" + model.DocID + "'"))
                    {
                        StorageDetail detail = new StorageDetail();
                        detail.FillData(ddr);
                        model.Details.Add(detail);
                    }
                }

                list.Add(model);
            }
            return list;
        }

        public static DataTable GetPurchasesByExcel(string userid, int status, string keywords, string begintime,
            string endtime, string wareid, string providerid, int doctype, string clientid)
        {
            return StockDAL.GetPurchasesByExcel(userid, status, keywords, begintime, endtime, wareid, providerid,
                doctype, clientid);
        }

        public static List<StorageDoc> GetStorageDocList(string userid, EnumDocType type, EnumDocStatus status, string keywords, string begintime, string endtime, string wareid, string providerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            DataSet ds = StockDAL.GetStorageDocList(userid, (int)type, (int)status, keywords, begintime, endtime, wareid, providerid, pageSize, pageIndex, ref totalCount, ref pageCount, clientid);

            List<StorageDoc> list = new List<StorageDoc>();
            if (ds.Tables.Contains("Doc"))
            {
                foreach (DataRow dr in ds.Tables["Doc"].Rows)
                {
                    StorageDoc model = new StorageDoc();
                    model.FillData(dr);
                    var user = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                    model.UserName = user != null ? user.Name : "";
                    model.StatusStr = GetDocStatusStr(model.DocType, model.Status);
                    var ware = SystemBusiness.BaseBusiness.GetWareByID(model.WareID, model.ClientID);
                    model.WareName = ware != null ? ware.Name : "";

                    if (ds.Tables.Contains("Details"))
                    {
                        model.Details = new List<StorageDetail>();
                        foreach (DataRow ddr in ds.Tables["Details"].Select("DocID='" + model.DocID + "'")) 
                        {
                            StorageDetail detail = new StorageDetail();
                            detail.FillData(ddr);
                            var depot = SystemBusiness.BaseBusiness.GetDepotByID(detail.DepotID, model.WareID, clientid);
                            if (depot != null)
                            {
                                detail.DepotCode = depot.DepotCode;
                            }
                            model.Details.Add(detail);
                        }
                    }

                    list.Add(model);
                }
            }
            return list;
        }

        public static StorageDoc GetStorageDetail(string docid, string agentid, string clientid)
        {
            DataSet ds = StockDAL.GetStorageDetail(docid, clientid);
            StorageDoc model = new StorageDoc();
            if (ds.Tables.Contains("Doc") && ds.Tables["Doc"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Doc"].Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                model.StatusStr = GetDocStatusStr(model.DocType, model.Status);

                model.DocTypeStr = CommonBusiness.GetEnumDesc<EnumDocType>((EnumDocType)model.DocType);

                model.WareHouse = SystemBusiness.BaseBusiness.GetWareByID(model.WareID, model.ClientID);

                model.Details = new List<StorageDetail>();
                foreach (DataRow item in ds.Tables["Details"].Rows)
                {
                    StorageDetail details = new StorageDetail();
                    details.FillData(item);
                    var depot = SystemBusiness.BaseBusiness.GetDepotByID(details.DepotID, model.WareID, clientid);
                    if (depot != null)
                    {
                        details.DepotCode = depot.DepotCode;
                    }
                    model.Details.Add(details);
                }
            }

            return model;
        }

        public static string GetDocStatusStr(int doctype, int status)
        {
            string str = "";
            switch (status)
            {
                case 0:
                    str = doctype == 6 ? "待入库"
                        : doctype == 2 ? "待审核"
                        : "待审核";
                    break;
                case 1:
                    str = doctype == 1 ? "部分入库"
                        : doctype == 2 ? "部分出库"
                        : "部分审核";
                    break;
                case 2:
                    str = doctype == 1 ? "已入库"
                        : doctype == 2 ? "已出库"
                        : doctype == 6 ? "已入库"
                        : "已审核";
                    break;
                case 3:
                    str ="待处理";
                    break;
                case 4:
                    str = "已作废";
                    break;
                case 9:
                    str = "已删除";
                    break;
            }
            return str;
        }

        public List<Products> GetProductStocks(string keywords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            DataSet ds = StockDAL.BaseProvider.GetProductStocks(keywords, pageSize, pageIndex, ref totalCount, ref pageCount, clientid);

            List<Products> list = new List<Products>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Products model = new Products();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<ProductDetail> GetProductDetailStocks(string productid, string agentid, string clientid)
        {
            DataTable dt = StockDAL.BaseProvider.GetProductDetailStocks(productid, clientid);

            List<ProductDetail> list = new List<ProductDetail>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductDetail model = new ProductDetail();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<ProductStock> GetDetailStocks(string wareid, string keywords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            //DataSet ds = StockDAL.BaseProvider.GetDetailStocks(wareid, keywords, pageSize, pageIndex, ref totalCount, ref pageCount, clientid);
            DataTable dt = GetDetailStocksDataTable(wareid, keywords, pageSize, pageIndex, ref totalCount, ref pageCount, clientid); ;
            List<ProductStock> list = new List<ProductStock>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductStock model = new ProductStock();
                model.FillData(dr);

                list.Add(model);
            }
            return list;
        }

        public DataTable GetDetailStocksDataTable(string wareid, string keywords, int pageSize, int pageIndex, ref int totalCount,
            ref int pageCount, string clientid)
        {
           return StockDAL.BaseProvider.GetDetailStocks(wareid, keywords, pageSize, pageIndex, ref totalCount,
                    ref pageCount, clientid).Tables[0];
        }

        public List<ProductStock> GetProductsByKeywords(string wareid, string keywords, string agentid, string clientid)
        {
            DataSet ds = StockDAL.BaseProvider.GetProductsByKeywords(wareid, keywords, clientid);

            List<ProductStock> list = new List<ProductStock>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ProductStock model = new ProductStock();
                model.FillData(dr);
                var ware = SystemBusiness.BaseBusiness.GetWareByID(model.WareID, clientid);
                model.WareName = ware != null ? ware.Name : "";
                var depot = SystemBusiness.BaseBusiness.GetDepotByID(model.DepotID, model.WareID, clientid);
                model.DepotCode = depot != null ? depot.DepotCode : "";
                list.Add(model);
            }
            return list;
        }

        public static List<StorageDoc> GetStorageDocDetails(string docid, string agentid)
        {
            DataSet ds = StockDAL.BaseProvider.GetStorageDocDetails(docid);

            List<StorageDoc> list = new List<StorageDoc>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StorageDoc model = new StorageDoc();
                model.FillData(dr);
                var user = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                model.UserName = user != null ? user.Name : "";

                if (ds.Tables.Contains("Details"))
                {
                    model.Details = new List<StorageDetail>();
                    foreach (DataRow ddr in ds.Tables["Details"].Select("DocID='" + model.DocID + "'"))
                    {
                        StorageDetail detail = new StorageDetail();
                        detail.FillData(ddr);
                        model.Details.Add(detail);
                    }
                }

                list.Add(model);
            }
            return list;
        }

        #endregion

        #region 添加

        public static bool CreateStorageDoc(string wareid, string ids, string remark, string userid, string operateip, string agentid, string clientid)
        {

            string guid = Guid.NewGuid().ToString();
            bool bl = StockDAL.AddStorageDoc(guid, (int)EnumDocType.RK, ids, remark, wareid, userid, operateip, clientid);
            if (bl)
            {
                //日志
                LogBusiness.AddActionLog(CloudSalesEnum.EnumSystemType.Client, CloudSalesEnum.EnumLogObjectType.StockIn, EnumLogType.Create, "", userid, agentid, clientid);
            }
            return bl;
        }

        public static string AddPurchaseDoc(string productid, string ids, string cmClientID, decimal totalfee, string remark, string wareid,int sourcetype, string userid,string agentid, string clientid
            ,string personname,string mobilephone,string address,string citycode)
        {
            string guid = Guid.NewGuid().ToString();
            bool bl = StockDAL.AddPurchaseDoc(guid, productid, (int)EnumDocType.RK, ids, cmClientID, totalfee, remark, wareid, sourcetype, userid, clientid, agentid, personname, mobilephone, address, citycode);
            if (bl)
            {
                //日志
                LogBusiness.AddActionLog(CloudSalesEnum.EnumSystemType.Client, CloudSalesEnum.EnumLogObjectType.StockIn, EnumLogType.Create, "", userid, agentid, clientid);
                return guid;
            }
            return "";
        }

        public bool SubmitDamagedDoc(string ids, string remark, string userid, string operateip, string clientid)
        {
            string guid = Guid.NewGuid().ToString();
            bool bl = StockDAL.SubmitDamagedDoc(guid, (int)EnumDocType.BS, 0, remark, ids, userid, operateip, clientid);
            return bl;
        }

        /// <summary>
        /// 出库按报损逻辑
        /// </summary>
        public bool SubmitHandOutDoc(string wareid, string remark, string userid, string operateip, string agentid, string clientid)
        {
            string guid = Guid.NewGuid().ToString();
            bool bl = StockDAL.SubmitDamagedDoc(guid, (int)EnumDocType.SGCK, 0, remark, wareid, userid, operateip, clientid);
            if (bl)
            {
                LogBusiness.AddActionLog(CloudSalesEnum.EnumSystemType.Client, CloudSalesEnum.EnumLogObjectType.StockOut, EnumLogType.Create, "", userid, agentid, clientid);
            }
            return bl;
        }

        public bool SubmitOverflowDoc(string wareid, string ids, string remark, string userid, string operateip, string clientid)
        {
            string guid = Guid.NewGuid().ToString();
            bool bl = StockDAL.SubmitOverflowDoc(guid, (int)EnumDocType.BY, ids, remark, wareid, userid, operateip, clientid);
            return bl;
        }

        public int AddDocPart(string orderid, string remarks, string nums, ref string errinfo, string userid = "", string zngcAutoID="")
        {
            return StockDAL.AddDocPart(orderid, remarks, nums, userid,zngcAutoID,ref errinfo);
        }

        #endregion

        #region 编辑、删除

        public bool DeleteDoc(string docid, string userid, string operateip, string clientid)
        {
            return new StockDAL().UpdateStorageStatus(docid, (int)EnumDocStatus.Delete, "删除单据", userid, operateip, clientid);
        }

        public bool InvalidDoc(string docid, string userid, string operateip, string clientid)
        {
            return new StockDAL().UpdateStorageStatus(docid, (int)EnumDocStatus.Invalid, "作废单据", userid, operateip, clientid);
        }

        public bool UpdateStorageDetailWare(string docid, string autoid, string wareid, string depotid, string userid, string operateip, string clientid)
        {
            return new StockDAL().UpdateStorageDetailWare(docid, autoid, wareid, depotid);
        }

        public bool UpdateStorageDetailBatch(string docid, string autoid, string batch, string userid, string operateip, string clientid)
        {
            return new StockDAL().UpdateStorageDetailBatch(docid, autoid, batch);
        }

        public bool AuditStorageIn(string docid, int doctype, int isover, string details, string remark, string userid, string operateip, string agentid, string clientid, ref int result, ref string errinfo)
        {
            bool bl = new StockDAL().AuditStorageIn(docid, doctype, isover, details, remark, userid, operateip, agentid, clientid, ref result, ref errinfo);
            return bl;
        }

        public bool AuditReturnIn(string docid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            return StockDAL.BaseProvider.AuditReturnIn(docid, userid, agentid, clientid, ref result, ref errinfo);
        }

        public bool AuditDamagedDoc(string docid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            return StockDAL.BaseProvider.AuditDamagedDoc(docid, userid, agentid, clientid, ref result, ref errinfo);
        }

        public bool AuditOverflowDoc(string docid, string userid, string agentid, string clientid, ref int result, ref string errinfo)
        {
            return StockDAL.BaseProvider.AuditOverflowDoc(docid, userid, agentid, clientid, ref result, ref errinfo);
        }

        public bool AuditDocPart(string docid, string originid, int isover, string details, string remark, string userid, string operateip, string agentid, string clientid, ref int result, ref string errinfo)
        {
            return StockDAL.AuditDocPart(docid, originid, isover, details, remark, userid, operateip, agentid, clientid,
                ref result, ref errinfo);
        }
        #endregion
    }
}
