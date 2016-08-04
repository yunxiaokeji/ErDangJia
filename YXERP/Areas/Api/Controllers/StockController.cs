using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CloudSalesBusiness;
using CloudSalesEntity;
using YunXiaoService;
using YXERP.Areas.Api.Models;
using YXERP.Models;

namespace YXERP.Areas.Api.Controllers
{
    [YXERP.Common.ApiAuthorize]
    public class StockController : BaseAPIController
    {
        //
        // GET: /Api/Stock/

        #region 仓库信息

        /// <summary>
        /// 获取仓库库位信息
        /// </summary>
        /// <param name="wareid">仓库ID</param>
        /// <param name="agentid">代理商ID</param>
        /// <param name="clientid">公司ID</param>
        /// <returns></returns>
        public ActionResult GetDepotSeatsByWareID(string wareid, string agentid, string clientid)
        {
            List<DepotSeat> list = new List<DepotSeat>();
            if (!string.IsNullOrEmpty(wareid))
            {

                if (!string.IsNullOrEmpty(clientid))
                {
                    list = StockService.GetDepotSeatsByWareID(wareid, agentid, clientid);
                }
                else
                {
                    JsonDictionary["error_code"] = -100;
                    JsonDictionary["error_msg"] = "参数clientid不能为空";
                }
            }
            else
            {
                JsonDictionary["error_code"] = -100;
                JsonDictionary["error_msg"] = "参数wareid不能为空";
            }
            JsonDictionary.Add("result", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取仓库库位信息
        /// </summary>
        /// <param name="wareid">仓库ID</param>
        /// <param name="agentid">代理商ID</param>
        /// <param name="clientid">公司ID</param>
        /// <returns></returns>
        public ActionResult GetAllWareHouses(string agentid, string clientid)
        {
            var list = StockService.GetAllWareHouses(clientid);
            JsonDictionary.Add("result", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            }; 
        }
        /// <summary>
        /// 获取系统所有快递公司
        /// </summary>
        /// <returns></returns>
        public ActionResult GetExpress()
        {
            var list = StockService.GetExpress();
            JsonDictionary.Add("result", list);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 采购订单

        /// <summary>
        /// 分页获取采购单
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="status">单据状态</param>
        /// <param name="keywords">关键字</param>
        /// <param name="begintime">采购单添加时间</param>
        /// <param name="endtime"></param>
        /// <param name="wareid">仓库ID</param>
        /// <param name="providerid">经销商ID</param>
        /// <param name="pageSize">显示条数　默认10</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageCount">总页数</param>
        /// <param name="agentid">代理商ID</param>
        /// <param name="clientid">公司ID</param>
        /// <returns></returns>
        public ActionResult GetPurchases(string userid, int status, string keywords, string begintime, string endtime, string wareid, string providerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            int total = 0;
            int pagecount = 0;
            var list = StockService.GetPurchases(userid, status, keywords, begintime, endtime, wareid, providerid,
                pageSize, pageIndex, ref total, ref pagecount, agentid, clientid);

            JsonDictionary.Add("TotalCount", total);
            JsonDictionary.Add("PageCount", pagecount);
            JsonDictionary.Add("result", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 采购单入库 支持部分入库 分批入库
        /// </summary>
        /// <param name="filterPurchase">详情参考 FilterPurchase</param>
        /// <param name="userid">用户ID</param>
        /// <param name="agentid">代理商ID</param>
        /// <param name="clientid">公司ID</param>
        /// <returns></returns>
        public ActionResult AuditPurchase(string filterPurchase, string userid, string agentid, string clientid)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(filterPurchase))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                FilterPurchase model = serializer.Deserialize<FilterPurchase>(filterPurchase);
                if (!string.IsNullOrEmpty(model.DocID))
                {
                    string details = "";
                    if (model.Details != null)
                    {
                        model.Details.ForEach(x => { details += x.AutoID + "-" + x.Num + ":" + x.DepotID + ","; });
                    }
                    string errmsg = "";
                    result = StockService.AuditPurchase(model.DocID, model.DocType, model.IsOver, details,
                        model.Remark,OperateIP, userid, agentid, clientid, ref errmsg);
                    JsonDictionary["error_msg"] = errmsg;
                }
                else
                {
                    JsonDictionary["error_code"] = -100;
                    JsonDictionary["error_msg"] = "参数filterPurchase中订单DocID不能为空";
                }
            }
            else
            {
                JsonDictionary["error_code"] = -100;
                JsonDictionary["error_msg"] = "参数filterPurchase不能为空";
            }
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 出库单

        public ActionResult GetStockOut(int status, int outstatus, int sendstatus, int returnstatus, string keywords, string beginTime, string endTime, int pagesize, int pageindex, ref int totalCount, ref int pageCount, string clientid, string agentid = "")
        {
            int total = 0;
            int pagecount = 0;
               var list = StockService.GetStockOut(status, outstatus, sendstatus, returnstatus, keywords, beginTime,
                   endTime, pagesize, pageindex, ref totalCount, ref pageCount, clientid, agentid);

            JsonDictionary.Add("TotalCount", total);
            JsonDictionary.Add("PageCount", pagecount);
            JsonDictionary.Add("result", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 订单出库--（根据issend(发货但是否自动发货)生成发货单 ）
        /// </summary>
        /// <param name="expresscode"></param>
        /// <param name="expressid"></param>
        /// <param name="issend"></param>
        /// <param name="orderid"></param>
        /// <param name="wareid"></param>
        /// <returns></returns>
        public ActionResult ConfirmAgentOrderOut(string expresscode, string expressid, string orderid, string wareid, string userid,string agentid, string clientid, ref string errmsg, int issend = 0)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(orderid) && !string.IsNullOrEmpty(wareid))
            {
                string msg = "";
                result=StockService.ConfirmAgentOrderOut(expresscode, expressid, orderid, wareid, userid, agentid, clientid,
                    ref msg, issend);
                if (!string.IsNullOrEmpty(msg))
                {
                    JsonDictionary["error_code"] = -101;
                    JsonDictionary["error_msg"] = msg;
                }
            }
            else
            {
                JsonDictionary["error_code"] = -100;
                JsonDictionary["error_msg"] = "参数orderid，wareid不能为空";
            }
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult ConfirmAgentOrderSend(string orderid, string expressid, string expresscode, string userid, string agentid, string clientid)
        {
            bool result = false;
            string errmsg = "";
            if (!string.IsNullOrEmpty(orderid) && !string.IsNullOrEmpty(expressid))
            {
                result = StockService.ConfirmAgentOrderSend(orderid, expressid, expresscode, userid, agentid, clientid, ref errmsg);
                if (!string.IsNullOrEmpty(errmsg))
                {
                    JsonDictionary["error_code"] = -101;
                    JsonDictionary["error_msg"] = errmsg;
                }
            }
             else
             {
                 JsonDictionary["error_code"] = -100;
                 JsonDictionary["error_msg"] = "参数orderid，expressid不能为空";
             }

            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion


        #region 发货单
        

        #endregion


    }
}
