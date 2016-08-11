using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CloudSalesBusiness;
using System.IO;
using CloudSalesEnum;
using System.Web.Script.Serialization;
using CloudSalesEntity;
using Qiniu.Conf;
using Qiniu.IO;
using Qiniu.RPC;
using Qiniu.RS;

namespace YXERP.Controllers
{
    public class PlugController : BaseController
    {
        /// <summary>
        /// 根据cityCode获取下级地区列表
        /// </summary>
        /// <param name="pcode"></param>
        /// <returns></returns>
        public JsonResult GetCityByPCode(string cityCode)
        {
            var list = CommonBusiness.Citys.Where(c => c.PCode == cityCode);
            JsonDictionary.Add("Items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult GetToken()
        {
            return new JsonResult()
            {
                Data = Common.Common.GetQNToken(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public int DeleteAttachment(string key)
        {
            Config.Init(); 
            //实例化一个RSClient对象，用于操作BucketManager里面的方法
            RSClient client = new RSClient();
            CallRet ret = client.Delete(new EntryPath(Common.Common.bucket, key));

            return ret.OK ? 1 : 0;
        }
        /// <summary>
        /// 支持批量上传  
        /// </summary>
        /// <param name="filepath">格式 英文逗号分割 A,B,C </param>
        /// <param name="file">文件夹名车 例如 产品product 订单 orders</param>
        /// <returns>图片地址A,图片地址B,图片地址C</returns>
        public string UploadAttachment(string filepath,string file)
        {
           return  Common.Common.UploadAttachment(filepath, file);
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadFile()
        {
            string oldPath = "",
                   folder = CloudSalesTool.AppSettings.Settings["UploadTempPath"], 
                   action = "";
            if (Request.Form.AllKeys.Contains("oldPath"))
            {
                oldPath = Request.Form["oldPath"];
            }
            if (Request.Form.AllKeys.Contains("folder") && !string.IsNullOrEmpty(Request.Form["folder"]))
            {
                folder = Request.Form["folder"];
            }
            string uploadPath = HttpContext.Server.MapPath(folder);

            if (Request.Form.AllKeys.Contains("action"))
            {
                action = Request.Form["action"];
            }
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            List<string> list = new List<string>();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                //判断图片类型
                string ContentType = file.ContentType;
                Dictionary<string, string> types = new Dictionary<string, string>();
                types.Add("image/x-png", "1");
                types.Add("image/png", "1");
                types.Add("image/gif", "1");
                types.Add("image/jpeg", "1");
                //types.Add("image/tiff", "1");
                types.Add("application/x-MS-bmp", "1");
                types.Add("image/pjpeg", "1");
                if (!types.ContainsKey(ContentType))
                {
                    continue;
                }
                if (file.ContentLength > 1024 * 1024 * 10)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(oldPath) && oldPath != "/modules/images/default.png" && new FileInfo(HttpContext.Server.MapPath(oldPath)).Exists)
                {
                    file.SaveAs(HttpContext.Server.MapPath(oldPath));
                    list.Add(oldPath);
                }
                else 
                {
                    string[] arr = file.FileName.Split('.');
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmssms") + new Random().Next(1000, 9999).ToString() + "." + arr[arr.Length - 1];
                    string filePath = uploadPath + fileName;
                    file.SaveAs(filePath);
                    list.Add(folder + fileName);
                }
            }

            JsonDictionary.Add("Items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取下属列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUserBranchs(string userid, string agentid)
        {
            if (string.IsNullOrEmpty(agentid))
            {
                agentid = CurrentUser.AgentID;
            }
            if (string.IsNullOrEmpty(userid))
            {
                userid = CurrentUser.UserID;
            }
            else if (userid == "-1")
            {
                userid = "6666666666";
            }
            var list = OrganizationBusiness.GetStructureByParentID(userid, agentid);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取团队
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTeams(string agentid)
        {
            if (string.IsNullOrEmpty(agentid))
            {
                agentid = CurrentUser.AgentID;
            }
            var list = SystemBusiness.BaseBusiness.GetTeams(agentid);
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //快递公司
        public JsonResult GetExpress()
        {
            var list = CloudSalesBusiness.Manage.ExpressCompanyBusiness.GetExpressCompanys();
            JsonDictionary.Add("items", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="type"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult GetObjectLogs(string guid, EnumLogObjectType type, int pageSize, int pageIndex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = LogBusiness.GetLogs(guid, type, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取评论备忘
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="type"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult GetReplys(string guid, EnumLogObjectType type, int pageSize, int pageIndex)
        {
            int totalCount = 0;
            int pageCount = 0;

            var list = ReplyBusiness.GetReplys(guid, type, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.AgentID);

            JsonDictionary.Add("items", list);
            JsonDictionary.Add("totalCount", totalCount);
            JsonDictionary.Add("pageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SavaReply(EnumLogObjectType type, string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ReplyEntity model = serializer.Deserialize<ReplyEntity>(entity);

            string replyID = "";
            replyID = ReplyBusiness.CreateReply(type, model.GUID, model.Content, CurrentUser.UserID, CurrentUser.AgentID, model.FromReplyID, model.FromReplyUserID, model.FromReplyAgentID);

            List<ReplyEntity> list = new List<ReplyEntity>();
            if (!string.IsNullOrEmpty(replyID))
            {
                model.ReplyID = replyID;
                model.CreateTime = DateTime.Now;
                model.CreateUser = CurrentUser;
                model.CreateUserID = CurrentUser.UserID;
                model.AgentID = CurrentUser.AgentID;
                if (!string.IsNullOrEmpty(model.FromReplyUserID) && !string.IsNullOrEmpty(model.FromReplyAgentID))
                {
                    model.FromReplyUser = OrganizationBusiness.GetUserByUserID(model.FromReplyUserID, model.FromReplyAgentID);
                }
                list.Add(model);
            }
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
