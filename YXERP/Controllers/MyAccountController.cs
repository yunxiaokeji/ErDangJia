﻿using CloudSalesBusiness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace YXERP.Controllers
{
    
    public class MyAccountController :BaseController
    {
        /// <summary>
        /// 文件默认存储路径
        /// </summary>
        public static string FilePath = CloudSalesTool.AppSettings.Settings["UploadFilePath"];
        public static string TempPath = CloudSalesTool.AppSettings.Settings["UploadTempPath"];

        #region view

        public ActionResult Index(string id)
        {
            ViewBag.Departments = OrganizationBusiness.GetDepartments(CurrentUser.AgentID);

            id = id ?? "-1";
            ViewBag.Option = id;
            return View();
        }

        public ActionResult SetAvatar()
        {
            return View();
        }
        public ActionResult SetPassWord()
        {
            return View();
        }
        public ActionResult Account()
        {
            return View();
        }
        #endregion

        #region ajax

        #region 基本信息
        /// <summary>
        /// 获取当前用户详情
        /// </summary>
        public JsonResult GetAccountDetail() 
        {
            JsonDictionary.Add("LoginName", CurrentUser.LoginName);
            JsonDictionary.Add("Name", CurrentUser.Name);
            JsonDictionary.Add("Jobs", CurrentUser.Jobs);
            JsonDictionary.Add("Birthday", CurrentUser.Birthday);
            JsonDictionary.Add("Age", CurrentUser.Age);
            JsonDictionary.Add("DepartID", CurrentUser.DepartID);
            JsonDictionary.Add("DepartmentName", CurrentUser.Department != null ? CurrentUser.Department.Name : string.Empty);
            JsonDictionary.Add("MobilePhone", CurrentUser.MobilePhone);
            JsonDictionary.Add("OfficePhone", CurrentUser.OfficePhone);
            JsonDictionary.Add("Email", CurrentUser.Email);
            JsonDictionary.Add("BindMobilePhone", CurrentUser.BindMobilePhone);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存用户基本信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult SaveAccountInfo(string entity, string departmentName)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CloudSalesEntity.Users model = serializer.Deserialize<CloudSalesEntity.Users>(entity);


            bool flag = OrganizationBusiness.UpdateUserInfo(CurrentUser.UserID, model.Name, model.Jobs, model.Birthday, 0, model.DepartID, model.Email, model.MobilePhone, model.OfficePhone, CurrentUser.AgentID);
            JsonDictionary.Add("Result", flag?1:0);

            if (flag)
            {
                CurrentUser.Name = model.Name;
                CurrentUser.Jobs = model.Jobs;
                CurrentUser.Birthday = model.Birthday;
                CurrentUser.Age = model.Age;
                if (CurrentUser.DepartID != model.DepartID)
                {
                    CurrentUser.DepartID = model.DepartID;
                    CurrentUser.Department = OrganizationBusiness.GetDepartmentByID(model.DepartID, CurrentUser.AgentID);
                }
                CurrentUser.Email = model.Email;
                CurrentUser.MobilePhone = model.MobilePhone;
                CurrentUser.OfficePhone = model.OfficePhone;
                Session["ClientManager"] = CurrentUser;
            }

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region 设置头像
        /// <summary>
        /// 设置用户头像
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        public JsonResult SaveAccountAvatar(string avatar)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(avatar))
            {
                avatar = avatar.Split(',')[1];             
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(avatar));
                Bitmap img = new Bitmap(stream);

                avatar = FilePath + CurrentUser.UserID+".png";
                img.Save(Server.MapPath(avatar));

                bool flag= OrganizationBusiness.UpdateAccountAvatar(CurrentUser.UserID, avatar, CurrentUser.AgentID);

                if (flag)
                {
                    result = 1;
                    CurrentUser.Avatar = avatar;
                    Session["ClientManager"] = CurrentUser;
                }
                    
            }

            JsonDictionary.Add("Result",result);
            JsonDictionary.Add("Avatar", avatar);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #region 账户管理
        /// <summary>
        /// 账号是否存在
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public JsonResult IsExistLoginName(string loginName)
        {
            bool bl = OrganizationBusiness.IsExistLoginName(loginName);
            JsonDictionary.Add("Result", bl);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 验证账号密码是否正确
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        public JsonResult ConfirmLoginPwd(string loginName, string loginPwd)
        {
            if (string.IsNullOrEmpty(loginName)) 
            {
                loginName = CurrentUser.LoginName;
            }
            bool bl = OrganizationBusiness.ConfirmLoginPwd(loginName, loginPwd);
            JsonDictionary.Add("Result", bl);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        ///编辑用户账户
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="loginPwd"></param>
        /// <returns></returns>
        public JsonResult UpdateUserAccount(string loginName, string loginPwd)
        {
            bool bl = OrganizationBusiness.UpdateUserAccount(CurrentUser.UserID, loginName, loginPwd, CurrentUser.AgentID);
            JsonDictionary.Add("Result", bl);

            if (bl) {
                CurrentUser.LoginName = loginName;
                Session["ClientManager"] = CurrentUser;
            }

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateUserPass(string loginPwd)
        {
            bool bl = OrganizationBusiness.UpdateUserPass(CurrentUser.UserID, loginPwd, CurrentUser.AgentID);
            JsonDictionary.Add("Result", bl);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 绑定手机
        public JsonResult SaveAccountBindMobile(string bindMobile, int option,string code)
        {
            bool flag = false;
            int result = 0;

            bool bl = Common.Common.ValidateMobilePhoneCode(bindMobile, code);
            if (!bl){
                result = 2;
            }
            else
            {
                if (option == 1)
                    flag = OrganizationBusiness.UpdateAccountBindMobile(CurrentUser.UserID, bindMobile, CurrentUser.AgentID);
                else
                    flag = OrganizationBusiness.ClearAccountBindMobile(CurrentUser.UserID, CurrentUser.AgentID);

                if (flag)
                {
                    if (option == 1)
                        CurrentUser.BindMobilePhone = bindMobile;
                    else
                        CurrentUser.BindMobilePhone = string.Empty;

                    Session["ClientManager"] = CurrentUser;
                    Common.Common.ClearMobilePhoneCode(bindMobile);
                    result = 1;
                }
            }

            JsonDictionary.Add("Result",result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

        #endregion

    }
}