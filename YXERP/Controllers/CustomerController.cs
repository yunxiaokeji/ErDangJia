using CloudSalesEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesBusiness;
using System.Web.Script.Serialization;
using CloudSalesEntity;
using YXERP.Models;
using System.Data;
using CloudSalesBusiness.Manage;
using Xfrog.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Web.Services.Description; 
using NPOI.SS.Formula;

namespace YXERP.Controllers
{
    public class CustomerController : BaseController
    {
        //
        // GET: /Customer/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyCustomer()
        {
            ViewBag.Title = "我的客户";
            ViewBag.Type = (int)EnumSearchType.Myself;
            ViewBag.ColorData = SystemBusiness.BaseBusiness.GetCustomerColors(CurrentUser.ClientID).ToList();
            return View("Customers");
        }

        public ActionResult CustomerImport()
        { 
            return View();
        }

        public ActionResult CustomerOwnRPT()
        {
            ViewBag.UserID = CurrentUser.UserID;
            return View();
        }

        public ActionResult BranchCustomer()
        {
            ViewBag.Title = "下属客户";
            ViewBag.Type = (int)EnumSearchType.Branch;
            ViewBag.ColorData = SystemBusiness.BaseBusiness.GetCustomerColors( CurrentUser.ClientID).ToList();
            return View("Customers");
        }

        public ActionResult Customers()
        {
            ViewBag.Title = "所有客户";
            ViewBag.Type = (int)EnumSearchType.All;
            ViewBag.ColorData = SystemBusiness.BaseBusiness.GetCustomerColors(CurrentUser.ClientID).ToList();
            return View("Customers");
        }

        public ActionResult Create(string id)
        {
            int total = 0;
            ViewBag.ActivityID = id;
            ViewBag.Sources = new SystemBusiness().GetCustomSources(CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.Industrys = CloudSalesBusiness.Manage.IndustryBusiness.GetIndustrys();
            ViewBag.Activity = ActivityBusiness.GetActivitys("", EnumActivityStage.All, 0, "", "", "", "", int.MaxValue, 1, ref total, ref total, CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.Extents = CustomBusiness.GetExtents();
            return View();
        }

        public ActionResult Detail(string id, string nav)
        {
            ViewBag.MDToken = CurrentUser.MDToken;
            ViewBag.NavID = nav;
            ViewBag.ID = id;
            ViewBag.ColorData = SystemBusiness.BaseBusiness.GetCustomerColors(CurrentUser.ClientID);
            return View();
        }

        #region Ajax

        public JsonResult GetCustomerSources()
        {
            var list = new SystemBusiness().GetCustomSources(CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult ExportFromCfg()
        {
            var excelWriter = new ExcelWriter();
            Dictionary<string, object> listColumn = new Dictionary<string, object>();
            excelWriter.Map("Name", "公司名称");
            excelWriter.Map("Contcat", "客户名称");
            excelWriter.Map("Jobs", "职位");
            excelWriter.Map("MobilePhone", "联系电话");
            excelWriter.Map("Email", "邮箱");
            excelWriter.Map("Province", "省");
            excelWriter.Map("City", "市");
            excelWriter.Map("District", "区");
            excelWriter.Map("Address", "详细地址");
            excelWriter.Map("Description", "描述");
          
            byte[] buffer = excelWriter.Write(OrganizationBusiness.GetUserById(CurrentUser.UserID),
                new Dictionary<string, ExcelFormatter>()
                {
                    {
                        "birthday", new ExcelFormatter()
                        {
                            ColumnTrans = EnumColumnTrans.ConvertTime,
                            DropSource = ""

                        }
                    }
                });
            var fileName = "客户信息导入";
            if (!Request.ServerVariables["http_user_agent"].ToLower().Contains("firefox"))
                fileName = HttpUtility.UrlEncode(fileName);
            this.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");
            return File(buffer, "application/ms-excel");
        }

        public ActionResult ExportFromCustomer(bool test = false, string model = "", string filleName = "企业客户", string filter="")
        {
            Dictionary<string, ExcelFormatter> dic = new Dictionary<string, ExcelFormatter>();
             FilterCustomer qicCustomer =new FilterCustomer();
             Dictionary<string, ExcelModel> listColumn = new Dictionary<string, ExcelModel>();
            if (string.IsNullOrEmpty(filter))
            {
                listColumn = GetColumnForJson("customer", ref dic, model, test ? "test" : "export",CurrentUser.ClientID );
            }
            else
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer(); 
                qicCustomer=serializer.Deserialize<FilterCustomer>(filter);
                listColumn = GetColumnForJson("customer", ref dic, !string.IsNullOrEmpty(model) ? model : qicCustomer.ExcelType == 0 ? "Item" : "OwnItem", test ? "test" : "export", CurrentUser.ClientID);
            }
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
                    if (key.Key == "extent")
                    {
                        dr[key.Key] = "下拉框中选择";
                    }
                    else if (key.Key == "industryid")
                    {
                        dr[key.Key] = "下拉框中选择";
                    }
                    else if (key.Key == "province")
                    {
                        dr[key.Key] = "下拉框中选择";
                    }
                    else
                    {
                        dr[key.Key] = key.Value.DefaultText;
                    }
                }
                dt.Rows.Add(dr);
            }
            else
            {
                int totalCount = 0;
                int pageCount = 0; 
                //客户
                dt = CustomBusiness.BaseBusiness.GetCustomersDatable(qicCustomer.SearchType, qicCustomer.Type,
                    qicCustomer.SourceID, qicCustomer.StageID, qicCustomer.Status, qicCustomer.Mark,
                    qicCustomer.ActivityID, qicCustomer.UserID, qicCustomer.TeamID, qicCustomer.AgentID,
                    qicCustomer.BeginTime, qicCustomer.EndTime,
                    qicCustomer.Keywords, qicCustomer.OrderBy, int.MaxValue, 1, ref totalCount, ref pageCount,
                    CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, qicCustomer.ExcelType);
                if (!dt.Columns.Contains("province"))
                {
                    dt.Columns.Add("province", Type.GetType("System.String"));
                }
                if (!dt.Columns.Contains("citys"))
                {
                    dt.Columns.Add("citys", Type.GetType("System.String"));
                }
                if (!dt.Columns.Contains("counties"))
                {
                    dt.Columns.Add("counties", Type.GetType("System.String"));
                }
                foreach (DataRow drRow in dt.Rows)
                {
                  var city=  CommonBusiness.GetCityByCode(drRow["CityCode"].ToString());
                    if (city != null)
                    {
                        drRow["province"] = city.Province;
                        drRow["citys"] = city.City;
                        drRow["counties"] = city.Counties;
                    }
                }
            }
            buffer = excelWriter.Write(dt, dic);
            var fileName = filleName + (test ? "导入模版" : "");
            if (!Request.ServerVariables["http_user_agent"].ToLower().Contains("firefox"))
                fileName = HttpUtility.UrlEncode(fileName);
            this.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");
            return File(buffer, "application/ms-excel");

        }

        [HttpPost]
        public ActionResult CustomerImport(HttpPostedFileBase file, int overType = 0, int type=0)
        {
            if (file == null) { return Content("请选择要导入的文件"); }
            if (file.ContentLength > 2097152)
            {
                return Content("导入文件超过规定（2M )大小,请修改后再操作."); 
            }
            if (!file.FileName.Contains(".xls") && !file.FileName.Contains(".xlsx")) { return Content("文件格式类型不正确"); }
            string mes = "";
            try
            {
                DataTable dt = ImportExcelToDataTable(file);
                if (dt.Columns.Count > 0)
                {
                    ///1.获取系统模版列
                    var checkColumn = dt.Columns.Contains("公司规模");
                    Dictionary<string, string> listColumn;
                    if (checkColumn)
                    {
                        listColumn = GetColumnForJson("customer", "Item");
                    }
                    else
                    {
                        listColumn = GetColumnForJson("customer", "OwnItem");
                    }
                    ///2.上传Excel 与模板中列是否一致 不一致返回提示
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (!listColumn.ContainsKey(dc.ColumnName))
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
                    List<CustomerEntity> list = new List<CustomerEntity>();
                    List<ContactEntity> contactList = new List<ContactEntity>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            CustomerEntity customers;
                            ContactEntity contact;
                            if (checkColumn)
                            {
                                customers = GetCustomerByDataRow(dr, checkColumn);
                                list.Add(customers);
                            }
                            else
                            {
                                if (dr["客户名称"] != null && !string.IsNullOrEmpty(dr["客户名称"].ToString()))
                                {
                                    contact = GetContactByDataRow(dr, checkColumn);
                                    contactList.Add(contact);
                                }
                                else
                                {
                                    customers = GetCustomerByDataRow(dr, checkColumn);
                                    list.Add(customers);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            mes += k +" 原因:"+ex.Message+ ",";
                        }
                    }
                    try
                    {
                        if (list.Count > 0) ExcelImportBusiness.InsertCustomer(list, type, overType);
                        if (contactList.Count > 0) ExcelImportBusiness.InsertContact(contactList, type, overType);
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
            return Content("操作成功");
        }
         
        public CustomerEntity GetCustomerByDataRow(DataRow dr ,bool isQiYe=false)
        {
            CustomerEntity customers = new CustomerEntity();
            customers.Address = dr["详细地址"].ToString();
            customers.Description = dr["描述"].ToString();
            customers.Email = dr["邮箱"].ToString();
            customers.MobilePhone = dr["联系电话"].ToString();
            customers.Jobs = dr["职位"].ToString();
            customers.ClientID = CurrentUser.ClientID;
            customers.AgentID = CurrentUser.AgentID;
            customers.CreateUserID = CurrentUser.UserID;
            customers.OwnerID = CurrentUser.UserID;
            customers.CreateTime = DateTime.Now;
            customers.Type = isQiYe?1:0;
            customers.ContactName = dr["联系人"].ToString();
            customers.CustomerID = Guid.NewGuid().ToString();
            customers.CityCode = GetCityCode(dr);
            if (isQiYe)
            {
                customers.Name = dr["客户名称"].ToString();
                Industry industry = CommonBusiness.IndustryList.Where(x => x.Name.Equals(dr["行业"].ToString()))
                    .FirstOrDefault();
                customers.IndustryID = industry != null ? industry.IndustryID : "";
                customers.Extent =
                    (int)CommonBusiness.GetEnumindexByDesc<EnumCustomerExtend>(EnumCustomerExtend.Huge, dr["公司规模"].ToString());
            }
            else
            {
                customers.Name = dr["联系人"].ToString();
            }
            return customers;
        }

        public ContactEntity GetContactByDataRow(DataRow dr, bool isQiYe = false)
        {
            ContactEntity contact = new ContactEntity();
            contact.Address = dr["详细地址"].ToString();
            contact.CompanyName = dr["客户名称"].ToString(); 
            contact.Description = dr["描述"].ToString();
            contact.Email = dr["邮箱"].ToString();
            contact.MobilePhone = dr["联系电话"].ToString();
            contact.Jobs = dr["职位"].ToString();
            contact.CityCode = GetCityCode(dr);
            contact.ClientID = CurrentUser.ClientID;
            contact.AgentID = CurrentUser.AgentID;
            contact.CreateUserID = CurrentUser.UserID;
            contact.OwnerID = CurrentUser.UserID;
            contact.CustomerID = "";
            contact.Name = dr["联系人"].ToString();
            contact.CreateTime = DateTime.Now;
            contact.Type = isQiYe ? 1 : 0;
            contact.ContactID = Guid.NewGuid().ToString();
            return contact;
        } 

        public JsonResult SaveCustomer(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CustomerEntity model = serializer.Deserialize<CustomerEntity>(entity);

            if (string.IsNullOrEmpty(model.CustomerID))
            {
                model.CustomerID = new CustomBusiness().CreateCustomer(model.Name, model.Type, model.SourceID, model.ActivityID, model.IndustryID, model.Extent, model.CityCode,
                                                                       model.Address, model.ContactName, model.MobilePhone, model.OfficePhone, model.Email, model.Jobs, model.Description, CurrentUser.UserID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new CustomBusiness().UpdateCustomer(model.CustomerID, model.Name, model.Type, model.IndustryID, model.Extent, model.CityCode, model.Address, model.ContactName, model.MobilePhone, model.OfficePhone,
                                                model.Email, model.Jobs, model.Description, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.CustomerID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomers(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterCustomer model = serializer.Deserialize<FilterCustomer>(filter);
            int totalCount = 0;
            int pageCount = 0;

            List<CustomerEntity> list = CustomBusiness.BaseBusiness.GetCustomers(model.SearchType, model.Type, model.SourceID, model.StageID, model.Status, model.Mark, model.ActivityID, model.UserID, model.TeamID, model.AgentID, model.BeginTime, model.EndTime,
                                                                                 model.Keywords, model.OrderBy, model.PageSize, model.PageIndex, ref totalCount, ref pageCount, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomersByKeywords(string keywords, int isAll = 0)
        {

            List<CustomerEntity> list = CustomBusiness.BaseBusiness.GetCustomersByKeywords(keywords, isAll == 0 ? CurrentUser.UserID : "", CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomerByID(string customerid)
        {
            var model = CustomBusiness.BaseBusiness.GetCustomerByID(customerid, CurrentUser.AgentID, CurrentUser.ClientID);
            model.Industrys = CloudSalesBusiness.Manage.IndustryBusiness.GetIndustrys();
            model.Extents = CustomBusiness.GetExtents();
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetActivityBaseInfoByID(string activityid)
        {
            var model = ActivityBusiness.GetActivityBaseInfoByID(activityid, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateCustomMark(string ids, int mark)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerMark(id, mark, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }

            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateCustomOwner(string ids, string userid)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerOwner(id, userid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }


            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult LoseCustomer(string ids)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerStatus(id, EnumCustomStatus.Loses, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult CloseCustomer(string ids)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerStatus(id, EnumCustomStatus.Close, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult RecoveryCustomer(string ids)
        {
            bool bl = false;
            string[] list = ids.Split(',');
            foreach (var id in list)
            {
                if (!string.IsNullOrEmpty(id) && CustomBusiness.BaseBusiness.UpdateCustomerStatus(id, EnumCustomStatus.Normal, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID))
                {
                    bl = true;
                }
            }
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomerLogs(string customerid, int pageindex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = LogBusiness.GetLogs(customerid, EnumLogObjectType.Customer, 10, pageindex, ref totalCount, ref pageCount, CurrentUser.AgentID);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetContacts(string customerid)
        {
            var list = CustomBusiness.BaseBusiness.GetContactsByCustomerID(customerid, CurrentUser.AgentID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveContact(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ContactEntity model = serializer.Deserialize<ContactEntity>(entity);

            if (string.IsNullOrEmpty(model.ContactID))
            {
                model.ContactID = new CustomBusiness().CreateContact(model.CustomerID, model.Name, model.CityCode, model.Address, model.MobilePhone, model.OfficePhone, model.Email, model.Jobs, model.Description, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new CustomBusiness().UpdateContact(model.ContactID, model.CustomerID, model.Name, model.CityCode, model.Address, model.MobilePhone, model.OfficePhone, model.Email, model.Jobs, model.Description, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.ContactID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetContactByID(string id)
        {
            var model = CustomBusiness.BaseBusiness.GetContactByID(id);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateContactDefault(string id, string name, string customerid)
        {
            bool bl = CustomBusiness.BaseBusiness.UpdateContactDefault(id, name, customerid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteContact(string id, string name, string customerid)
        {
            bool bl = CustomBusiness.BaseBusiness.DeleteContact(id, name, customerid, OperateIP, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl );
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #region 讨论

        public JsonResult SavaReply(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ReplyEntity model = serializer.Deserialize<ReplyEntity>(entity);

            string replyID = "";
            replyID = CustomBusiness.CreateReply(model.GUID, model.Content, CurrentUser.UserID, CurrentUser.AgentID, model.FromReplyID, model.FromReplyUserID, model.FromReplyAgentID);

            List<ReplyEntity> list = new List<ReplyEntity>();
            if (!string.IsNullOrEmpty(replyID))
            {
                model.ReplyID = replyID;
                model.CreateTime = DateTime.Now;
                model.CreateUser = CurrentUser;
                model.CreateUserID = CurrentUser.UserID;
                model.AgentID = CurrentUser.AgentID;
                if (!string.IsNullOrEmpty(model.FromReplyUserID) && !string.IsNullOrEmpty(model.FromReplyAgentID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }
                list.Add(model);
            }
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetReplys(string guid, int pageSize, int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;

            var list = CustomBusiness.GetReplys(guid, pageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteReply(string replyid)
        {
            bool bl = CustomBusiness.BaseBusiness.DeleteReply(replyid);
            JsonDictionary.Add("status", bl ? 1 : 0);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }

        #endregion

        #endregion

    }
}
