using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YunXiaoService;

namespace YXERP.Areas.Api.Controllers
{ 
    public class ClientController : BaseAPIController
    {
        //获取客户端信息
        public ActionResult GetClientInfo(string clientID)
        {
            var item = UserService.GetClientDetail(clientID);
            Dictionary<string, object> obj = new Dictionary<string, object>();
            if (item != null)
            {
                if (item.Status!=9)
                {
                    obj.Add("clientID", item.ClientID);
                    obj.Add("clientCode", item.ClientCode);
                    obj.Add("companyName", item.CompanyName);
                    obj.Add("logo", item.Logo);
                    obj.Add("contactName", item.ContactName);
                    obj.Add("mobilePhone", item.MobilePhone);
                    obj.Add("address", item.Address);
                }
                else
                {
                    JsonDictionary["error_code"] = -101;
                    JsonDictionary["error_msg"] = "客户信息已失效";
                }
            }
            else
            {
                JsonDictionary["error_code"] = -100;
                JsonDictionary["error_msg"] = "客户不存在";
            }
            JsonDictionary.Add("result", obj);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public ActionResult GetClientCallback(string clientID,string callBackUrl)
        {
            var item = UserService.GetClientDetail(clientID);
            Dictionary<string, object> obj = new Dictionary<string, object>();
            if (item != null)
            {
                if (item.Status == 9)
                { 
                    JsonDictionary["error_code"] = -101;
                    JsonDictionary["error_msg"] = "客户信息已失效";
                    return Redirect(callBackUrl);
                }
            }
            else
            {
                JsonDictionary["error_code"] = -100;
                JsonDictionary["error_msg"] = "客户不存在";
                return Redirect(callBackUrl);
            }
            JsonDictionary.Add("result", callBackUrl);
            return Redirect(callBackUrl);
        }
    }
}
