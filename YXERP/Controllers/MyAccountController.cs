using CloudSalesBusiness; 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CloudSalesBusiness.Manage;
using CloudSalesEntity;

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
            ViewBag.User = Session["ClientManager"];
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
            ViewBag.User = Session["ClientManager"];
            ViewBag.BindUrl = WeiXin.Sdk.Token.GetAuthorizeUrl(Server.UrlEncode(WeiXin.Sdk.AppConfig.BindCallBackUrl),
                "", false);
            return View();
        }

        public ActionResult MyFeedBack()
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
            JsonDictionary.Add("WeiXinID", CurrentUser.WeiXinID);
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
            excelWriter.Map("LoginName","登录名");
            excelWriter.Map("Name", "姓名");
            excelWriter.Map("MobilePhone", "手机号"); 
            excelWriter.Map("Birthday", "生日");
            byte[] buffer = excelWriter.Write(OrganizationBusiness.GetUserByIDNoCache(CurrentUser.UserID), new Dictionary<string, ExcelFormatter>() { { "birthday", new ExcelFormatter() { ColumnTrans = EnumColumnTrans.ConvertTime, DropSource = "" } } });
            var fileName = "用户信息导入";
            if (!Request.ServerVariables["http_user_agent"].ToLower().Contains("firefox"))
                fileName = HttpUtility.UrlEncode(fileName);
            this.Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");
            return File(buffer, "application/ms-excel");
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

        public JsonResult ConfirmLoginPwd(string userid, string loginName, string loginPwd)
        {
            if (string.IsNullOrEmpty(userid)) 
            {
                userid = CurrentUser.UserID;
            }
            bool bl = OrganizationBusiness.ConfirmLoginPwd(userid, loginName, loginPwd);
            JsonDictionary.Add("Result", bl);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateUserAccount(string loginName, string loginPwd)
        {
            bool bl = OrganizationBusiness.UpdateUserAccount(CurrentUser.UserID, loginName, loginPwd, CurrentUser.AgentID, CurrentUser.ClientID);
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
                {
                    flag = OrganizationBusiness.UpdateAccountBindMobile(CurrentUser.UserID, bindMobile, CurrentUser.AgentID, CurrentUser.ClientID);
                }
                else
                {
                    flag = OrganizationBusiness.ClearAccountBindMobile(CurrentUser.UserID, CurrentUser.AgentID);
                }
                if (flag)
                {
                    if (option == 1)
                    {
                        CurrentUser.BindMobilePhone = bindMobile;
                    }
                    else
                    {
                        CurrentUser.BindMobilePhone = string.Empty;
                    }

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

        #region 我的反馈
        public JsonResult GetFeedBacks(int pageIndex, int type, int status, string keyWords, string beginDate, string endDate)
        {
            string userID = CurrentUser.UserID;
            int totalCount = 0, pageCount = 0;
            var list = CloudSalesBusiness.Manage.FeedBackBusiness.GetFeedBacks(keyWords, beginDate, endDate, type, status,userID, PageSize, pageIndex, out totalCount, out pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetFeedBackDetail(string id)
        {
            var item = CloudSalesBusiness.Manage.FeedBackBusiness.GetFeedBackDetail(id);
            JsonDictionary.Add("Item", item);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion
        #region 绑定微信

        //微信授权地址
        public ActionResult WeiXinLogin(string ReturnUrl)
        {
            return Redirect(WeiXin.Sdk.Token.GetAuthorizeUrl(Server.UrlEncode(WeiXin.Sdk.AppConfig.BindCallBackUrl), ReturnUrl, false));
        }

        public ActionResult WeiXinBindUrl(string ReturnUrl)
        {
            return
                Content(WeiXin.Sdk.Token.GetAuthorizeUrl(Server.UrlEncode(WeiXin.Sdk.AppConfig.BindCallBackUrl),
                    ReturnUrl, false));
        }

        public ActionResult WeiXinCallBack(string code, string state)
        {
            string result ="0";
            string operateip = Common.Common.GetRequestIP();
            var userToken = WeiXin.Sdk.Token.GetAccessToken(code);

            if (string.IsNullOrEmpty(userToken.errcode))
            {
                var model = OrganizationBusiness.GetUserByWeiXinID(userToken.unionid, operateip);
                if (model == null)
                {
                    Session["WeiXinTokenInfo"] = userToken.access_token + "|" + userToken.openid + "|" +
                                                 userToken.unionid;
                    if (BindWeiXin() > 0)
                    {
                        Response.Write("<script> alert('绑定成功');window.close();</script>");
                        Response.End();
                        return Redirect(state);
                    }
                    else
                    {
                        Response.Write("<script>window.close();</script>");
                        Response.End();
                        return Content("");
                    }
                }
                else
                {
                    //未注销
                    if (model.Status.Value==1)
                    {
                        Session["ClientManager"] = model;
                        return Redirect("/Home/Index");
                    }
                    else
                    {
                        Response.Write("<script>alert('用户已被注销');window.close();</script>");
                        Response.End();
                        return Content("");
                    }
                }
            }
            else
            {
                Response.Write("<script>alert('暂未获取到微信用户信息，绑定失败');window.close();</script>");
                Response.End();
                return Content("");
            }
        }

        //绑定微信账户
        public int BindWeiXin()
        {
            int result = 0;
            if (Session["WeiXinTokenInfo"] != null)
            {
                string tokenInfo = Session["WeiXinTokenInfo"].ToString();
                string[] tokenArr = tokenInfo.Split('|');
                if (tokenArr.Length == 3)
                {
                    string access_token = tokenArr[0];
                    string unionid = tokenArr[2];
                    bool flag = ClientBusiness.BindUserWeiXinID(CurrentUser.ClientID, CurrentUser.UserID, unionid);
                    if (flag)
                    {
                        Users user = (Users)Session["ClientManager"];
                        user.WeiXinID = unionid; 
                        Session["ClientManager"] = user;
                        Session.Remove("WeiXinTokenInfo");
                        result = 1;
                    }
                }
            }
            else
            {
                result = -5;
            }

            return result;
        }

        public JsonResult UnBindWeiXin()
        {
            int result = 0;
            bool flag = ClientBusiness.BindUserWeiXinID(CurrentUser.ClientID, CurrentUser.UserID, "",1);
            if (flag)
            {
                if (Session["ClientManager"] != null)
                {
                    Users user = (Users) Session["ClientManager"];
                    user.WeiXinID = "";
                    Session["ClientManager"] = user;
                } 
                result = 1;
            }
            JsonDictionary.Add("Result", result);
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
