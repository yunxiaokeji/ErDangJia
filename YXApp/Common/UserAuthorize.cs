using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesEntity;
using CloudSalesEnum;

namespace YXAPP.Common
{
    public class UserAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["ClientManager"] == null)
            {
                httpContext.Response.StatusCode = 401;
                return false;
            }
            else
            {
                //if (user.LogGUID != OrganizationBusiness.GetUserByUserID(user.UserID, user.AgentID).LogGUID)
                //{
                //    httpContext.Response.StatusCode = 402;
                //    return false;
                //}
            }
            return true;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.HttpContext.Response.StatusCode == 401)
            {
                string source = HttpContext.Current.Request.QueryString["source"];
                if (!string.IsNullOrEmpty(source) && source == "md")
                {
                    filterContext.Result = new RedirectResult("/Home/MDLogin?ReturnUrl=" + HttpContext.Current.Request.Url);
                }
                else 
                {
                    filterContext.Result = new RedirectResult("/Home/Login?ReturnUrl=" + HttpContext.Current.Request.Url);
                }
                return;
            }
            //else if (filterContext.HttpContext.Response.StatusCode == 402)
            //{
            //    filterContext.Result = new RedirectResult("/Home/Logout?Status=" + (int)EnumLoginStatus.OtherLogin);
            //}

        }
    }
}