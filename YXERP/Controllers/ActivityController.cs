using CloudSalesBusiness;
using CloudSalesEntity;
using CloudSalesEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace YXERP.Controllers
{

    public class ActivityController : BaseController
    {
        #region view
        public ActionResult Index()
        {
            return Redirect("/Activity/MyActivity");
        }

        public ActionResult MyActivity()
        {
            ViewBag.Title = "我的活动";
            ViewBag.Option = 0;
            return View();
        }

        public ActionResult Activitys()
        {
            ViewBag.Title = "所有活动";
            ViewBag.Option = 1;
            return View("MyActivity");
        }

        public ActionResult Create()
        {
            ViewBag.ActivityID = "";
            return View();
        }

        public ActionResult Operate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect("/Activity/MyActivity");
            }
            ViewBag.ActivityID = id;
            return View("Create");
        }

        public ActionResult Detail(string id)
        {
            ViewBag.ActivityID = id ?? string.Empty;
            ViewBag.MDUserID = CurrentUser.MDToken;
            return View();
        }
        #endregion

        #region ajax

        public JsonResult GetActivityList(string keyWords, int pageSize, int pageIndex, int isAll, string beginTime, string endTime, string orderBy, int stage, int filterType, string userID)
        {
            int pageCount = 0;
            int totalCount = 0;
            string ownerID=CurrentUser.UserID;
            if (isAll == 1)
            {
                if (!string.IsNullOrEmpty(userID))
                    ownerID = userID;
                else
                    ownerID = string.Empty;

            }

            List<ActivityEntity> list = ActivityBusiness.GetActivitys(ownerID, (EnumActivityStage)stage, filterType, keyWords, beginTime, endTime, orderBy, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetActivityDetail(string activityID)
        {
            ActivityEntity model = ActivityBusiness.GetActivityByID(activityID, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("Item", model);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetUserDetail(string id)
        {
            Users model = null;
            if(!string.IsNullOrEmpty(id))
             model = OrganizationBusiness.GetUserByUserID(id, CurrentUser.AgentID);
            else
                model = OrganizationBusiness.GetUserByUserID(CurrentUser.UserID, CurrentUser.AgentID);
            JsonDictionary.Add("Item", model);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomersByActivityID(string activityID,int pageSize,int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;

            List<CustomerEntity> list = CustomBusiness.BaseBusiness.GetCustomersByActivityID(activityID, pageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SavaActivity(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ActivityEntity model = serializer.Deserialize<ActivityEntity>(entity);

            string activityID = "";
            model.OwnerID = model.OwnerID.Trim('|');
            model.MemberID = model.MemberID.Trim('|');
            //新增
            if (string.IsNullOrEmpty(model.ActivityID))
            {
                activityID =ActivityBusiness.CreateActivity(model.Name, model.Poster, model.BeginTime.ToString(), model.EndTime.ToString(), model.Address, model.OwnerID,model.MemberID, model.Remark, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }//编辑
            else
            {
                bool bl =ActivityBusiness.UpdateActivity(model.ActivityID, model.Name,model.Poster, model.BeginTime.ToString(), model.EndTime.ToString(), model.Address, model.Remark,model.OwnerID,model.MemberID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);

                if (bl)
                {
                    activityID = model.ActivityID;
                }
            }

            JsonDictionary.Add("ID", activityID);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteActivity(string activityID)
        {
            bool bl = new ActivityBusiness().DeleteActivity(activityID);
            JsonDictionary.Add("Result", bl?1:0);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

    }
}
