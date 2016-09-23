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
using CloudSalesEnum;
using YunXiaoService;

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

        public ActionResult Register(string ReturnUrl = "")
        {
            string loginUrl = "/Default/Index";
            ViewBag.LoginUrl = !string.IsNullOrEmpty(ReturnUrl) ? ReturnUrl.Replace("%26", "&") : loginUrl; 
            return View();
        }

        public ActionResult FindPassword()
        {
            return View();
        }

        public ActionResult Login(string ReturnUrl = "", int Status = 0, string name = "", int BindAccountType = 0)
        {

            if (Session["ClientManager"] != null)
            {
                return Redirect("/Default/Index");
            }

            ViewBag.Status = Status;
            ViewBag.BindAccountType = BindAccountType;
            ViewBag.ReturnUrl = ReturnUrl.Replace("%26", "&") + (string.IsNullOrEmpty(name) ? "" : "%26name=" + name).Replace("&", "%26") ?? string.Empty;
            
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
            ViewBag.IsMobileDevice=YXERP.Common.Common.IsMobileDevice();

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
            if (Status > 0)
            {
                return Redirect("/Home/Login?Status=" + Status);
            }
            return Redirect("/Home/Login");
        }
        public JsonResult GetSign(string ReturnUrl)
        {
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            resultObj.Add("sign", YXERP.Common.Signature.GetSignature(Common.Common.YXAgentID, Common.Common.YXClientID, ReturnUrl));

            return new JsonResult
            {
                Data = resultObj,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public ActionResult Authorize(string sign, string ReturnUrl)
        {
            if (!string.IsNullOrEmpty(sign) && !string.IsNullOrEmpty(ReturnUrl))
            {
                if (sign.Equals(YXERP.Common.Signature.GetSignature(Common.Common.YXAgentID, Common.Common.YXClientID, ReturnUrl), StringComparison.OrdinalIgnoreCase))
                {
                    return View("Login");
                }
            }

            Response.Write("<script>alert('参数有误');location.href='http://edj.yunxiaokeji.com';</script>");
            Response.End();
            return View("Login");
        }

        public ActionResult Terms() 
        {
            return View();
        }

        public ActionResult MDLogin(string ReturnUrl)
        {
            if (string.IsNullOrEmpty(ReturnUrl))
            {
                return Redirect(OauthBusiness.GetAuthorizeUrl());
            }
            else
            {
                return Redirect(OauthBusiness.GetAuthorizeUrl() + "&state=" + ReturnUrl);
            }
        }

        public ActionResult MDCallBack(string code, string state)
        {
            string operateip = Common.Common.GetRequestIP();
            var user = OauthBusiness.GetMDUser(code);
            if (user.error_code <= 0)
            {
                var model = OrganizationBusiness.GetUserByOtherAccount(user.user.id, user.user.project.id, operateip, (int)EnumAccountType.MingDao);
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
                            string userid = "";
                            Clients clientModel = new Clients();
                            clientModel.CompanyName = user.user.project.name;
                            clientModel.ContactName = user.user.name;
                            clientModel.MobilePhone = user.user.mobile_phone;
                            var clientid = ClientBusiness.InsertClient(EnumRegisterType.MingDao, EnumAccountType.MingDao, user.user.id, "", user.user.project.name, user.user.name,
                                                                       user.user.mobile_phone, user.user.email, "", "", "", "", user.user.project.id, "", "", "", out result, out userid);
                            if (!string.IsNullOrEmpty(clientid))
                            {
                                var current = OrganizationBusiness.GetUserByOtherAccount(user.user.id, user.user.project.id, operateip, (int)EnumAccountType.MingDao);

                                current.MDToken = user.user.token;
                                if (string.IsNullOrEmpty(current.Avatar))
                                {
                                    current.Avatar = user.user.avatar;
                                }
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
                            var newuser = OrganizationBusiness.CreateUser(EnumAccountType.MingDao, user.user.id, "", user.user.name, user.user.mobile_phone, user.user.email, "", "", "", "", "", "", "", "", user.user.project.id, 1, "", out result);
                            if (newuser != null)
                            {
                                var current = OrganizationBusiness.GetUserByOtherAccount(user.user.id, user.user.project.id, operateip, (int)EnumAccountType.MingDao);

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

        //微信账户选择进入方式
        public ActionResult WeiXinSelectLogin()
        {
            if (Session["WeiXinTokenInfo"] == null)
            {
                return Redirect("/Home/Login");
            }

            return View();
        }

        //微信授权地址
        public ActionResult WeiXinLogin(string ReturnUrl)
        {
            return Redirect(WeiXin.Sdk.Token.GetAuthorizeUrl(Server.UrlEncode(WeiXin.Sdk.AppConfig.CallBackUrl), ReturnUrl, YXERP.Common.Common.IsMobileDevice()));
        }

        //微信回调地址
        public ActionResult WeiXinCallBack(string code, string state)
        {
            string operateip = Common.Common.GetRequestIP();
            var userToken = WeiXin.Sdk.Token.GetAccessToken(code);

            if (string.IsNullOrEmpty(userToken.errcode))
            {
                var model = OrganizationBusiness.GetUserByOtherAccount(userToken.unionid, "", operateip, (int)EnumAccountType.WeiXin); 
                //已注册
                if (model != null)
                {
                    //未注销
                    if (model.Status.Value == 1)
                    {
                        Session["ClientManager"] = model; 
                        if (string.IsNullOrEmpty(state))
                        {
                            return Redirect("/Home/Index");
                        }
                        else
                        {
                            return Redirect(state);
                        }
                    }
                    else
                    {
                        if (model.Status.Value == 9)
                        {
                            Response.Write("<script>alert('您的账户已注销,请切换其他账户登录');location.href='/Home/login';</script>");
                            Response.End();
                        }
                        else
                        {
                            return Redirect("/Home/Login");
                        }

                    }
                }
                else
                {
                    Response.Write("<script>alert('您的微信账号暂未绑定用户,请使用其他方式登陆.');location.href='/Home/login';</script>");
                    Response.End(); 
                }
            }

            return Redirect("/Home/Login");
        }

        public ActionResult CMLogin(string ReturnUrl = "")
        {
            if (Session["ClientManager"] != null)
            {
                return Redirect("/Default/Index");
            }
            return Redirect(IntFactory.Sdk.OauthBusiness.GetAuthorize(ReturnUrl));
        }

        public ActionResult CMCallBack(string sign, string userid,string clientid)
        {
            if (!IntFactory.Sdk.OauthBusiness.GetSign(userid).Equals(sign, StringComparison.OrdinalIgnoreCase))
            {
                return Redirect("/Home/Login");
            }
            string operateip = Common.Common.GetRequestIP();
            var client = IntFactory.Sdk.ClientBusiness.BaseBusiness.GetClientInfo(clientid);
            if (client.error_code <= 0)
            {
                var model = OrganizationBusiness.GetUserByOtherAccount(userid, clientid, operateip, (int)EnumAccountType.ZNGC);
                //已注册云销账户
                if (model != null)
                {
                    //未注销
                    if (model.Status.Value != 9)
                    {
                        Session["ClientManager"] = model; 
                        return Redirect("/Default/Index");
                    }
                }
                else
                {
                    int result = 0;
                    var user = IntFactory.Sdk.UserBusiness.GetUserByUserID(userid, clientid);
                    bool bl = AgentsBusiness.IsExistsCMClient(clientid);
                    if (bl)
                    {
                        var newuser = OrganizationBusiness.CreateUser(EnumAccountType.ZNGC, userid, "", user.user.name, user.user.mobilePhone, "", "", "", "", "", "", "", "", "", clientid, user.user.isSystemAdmin ? 1 : 0, "", out result);
                        if (newuser != null)
                        {
                            var current = OrganizationBusiness.GetUserByOtherAccount(userid, clientid, operateip, (int)EnumAccountType.ZNGC);
                            Session["ClientManager"] = current;
                            return Redirect("/Default/Index");
                        }
                    }
                    else
                    {
                        if (user.user.isSystemAdmin)
                        {
                            Users tempUser = new Users();
                            tempUser.MobilePhone = user.user.mobilePhone;
                            tempUser.Name = user.user.name;
                            tempUser.MDUserID = userid;
                            tempUser.MDProjectID = clientid;
                            tempUser.Client = new Clients();
                            tempUser.Client.CompanyName = client.client.companyName;
                            tempUser.Client.Logo = client.client.logo;
                            tempUser.Client.ContactName = client.client.contactName;
                            tempUser.Client.Address = client.client.address;
                            tempUser.Client.MobilePhone = client.client.mobilePhone;
                            tempUser.Client.CityCode = client.client.cityCode;
                            Session["CMTokenInfo"] = tempUser;

                            return Redirect("/Home/CMSelectLogin");
                        }
                        else
                        {
                            return Redirect("/Home/Login?Status=4");
                        }
                    }
                }
            }
            return Redirect("/Home/Login");
        }

        public ActionResult CMSelectLogin()
        {
            if (Session["CMTokenInfo"] == null)
            {
                return Redirect("/Home/Login");
            }
            var user = (Users)Session["CMTokenInfo"];
            ViewBag.Name = user.Name;
            ViewBag.CompanyName = user.Client.CompanyName;
            ViewBag.Logo = user.Client.Logo;
            return View();
        }

        public ActionResult CMRegister()
        {
            
            if (Session["CMTokenInfo"] == null)
            {
                return Redirect("/Home/Login");
            }
            int result = 0;
            string userid = "";
            string operateip = Common.Common.GetRequestIP();
            var user = (Users)Session["CMTokenInfo"];
            var tempclientid = ClientBusiness.InsertClient(EnumRegisterType.ZNGC, EnumAccountType.ZNGC, user.MDUserID, "", user.Client.CompanyName, user.Name,
                                                                        user.MobilePhone, "", "", user.Client.CityCode, user.Client.Address, "", user.MDProjectID, "", "", "", out result, out userid);
            if (!string.IsNullOrEmpty(tempclientid))
            {
                var current = OrganizationBusiness.GetUserByOtherAccount(user.MDUserID, user.MDProjectID, operateip, (int)EnumAccountType.ZNGC);

                Session["ClientManager"] = current;
                Session["CMTokenInfo"] = null;
                return Redirect("/Default/Index");
            }
            return Redirect("/Home/Login");
        }

        #region Ajax

        //员工登录
        public JsonResult UserLogin(string userName, string pwd, string remember, int bindAccountType)
        {
            int result = 0;
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            YXERP.Common.PwdErrorUserEntity pwdErrorUser = null;

            if (Common.Common.CachePwdErrorUsers.ContainsKey(userName)) pwdErrorUser = Common.Common.CachePwdErrorUsers[userName];

            if (pwdErrorUser == null || (pwdErrorUser.ErrorCount < 3 && pwdErrorUser.ForbidTime<DateTime.Now) )
            {
                result = 1;
                string operateip = string.IsNullOrEmpty(Request.Headers.Get("X-Real-IP")) ? Request.UserHostAddress : Request.Headers["X-Real-IP"];
                int outResult;
                CloudSalesEntity.Users model = CloudSalesBusiness.OrganizationBusiness.GetUserByUserName(userName, pwd, out outResult, operateip);
                if (model != null)
                {
                    if (bindAccountType == 4)
                    {
                        string errinfo = BindCMClient(model);
                        if (!string.IsNullOrEmpty(errinfo))
                        {
                            result = 4;
                            resultObj.Add("errinfo", errinfo);
                        }
                        else
                        {
                            Session["ClientManager"] = model;
                        }
                    }
                    else
                    {
                        //保持登录状态
                        HttpCookie cook = new HttpCookie("yunxiao_erp_user");
                        cook["username"] = userName;
                        cook["pwd"] = pwd;
                        cook["status"] = remember;
                        cook.Expires = DateTime.Now.AddDays(7);
                        Response.Cookies.Add(cook);
                        Session["ClientManager"] = model;
                    }
                    Common.Common.CachePwdErrorUsers.Remove(userName);
                }
                else
                {
                    if (outResult == 3)
                    {
                        if (pwdErrorUser == null)
                        {
                            pwdErrorUser = new Common.PwdErrorUserEntity();
                        }
                        else
                        {
                            if (pwdErrorUser.ErrorCount > 2)
                            {
                                pwdErrorUser.ErrorCount = 0;
                            }
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

        public string BindCMClient(Users model)
        {
            if (Session["CMTokenInfo"] != null)
            {
                var user = (Users)Session["CMTokenInfo"];

                return OrganizationBusiness.BindCMClient(model.UserID, user.MDUserID, user.MDProjectID, model.AgentID, model.ClientID,
                    user.Client.CompanyName, user.Client.ContactName, user.Client.MobilePhone, user.Client.CityCode, user.Client.Address);
            }
            return "";
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
        public JsonResult RegisterClient(string name, string companyName, string loginName, string loginPWD, string code, int regType)
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
                if (!bl)
                {
                    result = 3;
                }
                else
                {
                    string userid = "";
                    //自助注册 
                    var clientid = ClientBusiness.InsertClient((EnumRegisterType)Enum.Parse(typeof(EnumRegisterType), regType.ToString()), EnumAccountType.Mobile, loginName, loginPWD, companyName, name, loginName, "", "", "", "", "", "", "", "", "", out result, out userid);
                    
                    if (result == 1)
                    {
                        string operateip = Common.Common.GetRequestIP();
                        int outResult;
                        CloudSalesEntity.Users user = CloudSalesBusiness.OrganizationBusiness.GetUserByUserName(loginName, loginPWD, out outResult, operateip);
                        if (user != null)
                        {
                            Session["ClientManager"] = user;
                        }

                        Common.Common.ClearMobilePhoneCode(loginName);
                    }
                    else
                    {
                        result = 0;
                    }
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

                    if (bl)
                    {
                        Common.Common.CachePwdErrorUsers.Remove(loginName);
                        Common.Common.ClearMobilePhoneCode(loginName);
                    }
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
