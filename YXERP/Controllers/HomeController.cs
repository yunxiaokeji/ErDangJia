using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using MD.SDK.Business;
using CloudSalesBusiness;
using CloudSalesEntity;
using CloudSalesBusiness.Manage;
using CloudSalesEntity.Manage;

namespace YXERP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["ClientManager"] == null)
            {
                return Redirect("/Home/Login");
            }
            return Redirect("/Default/Index");
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult FindPassword()
        {
            return View();
        }

        public ActionResult Login(string ReturnUrl, int Status = 0)
        {
            if (Session["ClientManager"] != null)
            {
                return Redirect("/Default/Index");
            }
            HttpCookie cook = Request.Cookies["yunxiao_erp_user"];
            if (cook != null)
            {
                if (cook["status"] == "1")
                {
                    string operateip = Common.Common.GetRequestIP();
                    int result;
                    CloudSalesEntity.Users model = CloudSalesBusiness.OrganizationBusiness.GetUserByUserName(cook["username"], cook["pwd"],out result, operateip);
                    if (model != null)
                    {
                        Session["ClientManager"] = model;
                        return Redirect("/Default/Index");
                    }
                }
                else
                {
                    ViewBag.UserName = cook["username"];
                }
            }
            ViewBag.Status = Status;
            ViewBag.ReturnUrl = ReturnUrl ?? string.Empty;
            return View();
        }
        
        public ActionResult Logout(int Status = 0)
        {
            HttpCookie cook = Request.Cookies["yunxiao_erp_user"];
            if (cook != null)
            {
                cook["status"] = "0";
                Response.Cookies.Add(cook);
            }

            Session["ClientManager"] = null;
            return Redirect("/Home/Login?Status=" + Status);
        }

        public ActionResult Terms() 
        {
            return View();
        }

        public ActionResult MDLogin(string ReturnUrl)
        {
            if(string.IsNullOrEmpty(ReturnUrl))
            return Redirect(OauthBusiness.GetAuthorizeUrl());
            else
                return Redirect(OauthBusiness.GetAuthorizeUrl() + "&state=" + ReturnUrl);
        }

        public ActionResult MDCallBack(string code, string state)
        {
            string operateip = Common.Common.GetRequestIP();
            var user = OauthBusiness.GetMDUser(code);
            if (user.error_code <= 0)
            {
                var model = OrganizationBusiness.GetUserByMDUserID(user.user.id, user.user.project.id, operateip);
                //已注册云销账户
                if (model != null)
                {
                    //未注销
                    if (model.Status.Value != 9)
                    {
                        model.MDToken = user.user.token;
                        if (string.IsNullOrEmpty(model.Avatar)) model.Avatar = user.user.avatar;

                        Session["ClientManager"] = model;
                        if (string.IsNullOrEmpty(state))
                        {
                            return Redirect("/Default/Index");
                        }
                        else
                        {
                            return Redirect(state);
                        }
                    }
                }
                else
                {
                    int error = 0;
                    bool isAdmin = MD.SDK.Entity.App.AppBusiness.IsAppAdmin(user.user.token, user.user.id, out error);
                    if (isAdmin)
                    {
                        bool bl = AgentsBusiness.IsExistsMDProject(user.user.project.id);
                        //明道网络未注册
                        if (!bl)
                        {
                            int result = 0;
                            Clients clientModel = new Clients();
                            clientModel.CompanyName = user.user.project.name;
                            clientModel.ContactName = user.user.name;
                            clientModel.MobilePhone = user.user.mobile_phone;
                            var clientid = ClientBusiness.InsertClient(clientModel, "", "", "", out result, user.user.email, user.user.id, user.user.project.id);
                            if (!string.IsNullOrEmpty(clientid))
                            {
                                var current = OrganizationBusiness.GetUserByMDUserID(user.user.id, user.user.project.id, operateip);

                                current.MDToken = user.user.token;
                                if (string.IsNullOrEmpty(current.Avatar)) current.Avatar = user.user.avatar;
                                Session["ClientManager"] = current;

                                if (string.IsNullOrEmpty(state))
                                {
                                    return Redirect("/Default/Index");
                                }
                                else
                                {
                                    return Redirect(state);
                                }
                            }

                        }
                        else
                        {
                            int result = 0;
                            var newuser = OrganizationBusiness.CreateUser("", "", user.user.name, user.user.mobile_phone, user.user.email, "", "", "", "", "", "", "", "", user.user.id, user.user.project.id, 1, "", out result);
                            if (newuser != null)
                            {
                                var current = OrganizationBusiness.GetUserByMDUserID(user.user.id, user.user.project.id, operateip);

                                current.MDToken = user.user.token;
                                if (string.IsNullOrEmpty(current.Avatar)) current.Avatar = user.user.avatar;
                                Session["ClientManager"] = current;

                                if (string.IsNullOrEmpty(state))
                                {
                                    return Redirect("/Default/Index");
                                }
                                else
                                {
                                    return Redirect(state);
                                }
                            }
                        }
                    }
                    else
                    {
                        return Redirect("/Home/Login?Status=1");
                    }
                }
            }
            return Redirect("/Home/Login");
        }

        #region Ajax

        //员工登录
        public JsonResult UserLogin(string userName, string pwd, string remember)
        {
            int result = 0;
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            YXERP.Common.PwdErrorUserEntity pwdErrorUser = null;

            if (Common.Common.CachePwdErrorUsers.ContainsKey(userName)) pwdErrorUser = Common.Common.CachePwdErrorUsers[userName];

            if (pwdErrorUser == null || (pwdErrorUser.ErrorCount < 3 && pwdErrorUser.ForbidTime<DateTime.Now) )
            {
                string operateip = string.IsNullOrEmpty(Request.Headers.Get("X-Real-IP")) ? Request.UserHostAddress : Request.Headers["X-Real-IP"];
                int outResult;
                CloudSalesEntity.Users model = CloudSalesBusiness.OrganizationBusiness.GetUserByUserName(userName, pwd, out outResult, operateip);
                if (model != null)
                {
                    //保持登录状态
                    HttpCookie cook = new HttpCookie("yunxiao_erp_user");
                    cook["username"] = userName;
                    cook["pwd"] = pwd;
                    cook["status"] = remember;
                    cook.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Add(cook);

                    Session["ClientManager"] = model;
                    Common.Common.CachePwdErrorUsers.Remove(userName);
                    result = 1;
                }
                else
                {
                    if (outResult == 3)
                    {
                        if (pwdErrorUser == null)
                            pwdErrorUser = new Common.PwdErrorUserEntity();
                        else
                        {
                            if (pwdErrorUser.ErrorCount > 2)
                                pwdErrorUser.ErrorCount = 0;
                        }

                        pwdErrorUser.ErrorCount += 1;
                        if (pwdErrorUser.ErrorCount > 2)
                        {
                            pwdErrorUser.ForbidTime = DateTime.Now.AddHours(2);
                            result = 2;
                        }
                        else
                        {
                            result = 3;
                            resultObj.Add("errorCount", pwdErrorUser.ErrorCount);
                        }

                        Common.Common.CachePwdErrorUsers[userName] = pwdErrorUser;
                    }

                }
            }
            else
            {
                int forbidTime =(int)(pwdErrorUser.ForbidTime - DateTime.Now).TotalMinutes;
                resultObj.Add("forbidTime", forbidTime);
                result = -1;
            }

           
            resultObj.Add("result",result);

            return new JsonResult
            {
                Data = resultObj,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //账号是否存在
        public JsonResult IsExistLoginName(string loginName)
        {
            bool bl = OrganizationBusiness.IsExistLoginName(loginName);
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            JsonDictionary.Add("Result", bl?1:0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //主动注册客户端
        public JsonResult RegisterClient(string name, string companyName, string loginName, string loginPWD,string code)
        {
            int result = 0;
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

            bool bl = OrganizationBusiness.IsExistLoginName(loginName);
            if (bl){
                result = 2;
            }
            else
            {
                bl = Common.Common.ValidateMobilePhoneCode(loginName, code);
                if (!bl){
                    result = 3;
                }
                else
                {
                    Clients client = new Clients() { CompanyName = companyName, ContactName = name, MobilePhone = loginName };
                    ClientBusiness.InsertClient(client, loginName, loginPWD, string.Empty, out result);

                    if (result == 1)
                    {
                        string operateip = Common.Common.GetRequestIP();
                        int outResult;
                        CloudSalesEntity.Users user = CloudSalesBusiness.OrganizationBusiness.GetUserByUserName(loginName, loginPWD, out outResult, operateip);
                        if (user != null){
                            Session["ClientManager"] = user;
                        }

                        Common.Common.ClearMobilePhoneCode(loginName);
                    }
                    else
                        result = 0;
                }
            }

            JsonDictionary.Add("Result", result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //发送手机验证码
        public JsonResult SendMobileMessage(string mobilePhone)
        {
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            Random rd = new Random();
            int code=rd.Next(100000, 1000000);

            bool flag = Common.MessageSend.SendMessage(mobilePhone, code);
            JsonDictionary.Add("Result",flag?1:0);

            if (flag) 
            {
                Common.Common.SetCodeSession(mobilePhone, code.ToString());

                Common.Common.WriteAlipayLog(mobilePhone + " : " + code.ToString());
                
            }

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //验证手机验证码
        public JsonResult ValidateMobilePhoneCode(string mobilePhone, string code)
        {
            bool bl = Common.Common.ValidateMobilePhoneCode(mobilePhone, code);
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            JsonDictionary.Add("Result", bl ? 1 : 0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //重置用户密码
        public JsonResult UpdateUserPwd(string loginName, string loginPwd, string code)
        {
            int result = 0;
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

            bool bl = OrganizationBusiness.IsExistLoginName(loginName);
            if (bl)
            {
                bl = Common.Common.ValidateMobilePhoneCode(loginName, code);
                if (!bl)
                {
                    result = 3;
                }
                else
                {
                    bl = OrganizationBusiness.UpdateUserAccountPwd(loginName, loginPwd);
                    result = bl ? 1 : 0;

                    if(bl)
                        Common.Common.ClearMobilePhoneCode(loginName);
                }
                
            }
            else
            {
                result = 2;
            }

            JsonDictionary.Add("Result",result);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
