using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CloudSalesBusiness;
using CloudSalesEnum;
using CloudSalesEntity;

namespace YXERP.Controllers
{
    public class StockController : BaseController
    {
        //
        // GET: /Stock/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Stocks()
        {
            return View();
        }

        public ActionResult Detail(string id)
        {
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.AgentID, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID))
            {
                return Redirect("/Stock/StorageDoc");
            }
            ViewBag.Model = model;
            return View();
        }

        public ActionResult DetailStocks()
        {
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            return View();
        }

        public ActionResult StorageDoc()
        {
            return View();
        }

        public ActionResult ReturnIn()
        {
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            return View();
        }

        public ActionResult ReturnInDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Stock/ReturnIn");
            }
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.AgentID, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID) || model.Status > 0)
            {
                return Redirect("/Stock/Detail/" + model.DocID);
            }
            ViewBag.Wares = new SystemBusiness().GetWareHouses(CurrentUser.ClientID);
            ViewBag.Model = model;
            return View();
        }

        public ActionResult Damaged()
        {
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            return View();
        }

        public ActionResult CreateDamaged(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = CurrentUser.UserID;
            }
            var list = ShoppingCartBusiness.GetShoppingCart(EnumDocType.BS, id, CurrentUser.UserID, CurrentUser.ClientID);
            Dictionary<string, string> wares = new Dictionary<string, string>();
            foreach (var model in list)
            {
                if (!wares.ContainsKey(model.WareID))
                {
                    wares.Add(model.WareID, model.WareName);
                }
            }
            ViewBag.guid = id;
            ViewBag.wares = wares;
            ViewBag.Items = list;
            return View();
        }

        public ActionResult DamagedDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Stock/Damaged");
            }
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.AgentID, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID))
            {
                return Redirect("/Stock/Damaged");
            }
            if (model.Status >= 2)
            {
                return Redirect("/Stock/Detail/" + model.DocID);
            }
            ViewBag.Model = model;
            return View();
        }

        public ActionResult Overflow()
        {
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            return View();
        }

        public ActionResult CreateOverflow(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = CurrentUser.UserID;
            }
            var wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            ViewBag.wares = wares;
            ViewBag.Items = ShoppingCartBusiness.GetShoppingCart(EnumDocType.BY, id, CurrentUser.UserID, CurrentUser.ClientID);
            ViewBag.guid = id;
            return View();
        }

        public ActionResult OverflowDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Stock/Overflow");
            }
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.AgentID, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID))
            {
                return Redirect("/Stock/Overflow");
            }
            if (model.Status >= 2)
            {
                return Redirect("/Stock/Detail/" + model.DocID);
            }
            ViewBag.Model = model;
            return View();
        }

        public ActionResult HandOut()
        {
            ViewBag.Wares = SystemBusiness.BaseBusiness.GetWareHouses(CurrentUser.ClientID);
            return View();
        }

        public ActionResult CreateHandOut(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Stock/HandOut");
            }
            var ware = SystemBusiness.BaseBusiness.GetWareByID(id, CurrentUser.ClientID);
            if (ware == null || string.IsNullOrEmpty(ware.WareID))
            {
                return Redirect("/Stock/HandOut");
            }
            ViewBag.Ware = ware;
            ViewBag.Items = ShoppingCartBusiness.GetShoppingCart(EnumDocType.SGCK, ware.WareID, CurrentUser.UserID, CurrentUser.ClientID);
            return View();
        }

        public ActionResult HandOutDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Stock/HandOut");
            }
            var model = StockBusiness.GetStorageDetail(id, CurrentUser.AgentID, CurrentUser.ClientID);
            if (model == null || string.IsNullOrEmpty(model.DocID))
            {
                return Redirect("/Stock/HandOut");
            }
            if (model.Status >= 2)
            {
                return Redirect("/Stock/Detail/" + model.DocID);
            }
            ViewBag.Model = model;
            return View();
        }

        public ActionResult ChooseBYProducts()
        {
            var list = ShoppingCartBusiness.GetShoppingCart(EnumDocType.BY, CurrentUser.UserID, CurrentUser.UserID, CurrentUser.ClientID);
            string pids = "";
            list.ForEach(x => { if (!pids.Contains(x.ProductDetailID)) pids += x.ProductDetailID + ","; });
            ViewBag.Type = (int)EnumDocType.BY;
            ViewBag.GUID = CurrentUser.UserID;
            ViewBag.Pids = pids;
            ViewBag.Title = "选择报溢产品";
            return View("FilterProducts");
        }

        #region Ajax

        public JsonResult GetDetailStocks(string WareID,string Keywords, int PageIndex, int PageSize)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = StockBusiness.BaseBusiness.GetDetailStocks(WareID, Keywords, PageSize, PageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductStocks(string Keywords, int PageIndex, int PageSize)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = StockBusiness.BaseBusiness.GetProductStocks(Keywords, PageSize, PageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductDetailStocks(string productid)
        {
            var list = StockBusiness.BaseBusiness.GetProductDetailStocks(productid, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetStorageDocs(string keyWords, int pageIndex, int totalCount, int status = -1, int type = -1, string begintime = "", string endtime = "", string wareid = "")
        {
            int pageCount = 0;
            List<StorageDoc> list = StockBusiness.GetStorageDocList(string.Empty, (EnumDocType)type, (EnumDocStatus)status, keyWords, begintime, endtime, wareid, "", PageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateStorageDetailBatch(string docid, string autoid, string batch)
        {
            var bl = new StockBusiness().UpdateStorageDetailBatch(docid, autoid, batch, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AuditReturnIn(string docid)
        {
            int result = 0;
            string errinfo = "";

            var bl = StockBusiness.BaseBusiness.AuditReturnIn(docid, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetProductsByKeywords(string wareid, string keywords)
        {
            var list = StockBusiness.BaseBusiness.GetProductsByKeywords(wareid, keywords, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SubmitDamagedDoc(string ids, string remark)
        {
            var bl = false;

            if (!string.IsNullOrEmpty(ids))
            {
                ids = ids.Substring(0, ids.Length - 1);
                bl = StockBusiness.BaseBusiness.SubmitDamagedDoc(ids, remark, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            }

            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SubmitOverflowDoc(string ids, string wareid, string remark)
        {
            bool bl = false;
            if (!string.IsNullOrEmpty(wareid) && !string.IsNullOrEmpty(ids))
            {
                ids = ids.Substring(0, ids.Length - 1);
                bl = StockBusiness.BaseBusiness.SubmitOverflowDoc(wareid, ids, remark, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            }

            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteStorageDoc(string docid)
        {
            var bl = new StockBusiness().DeleteDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InvalidStorageDoc(string docid)
        {
            var bl = new StockBusiness().InvalidDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteDamagedDoc(string docid)
        {
            var bl = new StockBusiness().DeleteDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InvalidDamagedDoc(string docid)
        {
            var bl = new StockBusiness().InvalidDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AuditDamagedDoc(string docid)
        {
            int result = 0;
            string errinfo = "";
            var bl = new StockBusiness().AuditDamagedDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteOverflowDoc(string docid)
        {
            var bl = new StockBusiness().DeleteDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InvalidOverflowDoc(string docid)
        {
            var bl = new StockBusiness().InvalidDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AuditOverflowDoc(string docid)
        {
            int result = 0;
            string errinfo = "";
            var bl = new StockBusiness().AuditOverflowDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SubmitHandOutDoc(string wareid, string remark)
        {
            var bl = StockBusiness.BaseBusiness.SubmitHandOutDoc(wareid, remark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult InvalidHandOutDoc(string docid)
        {
            var bl = new StockBusiness().InvalidDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteHandOutDoc(string docid)
        {
            var bl = new StockBusiness().DeleteDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult AuditHandOutDoc(string docid)
        {
            int result = 0;
            string errinfo = "";//使用报损逻辑
            var bl = new StockBusiness().AuditDamagedDoc(docid, CurrentUser.UserID, OperateIP, CurrentUser.ClientID, ref result, ref errinfo);
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("errinfo", errinfo);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        #region 库存导出
        public ActionResult ExportFromStock(string WareID,string Keywords, bool test=false, string model = "", string filleName = "库存")
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, ExcelFormatter> dic = new Dictionary<string, ExcelFormatter>();
            Dictionary<string, ExcelModel> listColumn = new Dictionary<string, ExcelModel>();
            listColumn = GetColumnForJson("stockdetail", ref dic, !string.IsNullOrEmpty(model) ? model : "Item", test ? "testexport" : "export", CurrentUser.ClientID);
            var excelWriter = new ExcelWriter();
            foreach (var key in listColumn)
            {
                excelWriter.Map(key.Key, key.Value.Title);
            }
            byte[] buffer;
            DataTable dt = new DataTable(); 
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
                int total = 0;
                dt = new StockBusiness().GetDetailStocksDataTable(WareID, Keywords,  int.MaxValue,1, ref total,
                    ref total, CurrentUser.ClientID);
                if (!dt.Columns.Contains("stocklast"))
                {
                    dt.Columns.Add("stocklast", Type.GetType("System.Int32"));
                }
                foreach (DataRow drRow in dt.Rows)
                {
                    drRow["stocklast"] = Convert.ToInt32(drRow["stockin"]) - Convert.ToInt32(drRow["stockout"]);
                } 
            }
            buffer = excelWriter.Write(dt, dic, "");
            var fileName = CurrentUser.Client.CompanyName + filleName + (test ? "导入模版" : "") + DateTime.Now.ToString("yyyyMMdd");
            if (!Request.ServerVariables["http_user_agent"].ToLower().Contains("firefox"))
                fileName = HttpUtility.UrlEncode(fileName);
            this.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");
            return File(buffer, "application/ms-excel");
        }

        #endregion
        #endregion
    }
}
