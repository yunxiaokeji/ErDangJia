using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using YunXiaoService;
using IntFactory.Sdk;
using CloudSalesEnum;
using CloudSalesEntity.Manage;
namespace YXApp.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();

        #region view
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            ViewBag.CustomerID = "ca91e1be-1e02-4fa1-97c5-b4d00841d421";
            ViewBag.ClientID = "a89cbb94-e32b-4f99-bab9-2db1d9cff607";
            return View();
        }

        public ActionResult Index()
        {
            return View();
        } 
        #endregion

        #region ajax

        public JsonResult IsExistAccount(int type, string account, string companyID, string name, string customerID, string zngcClientID, int verification)
        {
            bool falg = UserService.IsExistAccount((EnumAccountType)type, account, companyID);
            if (!falg && verification == 1)
            {
                int result = 0;
                string userID = "";
                string yxClientID = UserService.InsertClient(EnumRegisterType.ZNGC, (EnumAccountType)type, account, account, "",
                                                                name, account, "", "", "", "", "", companyID, "", customerID, "",
                                                                out result, out userID);
                Clients clientItem = UserService.GetClientDetail(yxClientID);

                var zngcResult = CustomerBusiness.BaseBusiness.SetCustomerYXinfo("", name, account, zngcClientID, clientItem.AgentID, yxClientID, clientItem.ClientCode);

                JsonDictionary.Add("zngcResult", zngcResult);
                //CustomerBusiness.BaseBusiness.SetCustomerYXinfo(
            }
            JsonDictionary.Add("result",!falg?"1":"0");
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //获取用户在智能工厂基本信息
        public JsonResult GetCustomerBaseInfo(string customerID, string clientID) 
        {
            var item = CustomerBusiness.BaseBusiness.GetCustomerByID(customerID, clientID);
            JsonDictionary.Add("item", item);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //发送手机验证码
        //public JsonResult SendMobileMessage(string mobilePhone)
        //{
        //    Dictionary<string, object> JsonDictionary = new Dictionary<string, object>();
        //    Random rd = new Random();
        //    int code = rd.Next(100000, 1000000);

        //    bool flag = Common.MessageSend.SendMessage(mobilePhone, code);
        //    JsonDictionary.Add("Result", flag ? 1 : 0);

        //    if (flag)
        //    {
        //        Common.Common.SetCodeSession(mobilePhone, code.ToString());

        //        Common.Common.WriteAlipayLog(mobilePhone + " : " + code.ToString());

        //    }

        //    return new JsonResult()
        //    {
        //        Data = JsonDictionary,
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //    };
        //}
        #endregion

    }
}
