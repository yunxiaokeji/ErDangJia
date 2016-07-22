using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using YunXiaoService;
using IntFactory.Sdk;
using CloudSalesEnum;
using CloudSalesEntity.Manage;
using IntFactory.Sdk.Business;
using CloudSalesEntity;
using YXAPP.Common;

namespace YXApp.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

        #region view
        public ActionResult Login(string ReturnUrl, string clientID = "")
        {
            ReturnUrl = ReturnUrl ?? string.Empty;
            ViewBag.ReturnUrl = ReturnUrl;
            ViewBag.zngcClientID = clientID;
            return View();
        }

        public ActionResult Register()
        {
            ViewBag.CustomerID = "ca91e1be-1e02-4fa1-97c5-b4d00841d421";
            ViewBag.ClientID = "e3aa5f69-0362-450d-a6f2-a9a055c11d59";
            return View();
        }

        public ActionResult Index()
        { 
            return View();
        } 
        #endregion

        #region ajax

        public JsonResult IsExistAccount(int type, string account, string companyID)
        {
            bool falg = UserService.IsExistAccount((EnumAccountType)type, account, companyID);
            JsonDictionary.Add("result",!falg?"1":"0");
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //注册
        public JsonResult RegisterClient(int type, string account, string companyID, string name, string customerID, string zngcClientID,string code)
        {
            int result = 0;
            bool falg = UserService.IsExistAccount((EnumAccountType)type, account, companyID);
            if (falg)
            {
                result = 2;
            }
            else
            {
                falg = Common.ValidateMobilePhoneCode(account, code);
                if (!falg)
                {
                    result = 3;
                }
                else
                {
                    int status = 0;
                    string userID = "";
                    string yxClientID = UserService.InsertClient(EnumRegisterType.ZNGC, (EnumAccountType)type, account, account, name,
                                                                    name, account, "", "", "", "", "", zngcClientID, "", customerID, "",
                                                                    out status, out userID);
                    if (status == 1)
                    {
                        result = 1;
                        ClientResult zngcClientItem = ClientBusiness.BaseBusiness.GetClientInfo(zngcClientID);
                        Clients yxClientItem = UserService.GetClientDetail(yxClientID);

                        string providerID = ProductService.AddProviders(zngcClientItem.client.companyName, zngcClientItem.client.contactName,
                                                    zngcClientItem.client.mobilePhone, "", "", zngcClientItem.client.address,
                                                    "", zngcClientID, zngcClientItem.client.clientCode, "", yxClientItem.AgentID, yxClientID);
                        var zngcResult = CustomerBusiness.BaseBusiness.SetCustomerYXinfo(customerID, name, account, zngcClientID,
                                                    yxClientItem.AgentID, yxClientID, yxClientItem.ClientCode);
                        
                        string operateip = Common.GetRequestIP();
                        int outResult;
                        CloudSalesEntity.Users user = UserService.GetUserByAccount((EnumAccountType)type, account, account, companyID, operateip, out outResult);
                        if (user != null)
                        {
                            Session["ClientManager"] = user;
                        }
                        Common.ClearMobilePhoneCode(account);
                    }
                    JsonDictionary.Add("status", status);
                }
            }
            JsonDictionary.Add("zngcClientID", zngcClientID);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //登陆
        public JsonResult UserLogin(int type, string userName, string pwd, string companyID, string zngcClientID)
        {
            int result = 0;
            Dictionary<string, object> resultObj = new Dictionary<string, object>();
            YXAPP.Common.PwdErrorUserEntity pwdErrorUser = null;

            if (Common.CachePwdErrorUsers.ContainsKey(userName)) pwdErrorUser = Common.CachePwdErrorUsers[userName];

            if (pwdErrorUser == null || (pwdErrorUser.ErrorCount < 3 && pwdErrorUser.ForbidTime < DateTime.Now))
            {
                string operateip = string.IsNullOrEmpty(Request.Headers.Get("X-Real-IP")) ? Request.UserHostAddress : Request.Headers["X-Real-IP"];
                int outResult;
                CloudSalesEntity.Users model = UserService.GetUserByAccount((EnumAccountType)type, userName, pwd, "", "", out outResult);
                if (model != null)
                {
                    //添加供应商
                    if (!string.IsNullOrEmpty(zngcClientID))
                    {
                        ClientResult zngcClientItem = ClientBusiness.BaseBusiness.GetClientInfo(zngcClientID);
                        Clients yxClientItem = UserService.GetClientDetail(model.ClientID);

                        string providerID = ProductService.AddProviders(zngcClientItem.client.companyName, zngcClientItem.client.contactName,
                                                    zngcClientItem.client.mobilePhone, "", "", zngcClientItem.client.address,
                                                    "", zngcClientID, zngcClientItem.client.clientCode, "", yxClientItem.AgentID, model.ClientID);
                    }

                    //保持登录状态
                    HttpCookie cook = new HttpCookie("yunxiao_erp_user");
                    cook["username"] = userName;
                    cook["pwd"] = pwd;
                    cook.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Add(cook);

                    Session["ClientManager"] = model;
                    Common.CachePwdErrorUsers.Remove(userName);
                    result = 1;
                }
                else
                {
                    if (outResult == 3)
                    {
                        if (pwdErrorUser == null)
                            pwdErrorUser = new YXAPP.Common.PwdErrorUserEntity();
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
                        Common.CachePwdErrorUsers[userName] = pwdErrorUser;
                    }
                }
            }
            else
            {
                int forbidTime = (int)(pwdErrorUser.ForbidTime - DateTime.Now).TotalMinutes;
                resultObj.Add("forbidTime", forbidTime);
                result = -1;
            }


            resultObj.Add("result", result);

            return new JsonResult
            {
                Data = resultObj,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取用户、智能工厂基本信息
        public JsonResult GetCustomerBaseInfo(string customerID, string clientID)
        {
            var userItem = CustomerBusiness.BaseBusiness.GetCustomerByID(customerID, clientID);
            var clientItem = ClientBusiness.BaseBusiness.GetClientInfo(clientID);
            JsonDictionary.Add("item", userItem);
            JsonDictionary.Add("clientName", clientItem.client.companyName);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //发送手机验证码
        public JsonResult SendMobileMessage(string mobilePhone)
        {
            Random rd = new Random();
            int code = rd.Next(100000, 1000000);

            //bool flag = YXAPP.Common.MessageSend.SendMessage(mobilePhone, code);
            bool flag = true;
            JsonDictionary.Add("Result", flag ? 1 : 0);
            if (flag)
            {
                YXAPP.Common.Common.SetCodeSession(mobilePhone, code.ToString());

                YXAPP.Common.Common.WriteAlipayLog(mobilePhone + " : " + code.ToString());
            }
            JsonDictionary.Add("code", code);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //验证手机验证码
        public JsonResult ValidateMobilePhoneCode(string mobilePhone, string code)
        {
            bool bl = YXAPP.Common.Common.ValidateMobilePhoneCode(mobilePhone, code);
            Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
            JsonDictionary.Add("Result", bl ? 1 : 0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        #endregion

    }
}
