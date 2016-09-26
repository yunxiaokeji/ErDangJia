using CloudSalesBusiness;
using CloudSalesEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YXERP.Areas.M.Controllers
{
    public class AccountController : YXERP.Controllers.BaseController
    {
        //
        // GET: /M/Account/

        public ActionResult Index()
        {
            ViewBag.providerID = CurrentUser.CurrentCMClientID; ;
            ViewBag.baseUser = CurrentUser;
            ViewBag.index = 2;

            var UserAccounts = OrganizationBusiness.GetUserAccounts(CurrentUser.UserID, CurrentUser.ClientID);
            var WeiXinID = string.Empty;
            var UserAccount = UserAccounts.Find(m => m.AccountType == (int)EnumAccountType.WeiXin);
            if (UserAccount != null)
            {
                WeiXinID = UserAccount.AccountName;
            }
            ViewBag.WeiXinID = WeiXinID;

            return View();
        }

        //微信授权地址
        public ActionResult WeiXinLogin()
        {
            string port = HttpContext.Request.Url.Port.ToString();
            string domain = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Host +
                          (string.IsNullOrEmpty(port) || port == "80" ? "" : ":" + port);

            string returnUrl = domain + "/m/Account/index";
            string callBackUrl = domain + "/MyAccount/WeiXinCallBack";

            return Redirect(WeiXin.Sdk.Token.GetAuthorizeUrl(Server.UrlEncode(callBackUrl), returnUrl, YXERP.Common.Common.IsMobileDevice()));
        }

    }
}
