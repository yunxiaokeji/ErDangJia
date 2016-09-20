using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using CloudSalesBusiness; 
using CloudSalesEntity;
using CloudSalesBusiness.Manage;
using CloudSalesEntity.Manage;
using CloudSalesEnum;

namespace YXERP.Controllers
{
    public class SystemController : BaseController
    {
        //
        // GET: /System/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sources()
        {
            ViewBag.integerFee = CommonBusiness.getClientSetting(EnumSettingKey.IntegralScale, "DValue",
                CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.Items = new SystemBusiness().GetClientIndustry(CurrentUser.AgentID, CurrentUser.ClientID);
            return View();
        }

        public ActionResult Stages()
        {
            var list = SystemBusiness.BaseBusiness.GetOpportunityStages(CurrentUser.AgentID, CurrentUser.ClientID);
            ViewBag.Items = list;
            return View();
        }

        public ActionResult OpportunityStages()
        {
            return View();
        }

        public ActionResult Teams()
        {
            return View();
        }

        public ActionResult OrderType()
        {
            return View();
        }

        public ActionResult Target()
        {
            return View();
        }

        public ActionResult Warehouse()
        {
            return View();
        }

        public ActionResult DepotSeat(string id = "")
        {
            if (string.IsNullOrEmpty(id)) 
            {
                return Redirect("/System/Warehouse");
            }
            ViewBag.Ware = new SystemBusiness().GetWareByID(id, CurrentUser.ClientID);
            return View();
        }

        public ActionResult Client(string id)
        {
            ViewBag.Industry = CloudSalesBusiness.Manage.IndustryBusiness.GetIndustrys();
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Option = 1;
            }
            else
            {
                ViewBag.Option = id;
            }
            ViewBag.CMClientID = CurrentUser.Agents.CMClientID;
            ViewBag.ClientID = CurrentUser.ClientID;
            ViewBag.BaseUrl = GetbaseUrl();
            return View();
        }

        #region Ajax

        #region 客户来源

        /// <summary>
        /// 获取客户来源列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCustomSources()
        {

            var list = new SystemBusiness().GetCustomSources(CurrentUser.AgentID, CurrentUser.ClientID).Where(m => m.Status == 1).ToList();
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        /// <summary>
        /// 客户颜色标记
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCustomColor()
        {

            var list = SystemBusiness.BaseBusiness.GetCustomerColors(CurrentUser.ClientID).ToList();
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetCustomerColorByColorID(int colorid)
        {
            var model = new SystemBusiness().GetCustomerColorsColorID(CurrentUser.ClientID, colorid);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveCustomerColor(string customercolor)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CustomerColorEntity model = serializer.Deserialize<CustomerColorEntity>(customercolor);
            model.CreateUserID = CurrentUser.UserID;
            model.ClientID = CurrentUser.ClientID;
            model.AgentID = CurrentUser.AgentID;
            model.Status = 0;
            int ColorID =-1;
            if ( model.ColorID==0)
            {
                ColorID = SystemBusiness.BaseBusiness.CreateCustomerColor(model.ColorName, model.ColorValue,
                    model.AgentID, model.ClientID, model.CreateUserID, model.Status);
            }
            else
            {
                int bl = SystemBusiness.BaseBusiness.UpdateCustomerColor(model.AgentID, model.ClientID, model.ColorID,
                    model.ColorName, model.ColorValue, CurrentUser.UserID); 
                ColorID =bl > 0? model.ColorID:bl;
              
            }
            JsonDictionary.Add("ID", ColorID);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteColor(int colorid)
        {
            int result = SystemBusiness.BaseBusiness.DeleteCutomerColor(colorid, CurrentUser.AgentID, CurrentUser.ClientID,
                CurrentUser.UserID);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取客户来源实体
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCustomSourceByID(string id)
        {

            var model = new SystemBusiness().GetCustomSourcesByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存客户来源
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult SaveCustomSource(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            CustomSourceEntity model = serializer.Deserialize<CustomSourceEntity>(entity);

            int result = 0;

            if (string.IsNullOrEmpty(model.SourceID))
            {
                model.SourceID = new SystemBusiness().CreateCustomSource(model.SourceCode, model.SourceName, model.IsChoose, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateCustomSource(model.SourceID, model.SourceName, model.IsChoose, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (bl)
                {
                    result = 1;
                }
            }
            JsonDictionary.Add("result", result);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 删除客户来源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteCustomSource(string id)
        {
            bool bl = new SystemBusiness().DeleteCustomSource(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("result", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        } 

        #endregion

        #region 机会阶段配置

        public JsonResult SaveStage(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OpportunityStageEntity model = serializer.Deserialize<OpportunityStageEntity>(entity);

            int result = 0;

            if (string.IsNullOrEmpty(model.StageID))
            {
                model.StageID = new SystemBusiness().CreateOpportunityStage(model.StageName, model.Probability, model.Sort, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID, out result);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateOpportunityStage(model.StageID, model.StageName, model.Probability, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (bl)
                {
                    result = 1;
                }
            }
            JsonDictionary.Add("status", result);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteStage(string id)
        {
            bool bl = new SystemBusiness().DeleteOpportunityStage(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveStageItem(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StageItemEntity model = serializer.Deserialize<StageItemEntity>(entity);

            if (string.IsNullOrEmpty(model.ItemID))
            {
                model.ItemID = new SystemBusiness().CreateStageItem(model.ItemName, model.StageID, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateStageItem(model.ItemID, model.ItemName, model.StageID, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.ItemID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteStageItem(string id, string stageid)
        {
            bool bl = new SystemBusiness().DeleteStageItem(id, stageid, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        } 

        #endregion

        #region 订单类型

        public JsonResult GetOrderTypes()
        {

            var list = new SystemBusiness().GetOrderTypes(CurrentUser.AgentID, CurrentUser.ClientID).ToList();
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetOrderTypeByID(string id)
        {

            var model = new SystemBusiness().GetOrderTypeByID(id, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveOrderType(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            OrderTypeEntity model = serializer.Deserialize<OrderTypeEntity>(entity);

            if (string.IsNullOrEmpty(model.TypeID))
            {
                model.TypeID = new SystemBusiness().CreateOrderType(model.TypeName, model.TypeCode, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateOrderType(model.TypeID, model.TypeName, model.TypeCode, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.TypeID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteOrderType(string id)
        {
            bool bl = new SystemBusiness().DeleteOrderType(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 销售团队

        public JsonResult GetTeams()
        {

            var list = new SystemBusiness().GetTeams(CurrentUser.AgentID).ToList();
            
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveTeam(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TeamEntity model = serializer.Deserialize<TeamEntity>(entity);

            if (string.IsNullOrEmpty(model.TeamID))
            {
                model.TeamID = new SystemBusiness().CreateTeam(model.TeamName, CurrentUser.UserID, CurrentUser.AgentID, CurrentUser.ClientID);
            }
            else
            {
                bool bl = new SystemBusiness().UpdateTeam(model.TeamID, model.TeamName, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
                if (!bl)
                {
                    model.TeamID = "";
                }
            }
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteTeam(string id)
        {
            bool bl = new SystemBusiness().DeleteTeam(id, CurrentUser.UserID, OperateIP, CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateUserTeamID(string ids, string teamid)
        {
            bool bl = false;//
            string[] list = ids.Split(',');
            foreach (var userid in list)
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    if (new SystemBusiness().UpdateUserTeamID(userid, teamid, CurrentUser.AgentID, CurrentUser.UserID, OperateIP, CurrentUser.ClientID))
                    {
                        bl = true;
                    }
                }
            }
            JsonDictionary.Add("status", bl);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        #endregion

        #region 仓库货位

        public JsonResult GetAllWareHouses()
        {
            List<WareHouse> list = new SystemBusiness().GetWareHouses(CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetWareHouses(string keyWords, int pageSize, int pageIndex, int totalCount)
        {
            int pageCount = 0;
            List<WareHouse> list = new SystemBusiness().GetWareHouses(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetWareHouseByID(string id)
        {
            WareHouse model = new SystemBusiness().GetWareByID(id, CurrentUser.ClientID);
            JsonDictionary.Add("model", model);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveWareHouse(string ware)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            WareHouse model = serializer.Deserialize<WareHouse>(ware);

            string id = string.Empty;
            if (string.IsNullOrEmpty(model.WareID))
            {
                id = new SystemBusiness().AddWareHouse(model.WareCode, model.Name, model.ShortName, model.CityCode, model.Status.Value, model.DepotCode, model.DepotName, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else if (new SystemBusiness().UpdateWareHouse(model.WareID, model.WareCode, model.Name, model.ShortName, model.CityCode, model.Status.Value, model.Description, CurrentUser.UserID, CurrentUser.ClientID))
            {
                id = model.WareID;
            }


            JsonDictionary.Add("ID", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateWareHouseStatus(string id, int status)
        {
            bool bl = new SystemBusiness().UpdateWareHouseStatus(id, (CloudSalesEnum.EnumStatus)status, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteWareHouse(string id)
        {
            int result = 0;
            bool bl = new SystemBusiness().DeleteWareHouse(id, CurrentUser.UserID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveDepotSeat(string obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            DepotSeat model = serializer.Deserialize<DepotSeat>(obj);

            string id = string.Empty;
            if (string.IsNullOrEmpty(model.DepotID))
            {
                id = new SystemBusiness().AddDepotSeat(model.DepotCode, model.WareID, model.Name, model.Status.Value, model.Description, CurrentUser.UserID, CurrentUser.ClientID);
            }
            else if (new SystemBusiness().UpdateDepotSeat(model.DepotID, model.WareID, model.DepotCode, model.Name, model.Status.Value, model.Description, CurrentUser.UserID, CurrentUser.ClientID))
            {
                id = model.WareID;
            }


            JsonDictionary.Add("ID", id);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetDepotSeats(string wareid, string keyWords, int pageSize, int pageIndex, int totalCount)
        {
            int pageCount = 0;
            List<DepotSeat> list = new SystemBusiness().GetDepotSeats(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, CurrentUser.ClientID, wareid);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetDepotSeatsByWareID(string wareid)
        {
            List<DepotSeat> list = new SystemBusiness().GetDepotSeatsByWareID(wareid, CurrentUser.ClientID);
            JsonDictionary.Add("Items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetDepotByID(string id, string wareid)
        {
            var model = new SystemBusiness().GetDepotByID(id, wareid, CurrentUser.ClientID);
            JsonDictionary.Add("Item", model);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteDepotSeat(string id, string wareid)
        {
            int result = 0;
            bool bl = new SystemBusiness().DeleteDepotSeat(id, wareid, CurrentUser.UserID, CurrentUser.ClientID, out result);
            JsonDictionary.Add("status", bl);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult UpdateDepotSeatStatus(string id, string wareid, int status)
        {
            bool bl = new SystemBusiness().UpdateDepotSeatStatus(id, wareid, (CloudSalesEnum.EnumStatus)status, CurrentUser.UserID, CurrentUser.ClientID);
            JsonDictionary.Add("Status", bl);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 公司信息
        /// <summary>
        /// 获取客户详情
        /// </summary>
        public JsonResult GetClientDetail()
        {
            var client = ClientBusiness.GetClientDetail(CurrentUser.ClientID);
            var agent = AgentsBusiness.GetAgentDetail(CurrentUser.AgentID);

            JsonDictionary.Add("Client", client);
            JsonDictionary.Add("Agent", agent);
            JsonDictionary.Add("Days", (agent.EndTime - DateTime.Now).Days);
            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取公司订单列表
        /// </summary>
        /// <param name="status"></param>
        /// <param name="type"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public JsonResult GetClientOrders(int status, int type, string beginDate, string endDate, int pageSize, int pageIndex, string keyWords="")
        {
            int pageCount = 0;
            int totalCount = 0;

            List<ClientOrder> list = ClientOrderBusiness.GetClientOrders(keyWords,status, type, beginDate, endDate, CurrentUser.AgentID, CurrentUser.ClientID, pageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 保存公司基本信息
        /// </summary>
        public JsonResult SaveClient(string entity)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Clients model = serializer.Deserialize<Clients>(entity);
            model.ClientID = CurrentUser.ClientID;

            bool flag = ClientBusiness.UpdateClient(model, CurrentUser.UserID);
            JsonDictionary.Add("Result", flag ? 1 : 0);


            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 关闭公司订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult CloseClientOrder(string id)
        {
            bool flag = ClientOrderBusiness.UpdateClientOrderStatus(id, CloudSalesEnum.EnumClientOrderStatus.Delete);
            JsonDictionary.Add("Result", flag ? 1 : 0);


            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /// <summary>
        /// 获取公司密钥
        /// </summary>
        public JsonResult GetAgentKey()
        {
            var agent = AgentsBusiness.GetAgentDetail(CurrentUser.AgentID);
            string key = string.Empty;

            if (string.IsNullOrEmpty(agent.AgentKey))
            {
                key = DateTime.Now.Ticks.ToString();
                AgentsBusiness.UpdateAgentKey(CurrentUser.AgentID, key);
            }
            else
                key = agent.AgentKey;

            JsonDictionary.Add("Key",key);

            return new JsonResult()
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult GetClientAuthorizeLogs(int pageIndex)
        {
            int pageCount = 0;
            int totalCount = 0;
            var list = ClientBusiness.GetClientAuthorizeLogs(CurrentUser.ClientID, "", PageSize, pageIndex, ref totalCount, ref pageCount);
            JsonDictionary.Add("Items", list);
            JsonDictionary.Add("TotalCount", totalCount);
            JsonDictionary.Add("PageCount", pageCount);

            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 客户行业
        /// <summary>
        /// 获取客户来源列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetClientIndustry()
        {

            var list = new SystemBusiness().GetClientIndustry(CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveClientIndustry(string clientindustry)
        { 
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ClientsIndustry model = serializer.Deserialize<ClientsIndustry>(clientindustry);
            model.CreateUserID = CurrentUser.UserID;
            model.ClientID = CurrentUser.ClientID;
            model.AgentID = CurrentUser.AgentID;
            model.Status = 1;
            string result = "1";
            if (string.IsNullOrEmpty(model.ClientIndustryID))
            {
                if (SystemBusiness.BaseBusiness.GetClientIndustryByName(model.Name, CurrentUser.AgentID, CurrentUser.ClientID) == null)
                {
                    string mes = SystemBusiness.BaseBusiness.CreateClientIndustry(Guid.NewGuid().ToString(),
                        model.Name.Trim(),
                        model.AgentID, model.ClientID, model.CreateUserID, model.Description, model.Status);
                    result = string.IsNullOrEmpty(mes) ? result : mes;
                }
                else
                {
                    result = "行业名称已存在，不能重复添加";
                }
            }
            else
            {
                int bl = SystemBusiness.BaseBusiness.UpdateClientIndustry(model.AgentID, model.ClientID, model.ClientIndustryID, model.Name, model.Description);
                result = bl > 0 ? result :"修改失败";
            }
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            }; 

        }

        public JsonResult DeleteClientIndustry(string clientindustryid)
        {
            bool result = SystemBusiness.BaseBusiness.DeleteClientIndustry(CurrentUser.ClientID,CurrentUser.AgentID, clientindustryid); 
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion

        #region 客户会员等级
        /// <summary>
        /// 获取客户来源列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetClientMemberLevel()
        {
            var list = new SystemBusiness().GetClientMemberLevel(CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("items", list);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveClientMemberLevel(string clientmemberlevel)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<ClientMemberLevel> modelList = serializer.Deserialize<List<ClientMemberLevel>>(clientmemberlevel);
            var tempList = SystemBusiness.BaseBusiness.GetClientMemberLevel(CurrentUser.AgentID, CurrentUser.ClientID);
            modelList.ForEach(x =>
            {
                x.CreateUserID = CurrentUser.UserID;
                x.ClientID = CurrentUser.ClientID;
                x.AgentID = CurrentUser.AgentID;
                x.Status = 1;
                var temp = tempList.Where(y => y.Origin == x.Origin).FirstOrDefault();
                if (temp != null)
                {
                    x.LevelID = temp.LevelID;
                } 
            });
            var delList = tempList.Where(x => !modelList.Exists(y => y.Origin == x.Origin)).OrderByDescending(x => x.Origin).ToList();
            var addList = modelList.Where(x => string.IsNullOrEmpty(x.LevelID)).ToList();
            var updList = modelList.Where(x => !string.IsNullOrEmpty(x.LevelID)).ToList();
            string result = "";
            if (delList.Any())
            {
                delList.ForEach(x =>
                {
                    string tempresult= SystemBusiness.BaseBusiness.DeleteClientMemberLevel(CurrentUser.ClientID, CurrentUser.AgentID, x.LevelID);
                    if (result.IndexOf(tempresult) == -1)
                    {
                        result += tempresult + ",";
                    }
                });
            }
            updList.ForEach(x =>
            {
                result += SystemBusiness.BaseBusiness.UpdateClientMemberLevel(x.ClientID, x.LevelID,
                x.Name, x.DiscountFee, x.IntegFeeMore, x.ImgUrl);
            });
            if (addList.Any())
            {
                addList.ForEach(x =>
                {
                    string mes = SystemBusiness.BaseBusiness.CreateClientMemberLevel(Guid.NewGuid().ToString(),
                        x.Name.Trim(), x.AgentID, x.ClientID, x.CreateUserID, x.DiscountFee,
                        x.IntegFeeMore, x.Status, x.ImgUrl,x.Origin);
                    result += string.IsNullOrEmpty(mes) ? result : mes;
                });
            }
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult DeleteClientMemberLevel(string levelid)
        {
            string result = SystemBusiness.BaseBusiness.DeleteClientMemberLevel(CurrentUser.ClientID, CurrentUser.AgentID, levelid);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult SaveClietRule(decimal integerFee)
        {
            bool result =CommonBusiness.SetClientSetting(EnumSettingKey.IntegralScale, integerFee, CurrentUser.UserID, OperateIP,
                CurrentUser.AgentID, CurrentUser.ClientID);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonResult ChangeCumoterLevel()
        {
            bool result = CustomBusiness.BaseBusiness.RefreshCustomerLeve( CurrentUser.AgentID, CurrentUser.ClientID ,OperateIP, CurrentUser.UserID);
            JsonDictionary.Add("result", result);
            return new JsonResult
            {
                Data = JsonDictionary,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion
        #endregion

    }
}
