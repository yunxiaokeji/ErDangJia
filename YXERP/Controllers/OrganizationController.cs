﻿using CloudSalesBusiness;
using CloudSalesEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YXERP.Models;

namespace YXERP.Controllers
{
    public class OrganizationController : BaseController
    {
        //
        // GET: /Organization/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Department()
        {
            return View();
        }

        public ActionResult Roles()
        {
            return View();
        }

        public ActionResult RolePermission(string id)
        {
            ViewBag.Model = OrganizationBusiness.GetRoleByID(id, CurrentUser.AgentID);
            ViewBag.Menus = CommonBusiness.ClientMenus.Where(m => m.PCode == ExpandClass.CLIENT_TOP_CODE).ToList();
            return View();
        }

        public ActionResult Users()
        {
            ViewBag.MDToken = CurrentUser.MDToken;
            ViewBag.Roles = OrganizationBusiness.GetRoles(CurrentUser.AgentID);
            ViewBag.Departments = OrganizationBusiness.GetDepartments(CurrentUser.AgentID);
            return View();
        }

        public ActionResult CreateUser()
        {
            ViewBag.Roles = OrganizationBusiness.GetRoles(CurrentUser.AgentID);
            ViewBag.Departments = OrganizationBusiness.GetDepartments(CurrentUser.AgentID);
            return View();
        }

        public ActionResult Structure()
        {
            var list = OrganizationBusiness.GetStructureByParentID("6666666666", CurrentUser.AgentID);
            if (list.Count > 0)
            {
                ViewBag.Model = list[0];
            }
            else
            {
                ViewBag.Model = new Users();
            }
            return View();
        }

        #region Ajax

        public JsonResult GetDepartments()
        {
            var list = OrganizationBusiness.GetDepartments(CurrentUser.AgentID);
            foreach (var item in list)
            {
                if (item.CreateUser == null && !string.IsNullOrEmpty(item.CreateUserID))
                {
                    var user = OrganizationBusiness.GetUserByUserID(item.CreateUserID, CurrentUser.AgentID);
                    item.CreateUser = new Users() { Name = user.Name };
                }
            }
            JsonDictionary.Add("items", list);
            return new JsonResult() 
            {
                Data = JsonDictionary,
                JsonRequestBehavior=JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetDepartmentByID(string id)
        {
            var model = OrganizationBusiness.GetDepartmentByID(id, CurrentUser.AgentID);
            JsonDictionary.Add("model", model);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveDepartment(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Department model = serializer.Deserialize<Department>(entity);

            if (string.IsNullOrEmpty(model.DepartID))
            {
                model.DepartID = new OrganizationBusiness().CreateDepartment(model.Name, model.ParentID, model.Description, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new OrganizationBusiness().UpdateDepartment(model.DepartID, model.Name, model.Description, CurrentUser.UserID, OperateIP, CurrentUser.AgentID);
                if (!bl)
                {
                    model.DepartID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteDepartment(string departid)
        {
            var status = new OrganizationBusiness().UpdateDepartmentStatus(departid, CloudSalesEnum.EnumStatus.Delete, CurrentUser.UserID, OperateIP, CurrentUser.AgentID);
            JsonDictionary.Add("status", (int)status);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetRoles()
        {
            var list = OrganizationBusiness.GetRoles(CurrentUser.AgentID);
            foreach (var item in list)
            {
                if (item.CreateUser == null && !string.IsNullOrEmpty(item.CreateUserID))
                {
                    var user = OrganizationBusiness.GetUserByUserID(item.CreateUserID, CurrentUser.AgentID);
                    item.CreateUser = new Users() { Name = user.Name };
                }
            }
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetRoleByID(string id)
        {
            var model = OrganizationBusiness.GetRoleByID(id, CurrentUser.AgentID);
            JsonDictionary.Add("model", model);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveRole(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Role model = serializer.Deserialize<Role>(entity);

            if (string.IsNullOrEmpty(model.RoleID))
            {
                model.RoleID = new OrganizationBusiness().CreateRole(model.Name, model.ParentID, model.Description, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new OrganizationBusiness().UpdateRole(model.RoleID, model.Name, model.Description, CurrentUser.UserID, OperateIP, CurrentUser.AgentID);
                if (!bl)
                {
                    model.RoleID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteRole(string roleid)
        {
            int result = 0;
            bool bl = new OrganizationBusiness().DeleteRole(roleid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, out result);
            JsonDictionary.Add("status", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveRolePermission(string roleid, string permissions)
        {
            if (permissions.Length > 0)
            {
                permissions = permissions.Substring(0, permissions.Length - 1);
               
            }
            bool bl = new OrganizationBusiness().UpdateRolePermission(roleid, permissions, CurrentUser.UserID, OperateIP);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetMDUsers()
        {
            if (!string.IsNullOrEmpty(CurrentUser.MDToken))
            {
                var list = MD.SDK.UserBusiness.GetUserAll(CurrentUser.MDToken, "", 0, 1000).users;
                JsonDictionary.Add("status", true);
                JsonDictionary.Add("items", list.OrderBy(u => u.name));
            }
            else 
            {
                JsonDictionary.Add("status", false);
            }
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveMDUser(string parentid, string mduserids)
        {
            bool bl = false;

            string[] list = mduserids.Split(',');
            foreach (string mduserid in list)
            {
                if (!string.IsNullOrEmpty(mduserid) && !string.IsNullOrEmpty(CurrentUser.MDToken))
                {
                    var model = MD.SDK.UserBusiness.GetUserDetail(CurrentUser.MDToken, mduserid);
                    if (model.error_code <= 0)
                    {
                        var user = model.user;
                        int result = 0;

                        bool isAdmin = false;//MD.SDK.Entity.App.AppBusiness.IsAppAdmin(CurrentUser.MDToken, user.id, out error);

                        OrganizationBusiness.CreateUser("", "", user.name, user.mobile_phone, user.email, "", "", "", "", "", parentid, CurrentUser.AgentID, CurrentUser.ClientID, user.id, user.project.id, isAdmin ? 1 : 0, CurrentUser.UserID, out result);
                        //添加成功
                        if (result == 1)
                        {
                            bl = true;
                        }
                    }
                }
            }

            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveUser(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Users model = serializer.Deserialize<Users>(entity);

            int result = 0;

            var user = OrganizationBusiness.CreateUser(model.LoginName, model.LoginName, model.Name, model.MobilePhone, model.Email,model.CityCode,model.Address,model.Jobs,model.RoleID,model.DepartID, "",
                CurrentUser.AgentID, CurrentUser.ClientID, "", "", 0, CurrentUser.UserID, out result);

            JsonDictionary.Add("model", user);
            JsonDictionary.Add("result", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult IsExistLoginName(string loginname)
        {
            var bl = OrganizationBusiness.IsExistLoginName(loginname);
            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUsersByParentID(string parentid = "")
        {
            var list = OrganizationBusiness.GetUsersByParentID(parentid, CurrentUser.AgentID).OrderBy(m => m.FirstName).ToList();
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUsers(string filter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            FilterUser model = serializer.Deserialize<FilterUser>(filter);
            int totalCount = 0;
            int pageCount = 0;

            var list = OrganizationBusiness.GetUsers(model.Keywords, model.DepartID, model.RoleID, CurrentUser.AgentID, PageSize, model.PageIndex, ref totalCount, ref pageCount);

            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUserAll()
        {
            var list = OrganizationBusiness.GetUsers(CurrentUser.AgentID).Where(m => m.Status == 1).OrderBy(m => m.FirstName).ToList();
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUserNoTeam() 
        {
            var list = OrganizationBusiness.GetUsers(CurrentUser.AgentID).Where(m => m.TeamID == "");
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateUserParentID(string ids, string parentid)
        {
            bool bl = false;//
            string[] list = ids.Split(',');
            foreach (var userid in list)
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    if (new OrganizationBusiness().UpdateUserParentID(userid, parentid, CurrentUser.AgentID, CurrentUser.UserID, OperateIP))
                    {
                        bl = true;
                    }
                }
            }
            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ClearUserParentID(string ids, string parentid)
        {
            bool bl = false;//
            string[] list = ids.Split(',');
            foreach (var userid in list)
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    if (new OrganizationBusiness().UpdateUserParentID(userid, parentid, CurrentUser.AgentID, CurrentUser.UserID, OperateIP))
                    {
                        bl = true;
                    }
                }
            }
            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ChangeUsersParentID(string userid, string olduserid)
        {

            bool bl = new OrganizationBusiness().ChangeUsersParentID(userid, olduserid, CurrentUser.AgentID, CurrentUser.UserID, OperateIP);
           
            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteUserByID(string userid)
        {
            int result = 0;
            bool bl = new OrganizationBusiness().DeleteUserByID(userid, CurrentUser.AgentID, CurrentUser.UserID, OperateIP, out result);

            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateUserRole(string userid,string roleid)
        {
            bool bl = new OrganizationBusiness().UpdateUserRole(userid, roleid, CurrentUser.AgentID, CurrentUser.UserID, OperateIP);

            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
