using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesBusiness.Manage;
using System.Web.Script.Serialization;
using CloudSalesEntity;
using CloudSalesEntity.Manage;
namespace YXManage.Controllers
{
     [YXManage.Common.UserAuthorize]
    public class SystemController : BaseController
    {
        #region view
         public ActionResult Admin()
        {
            return View();
        }
         #region ExpressView
         public ActionResult ExpressIndex()
         {
             return View();
         }

         public ActionResult ExpressDetail(string id)
         {
             ViewBag.ExpressID = id;

             return View();
         }
         #endregion
        #endregion

        #region ajax
        public JsonResult GetAdminDetail()
        {
            JsonDictionary.Add("LoginName", CurrentUser.LoginName);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ConfirmAdminPwd(string pwd)
        {
            CloudSalesEntity.Manage.M_Users model = CloudSalesBusiness.M_UsersBusiness.GetM_UserByUserName(CurrentUser.LoginName, pwd, string.Empty);
            JsonDictionary.Add("Result", model!=null?1:0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SetAdminAccount(string loginName, string newPwd)
        {
            bool flag = CloudSalesBusiness.M_UsersBusiness.SetAdminAccount(CurrentUser.UserID, loginName, newPwd);
            
            JsonDictionary.Add("Result", flag ? 1 : 0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #region ExpressAjax
        /// <summary>
        /// 快递公司列表
        /// </summary>
        public JsonResult GetExpressCompanys(int pageIndex, string keyWords)
        {
            int totalCount = 0, pageCount = 0;
            var list = ExpressCompanyBusiness.GetExpressCompanys(keyWords, PageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 快递公司详情
        /// </summary>
        public JsonResult GetExpressCompanyDetail(string id)
        {
            var item = ExpressCompanyBusiness.GetExpressCompanyDetail(id);
            JsonDictionary.Add("Item", item);
            JsonDictionary.Add("Result", 1);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存快递公司
        /// </summary>
        public JsonResult SaveExpressCompany(string expressCompany)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ExpressCompany model = serializer.Deserialize<ExpressCompany>(expressCompany);

            bool flag = false;
            if (model.AutoID == 0)
            {
                model.CreateUserID = string.Empty;
                flag = ExpressCompanyBusiness.InsertExpressCompany(model);
            }
            else
            {
                model.CreateUserID = string.Empty;
                flag = ExpressCompanyBusiness.UpdateExpressCompany(model);
            }
            JsonDictionary.Add("Result", flag ? 1 : 0);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除快递公司
        /// </summary>
        public JsonResult DeleteExpressCompany(string id)
        {
            bool flag = ExpressCompanyBusiness.DeleteExpressCompany(id);
            JsonDictionary.Add("Result", flag ? 1 : 0);
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
