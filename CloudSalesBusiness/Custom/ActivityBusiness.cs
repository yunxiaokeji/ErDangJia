using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CloudSalesDAL;
using CloudSalesEntity;
using System.Data;
using CloudSalesEnum;

namespace CloudSalesBusiness
{
    public class ActivityBusiness
    {
        public static  string FilePath = CloudSalesTool.AppSettings.Settings["UploadFilePath"];
        public static string TempPath = CloudSalesTool.AppSettings.Settings["UploadTempPath"];

        #region 查询

        public static List<ActivityEntity> GetActivitys(string userid, EnumActivityStage stage, int filterType, string keyWords, string beginTime, string endTime, string orderBy, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            List<ActivityEntity> list = new List<ActivityEntity>();
            DataTable dt = ActivityDAL.BaseProvider.GetActivitys(userid, (int)stage, filterType, keyWords, beginTime, endTime, orderBy, pageSize, pageIndex, ref totalCount, ref pageCount, agentid, clientid);
            foreach (DataRow dr in dt.Rows)
            {
                ActivityEntity model = new ActivityEntity();
                model.FillData(dr);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.Members = new List<Users>();
                foreach (var id in model.MemberID.Split('|'))
                {
                    model.Members.Add(OrganizationBusiness.GetUserByUserID(id, model.AgentID));
                }
                list.Add(model);
            }
            return list;

        }

        public static ActivityEntity GetActivityByID(string activityid, string agentid, string clientid)
        {
            ActivityEntity model = new ActivityEntity();
            DataTable dt = ActivityDAL.BaseProvider.GetActivityByID(activityid, agentid, clientid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);

                model.Owner= OrganizationBusiness.GetUserByUserID(model.OwnerID,model.AgentID);
                model.Members = new List<Users>();
                foreach (var id in model.MemberID.Split('|')) {
                    model.Members.Add(OrganizationBusiness.GetUserByUserID(id, model.AgentID));
                }

            }
            return model;
        }

        public static ActivityEntity GetActivityBaseInfoByID(string activityid, string agentid, string clientid)
        {
            ActivityEntity model = new ActivityEntity();
            DataTable dt = ActivityDAL.BaseProvider.GetActivityByID(activityid, agentid, clientid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }
            return model;
        }

        public static ActivityEntity GetActivityByCode(string activitycode, string agentid, string clientid)
        {
            ActivityEntity model = new ActivityEntity();
            DataTable dt = ActivityDAL.BaseProvider.GetActivityByCode(activitycode, agentid, clientid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }
            return model;
        }

        #endregion

        #region 添加

        public static string CreateActivity(string name, string poster, string begintime, string endtime, string address, string ownerid, string memberid, string remark, string userid, string agentid, string clientid)
        {
            string activityid = Guid.NewGuid().ToString();

            //if (!string.IsNullOrEmpty(poster))
            //{
            //    if (poster.IndexOf("?") > 0)
            //    {
            //        poster = poster.Substring(0, poster.IndexOf("?"));
            //    }
            //    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(poster));
            //    poster = FilePath + file.Name;
            //    if (file.Exists)
            //    {
            //        file.MoveTo(HttpContext.Current.Server.MapPath(poster));
            //    }
            //}
            bool bl = ActivityDAL.BaseProvider.CreateActivity(activityid, name, poster, begintime, endtime, address, ownerid,memberid, remark, userid, agentid, clientid);
            if (!bl)
            {
                return "";
            }
            else
            {
                //日志
                LogBusiness.AddActionLog(CloudSalesEnum.EnumSystemType.Client, CloudSalesEnum.EnumLogObjectType.Activity, EnumLogType.Create, "", userid, agentid, clientid);
            }
            return activityid;
        }

        #endregion

        #region 编辑/删除

        public static bool UpdateActivity(string activityid, string name, string poster, string begintime, string endtime, string address, string remark, string ownerid, string memberid, string userid, string agentid, string clientid)
        {
            if (!string.IsNullOrEmpty(poster) && poster.IndexOf(TempPath) >= 0)
            {
                if (poster.IndexOf("?") > 0)
                {
                    poster = poster.Substring(0, poster.IndexOf("?"));
                }
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(poster));
                poster = FilePath + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(poster));
                }
            }
            bool bl = ActivityDAL.BaseProvider.UpdateActivity(activityid, name, poster, begintime, endtime, address, remark,ownerid,memberid);
            return bl;   
        }

        public bool DeleteActivity(string activityid){
            bool bl = ActivityDAL.BaseProvider.DeleteActivity(activityid);
            return bl;   
        }

        #endregion
    }
}
