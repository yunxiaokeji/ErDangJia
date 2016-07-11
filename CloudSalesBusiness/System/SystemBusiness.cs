using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesEntity;
using System.Data;
using System.IO;
using System.Web;
using CloudSalesDAL;
using CloudSalesDAL.Custom;
using CloudSalesEnum;

namespace CloudSalesBusiness
{
    public class SystemBusiness
    {
        public static SystemBusiness BaseBusiness = new SystemBusiness();
        /// <summary>
        /// 文件默认存储路径
        /// </summary>
        public static string FILEPATH = CloudSalesTool.AppSettings.Settings["UploadFilePath"] + "MemberLevel/" + DateTime.Now.ToString("yyyyMM") + "/";
        public static string TempPath = CloudSalesTool.AppSettings.Settings["UploadTempPath"];
        #region Cache

        private static Dictionary<string, List<CustomSourceEntity>> _source;
        private static Dictionary<string, List<CustomerColorEntity>> _color;
        private static Dictionary<string, List<OpportunityStageEntity>> _opportunitystages;
        private static Dictionary<string, List<OrderTypeEntity>> _ordertypes;
        private static Dictionary<string, List<TeamEntity>> _teams;
        private static Dictionary<string, List<WareHouse>> _wares;
        private static Dictionary<string, List<ClientsIndustry>> _clientInsdutryList;
        private static Dictionary<string, List<ClientMemberLevel>> _clientMemberLevelList;

        public static Dictionary<string, List<ClientMemberLevel>> ClientMemberLevelList
        {
            get
            {
                if (_clientMemberLevelList == null)
                {
                    _clientMemberLevelList = new Dictionary<string, List<ClientMemberLevel>>();
                }
                return _clientMemberLevelList;
            }
        }

        public static Dictionary<string, List<ClientsIndustry>> ClientIndustryList
        {
            get
            {
                if (_clientInsdutryList == null)
                {
                    _clientInsdutryList = new Dictionary<string, List<ClientsIndustry>>(); 
                }
                return _clientInsdutryList;
            }
        }

        private static Dictionary<string, List<CustomSourceEntity>> CustomSources
        {
            get
            {
                if (_source == null)
                {
                    _source = new Dictionary<string, List<CustomSourceEntity>>();
                }
                return _source;
            }
            set 
            {
                _source = value;
            }
        }

        private static Dictionary<string, List<CustomerColorEntity>> CustomColor
        {
            get
            {
                if (_color == null)
                {
                    _color = new Dictionary<string, List<CustomerColorEntity>>();
                }
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        private static Dictionary<string, List<OpportunityStageEntity>> OpportunityStages
        {
            get
            {
                if (_opportunitystages == null)
                {
                    _opportunitystages = new Dictionary<string, List<OpportunityStageEntity>>();
                }
                return _opportunitystages;
            }
            set
            {
                _opportunitystages = value;
            }
        }

        private static Dictionary<string, List<OrderTypeEntity>> OrderTypes
        {
            get
            {
                if (_ordertypes == null)
                {
                    _ordertypes = new Dictionary<string, List<OrderTypeEntity>>();
                }
                return _ordertypes;
            }
            set
            {
                _ordertypes = value;
            }
        }

        private static Dictionary<string, List<TeamEntity>> Teams
        {
            get
            {
                if (_teams == null)
                {
                    _teams = new Dictionary<string, List<TeamEntity>>();
                }
                return _teams;
            }
            set
            {
                _teams = value;
            }
        }

        private static Dictionary<string, List<WareHouse>> WareHouses
        {
            get
            {
                if (_wares == null)
                {
                    _wares = new Dictionary<string, List<WareHouse>>();
                }
                return _wares;
            }
            set
            {
                _wares = value;
            }
        }

        #endregion

        #region 查询

        public List<CustomSourceEntity> GetCustomSources(string agentid,string clientid)
        {
            if (CustomSources.ContainsKey(clientid)) 
            {
                return CustomSources[clientid];
            }

            List<CustomSourceEntity> list = new List<CustomSourceEntity>();
            DataTable dt = SystemDAL.BaseProvider.GetCustomSources(clientid);
            foreach (DataRow dr in dt.Rows)
            {
                CustomSourceEntity model = new CustomSourceEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                list.Add(model);
            }
            CustomSources.Add(clientid, list);

            return list;

        }

        public List<CustomerColorEntity> GetCustomerColors(string clientid)
        {
            if (CustomColor.ContainsKey(clientid))
            {
                return CustomColor[clientid];
            }
            List<CustomerColorEntity> list = new List<CustomerColorEntity>();
            DataTable dt = CustomerColorDAL.BaseProvider.GetCustomerColors(clientid);
            foreach (DataRow dr in dt.Rows)
            {
                CustomerColorEntity model = new CustomerColorEntity();
                model.FillData(dr);
                list.Add(model);
            }
            CustomColor.Add(clientid, list);
            return list;
        }

        public List<ClientsIndustry> GetClientIndustry(string agentid, string clientid)
        {
            if (ClientIndustryList.ContainsKey(clientid))
            {
                return ClientIndustryList[clientid];
            }

            List<ClientsIndustry> list = new List<ClientsIndustry>();
            DataTable dt = SystemDAL.BaseProvider.GetClientIndustry(clientid);
            foreach (DataRow dr in dt.Rows)
            {
                ClientsIndustry model = new ClientsIndustry();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                list.Add(model);
            }
            ClientIndustryList.Add(clientid, list);
            return list;

        }

        public List<ClientMemberLevel> GetClientMemberLevel(string agentid, string clientid)
        {
            if (ClientMemberLevelList.ContainsKey(clientid))
            {
                return ClientMemberLevelList[clientid];
            }

            List<ClientMemberLevel> list = new List<ClientMemberLevel>();
            DataTable dt = SystemDAL.BaseProvider.GetClientMemberLevel(clientid);
            foreach (DataRow dr in dt.Rows)
            {
                ClientMemberLevel model = new ClientMemberLevel();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                list.Add(model);
            }
            ClientMemberLevelList.Add(clientid, list);
            return list;

        }
        public ClientMemberLevel GetClientMemberLevelByID(string levelid, string agentid, string clientid)
        {
            if (string.IsNullOrEmpty(levelid))
            {
                return null;
            }
            var list = GetClientMemberLevel(agentid, clientid);
            if (list.Where(m => m.LevelID == levelid).Count() > 0)
            {
                return list.Where(m => m.LevelID == levelid).FirstOrDefault();
            }

            ClientMemberLevel model = new ClientMemberLevel();
            DataTable dt = SystemDAL.BaseProvider.GetClientMemberLevelByLevelID(levelid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                list.Add(model);
            }
            return model;
        }
        public ClientsIndustry GetClientIndustryByName(string name, string agentid,string clientid)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var list = GetClientIndustry(agentid, clientid);
            return list.Where(x => x.Name == name.Trim()).FirstOrDefault();
        }

        public ClientsIndustry GetClientIndustryByID(string clientIndustryID, string agentid, string clientid)
        {
            if (string.IsNullOrEmpty(clientIndustryID))
            {
                return null;
            }
            var list = GetClientIndustry(agentid, clientid);
            if (list.Where(m => m.ClientIndustryID == clientIndustryID).Count() > 0)
            {
                return list.Where(m => m.ClientIndustryID == clientIndustryID).FirstOrDefault();
            }

            ClientsIndustry model = new ClientsIndustry();
            DataTable dt = SystemDAL.BaseProvider.GetClientIndustryByID(clientIndustryID);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                list.Add(model);
            }
            return model;
        }

        public CustomerColorEntity GetCustomerColorsColorID(string clientid, int colorid = 0)
        {
            var list = GetCustomerColors(clientid);
            return list.Where(x => x.ColorID == colorid).FirstOrDefault();
        }

        public CustomSourceEntity GetCustomSourcesByID(string sourceid, string agentid, string clientid)
        {
            if (string.IsNullOrEmpty(sourceid))
            {
                return null;
            }
            var list = GetCustomSources(agentid, clientid);
            if (list.Where(m => m.SourceID == sourceid).Count() > 0)
            {
                return list.Where(m => m.SourceID == sourceid).FirstOrDefault();
            }

            CustomSourceEntity model = new CustomSourceEntity();
            DataTable dt = SystemDAL.BaseProvider.GetCustomSourceByID(sourceid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                CustomSources[clientid].Add(model);
            }
           
            return model;
        }

        public List<OpportunityStageEntity> GetOpportunityStages(string agentid, string clientid)
        {
            if (OpportunityStages.ContainsKey(clientid))
            {
                return OpportunityStages[clientid].OrderBy(m => m.Sort).ToList();
            }

            List<OpportunityStageEntity> list = new List<OpportunityStageEntity>();
            DataSet ds = SystemDAL.BaseProvider.GetOpportunityStages(clientid);
            foreach (DataRow dr in ds.Tables["Stages"].Rows)
            {
                OpportunityStageEntity model = new OpportunityStageEntity();
                model.FillData(dr);
                model.StageItem = new List<StageItemEntity>();
                foreach (DataRow itemdr in ds.Tables["Items"].Select("StageID='" + model.StageID + "'"))
                {
                    StageItemEntity item = new StageItemEntity();
                    item.FillData(itemdr);
                    model.StageItem.Add(item);
                }
                list.Add(model);
            }
            OpportunityStages.Add(clientid, list);

            return list;
        }

        public OpportunityStageEntity GetOpportunityStageByID(string stageid, string agentid, string clientid)
        {
            if (string.IsNullOrEmpty(stageid))
            {
                return null;
            }
            var list = GetOpportunityStages(agentid, clientid);
            if (list.Where(m => m.StageID == stageid).Count() > 0)
            {
                return list.Where(m => m.StageID == stageid).FirstOrDefault();
            }

            OpportunityStageEntity model = new OpportunityStageEntity();
            DataSet ds = SystemDAL.BaseProvider.GetOpportunityStageByID(stageid);
            if (ds.Tables["Stages"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Stages"].Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                model.StageItem = new List<StageItemEntity>();
                foreach (DataRow itemdr in ds.Tables["Items"].Rows)
                {
                    StageItemEntity item = new StageItemEntity();
                    item.FillData(itemdr);
                    model.StageItem.Add(item);
                }
                OpportunityStages[clientid].Add(model);
            }

            return model;
        }

        public List<OrderTypeEntity> GetOrderTypes(string agentid, string clientid)
        {
            if (OrderTypes.ContainsKey(clientid))
            {
                return OrderTypes[clientid].ToList();
            }

            List<OrderTypeEntity> list = new List<OrderTypeEntity>();
            DataSet ds = SystemDAL.BaseProvider.GetOrderTypes(clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                OrderTypeEntity model = new OrderTypeEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                list.Add(model);
            }

            OrderTypes.Add(clientid, list);

            return list;
        }

        public OrderTypeEntity GetOrderTypeByID(string typeid, string agentid, string clientid)
        {
            if (string.IsNullOrEmpty(typeid))
            {
                return null;
            }
            var list = GetOrderTypes(agentid, clientid);
            if (list.Where(m => m.TypeID == typeid).Count() > 0)
            {
                return list.Where(m => m.TypeID == typeid).FirstOrDefault();
            }

            OrderTypeEntity model = new OrderTypeEntity();
            DataTable dt = SystemDAL.BaseProvider.GetOrderTypeByID(typeid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                OrderTypes[clientid].Add(model);
            }
            
            return model;
        }

        public List<TeamEntity> GetTeams(string agentid)
        {
            if (Teams.ContainsKey(agentid))
            {
                return Teams[agentid];
            }

            List<TeamEntity> list = new List<TeamEntity>();
            DataTable dt = SystemDAL.BaseProvider.GetTeams(agentid);
            foreach (DataRow dr in dt.Rows)
            {
                TeamEntity model = new TeamEntity();
                model.FillData(dr);
                model.Users = OrganizationBusiness.GetUsers(agentid).Where(m => m.TeamID == model.TeamID).ToList();
                list.Add(model);
            }
            Teams.Add(agentid, list);

            return list;

        }

        public TeamEntity GetTeamByID(string teamid, string agentid)
        {

            if (string.IsNullOrEmpty(teamid))
            {
                return null;
            }
            var list = GetTeams(agentid);
            if (list.Where(m => m.TeamID == teamid).Count() > 0)
            {
                return list.Where(m => m.TeamID == teamid).FirstOrDefault();
            }

            TeamEntity model = new TeamEntity();
            DataTable dt = SystemDAL.BaseProvider.GetTeamByID(teamid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.Users = OrganizationBusiness.GetUsers(agentid).Where(m => m.TeamID == model.TeamID).ToList();
                Teams[teamid].Add(model);
            }
            
            return model;
        }

        public List<WareHouse> GetWareHouses(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            DataSet ds = SystemDAL.BaseProvider.GetWareHouses(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<WareHouse> list = new List<WareHouse>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                WareHouse model = new WareHouse();
                model.FillData(dr);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                list.Add(model);
            }
            return list;
        }

        public List<WareHouse> GetWareHouses(string clientID)
        {
            clientID = clientID.ToLower();

            if (WareHouses.ContainsKey(clientID))
            {
                return WareHouses[clientID];
            }

            DataTable dt = SystemDAL.BaseProvider.GetWareHouses(clientID);

            List<WareHouse> list = new List<WareHouse>();
            foreach (DataRow dr in dt.Rows)
            {
                WareHouse model = new WareHouse();
                model.FillData(dr);
                list.Add(model);
            }
            WareHouses.Add(clientID, list);
            return list;
        }

        public WareHouse GetWareByID(string wareid, string clientid)
        {
            if (string.IsNullOrEmpty(wareid))
            {
                return null;
            }

            wareid = wareid.ToLower();
            clientid = clientid.ToLower();

            var list = GetWareHouses(clientid);

            if (list.Where(m => m.WareID == wareid).Count() > 0)
            {
                return list.Where(m => m.WareID == wareid).FirstOrDefault();
            }

            DataTable dt = SystemDAL.BaseProvider.GetWareByID(wareid);

            WareHouse model = new WareHouse();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                list.Add(model);
            }
            return model;
        }

        public List<DepotSeat> GetDepotSeats(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID, string wareid = "")
        {
            DataSet ds = SystemDAL.BaseProvider.GetDepotSeats(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientID, wareid);

            List<DepotSeat> list = new List<DepotSeat>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DepotSeat model = new DepotSeat();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }
        
        public List<DepotSeat> GetDepotSeatsByWareID(string wareid, string clientid)
        {
            DataTable dt = SystemDAL.BaseProvider.GetDepotSeatsByWareID(wareid);

            List<DepotSeat> list = new List<DepotSeat>();
            foreach (DataRow dr in dt.Rows)
            {
                DepotSeat model = new DepotSeat();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public DepotSeat GetDepotByID(string depotid)
        {
            DataTable dt = SystemDAL.BaseProvider.GetDepotByID(depotid);

            DepotSeat model = new DepotSeat();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }
            return model;
        }

        #endregion

        #region 添加

        public string CreateCustomSource(string sourcecode, string sourcename, int ischoose, string userid, string agentid, string clientid,out int result)
        {
            string sourceid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateCustomSource(sourceid, sourcecode, sourcename, ischoose , userid, clientid, out result);
            if (bl)
            {
                if (!CustomSources.ContainsKey(clientid)) 
                {
                    GetCustomSources(agentid, clientid);
                }

                CustomSources[clientid].Add(new CustomSourceEntity()
                {
                    SourceID = sourceid.ToLower(),
                    SourceName = sourcename,
                    SourceCode = sourcecode,
                    IsChoose = ischoose,
                    IsSystem = 0,
                    Status = 1,
                    CreateTime = DateTime.Now,
                    CreateUserID = userid,
                    CreateUser = OrganizationBusiness.GetUserByUserID(userid, agentid),
                    ClientID = clientid
                });

                return sourceid;
            }
            return "";
        }

        public int CreateCustomerColor(string colorName, string colorValue, string agentid, string clientid, string userid, int status = 0)
        {
            int result= CustomerColorDAL.BaseProvider.InsertCustomerColor(colorName, colorValue, agentid,
                clientid, userid, status);
             if (result>0)
            {
                if (!CustomColor.ContainsKey(clientid))
                {
                    GetCustomerColors(clientid);
                }
                else
                {
                    CustomColor[clientid].Add(new CustomerColorEntity()
                    {
                        AgentID = agentid,
                        ColorID = result,
                        ColorValue = colorValue,
                        ColorName = colorName,
                        ClientID = clientid,
                        CreateUserID = userid,
                        CreateTime = DateTime.Now,
                        Status = 0
                    });
                }
            }
            return result;
        }
        
        public string CreateClientIndustry(string clientindustryid, string name, string agentid, string clientid, string userid, string description, int status = 1)
        {
            bool result = SystemDAL.BaseProvider.InsertClientIndustry(clientindustryid, name, clientid, agentid, userid, description, status);
            if (result)
            {
                var list = GetClientIndustry(agentid, clientid);
                list.Add(new ClientsIndustry()
                {
                    AgentID = agentid,
                    ClientIndustryID = clientindustryid,
                    Description = description,
                    Name = name,
                    ClientID = clientid,
                    CreateUserID = userid,
                    CreateTime = DateTime.Now,
                    CreateUser = OrganizationBusiness.GetUserByUserID(userid, agentid),
                    Status = 0
                });
            }
            return "";
        }
        public string CreateClientMemberLevel(string levelid, string name, string agentid, string clientid, string userid, decimal discountfee, decimal integfeemore, int status = 1, string imgurl="")
        {
            imgurl = GetUploadImgurl(imgurl);
            int origin = 1;
            string result = SystemDAL.BaseProvider.InsertClientMemberLevel(levelid, name, clientid, agentid, userid, discountfee, integfeemore, ref origin, status, imgurl);
            if (string.IsNullOrEmpty(result))
            {
                var list = GetClientMemberLevel(agentid, clientid);
                list.Add(new ClientMemberLevel()
                {
                    AgentID = agentid,
                    LevelID = levelid,
                    DiscountFee = discountfee,
                    Name = name,
                    ImgUrl = imgurl,
                    Origin = origin,
                    ClientID = clientid,
                    IntegFeeMore = integfeemore, 
                    CreateUserID = userid,
                    CreateTime = DateTime.Now,
                    CreateUser = OrganizationBusiness.GetUserByUserID(userid, agentid),
                    Status = 0
                });
            }
            return result;
        }
        
        public string CreateStageItem(string name, string stageid, string userid, string agentid, string clientid)
        {
            string itemid = Guid.NewGuid().ToString().ToLower();

            bool bl = SystemDAL.BaseProvider.CreateStageItem(itemid, name, stageid, userid, clientid);
            if (bl)
            {
                var model = GetOpportunityStageByID(stageid, agentid, clientid);
                if (model.StageItem == null)
                {
                    model.StageItem = new List<StageItemEntity>();
                }
                model.StageItem.Add(new StageItemEntity()
                {
                    ItemID = itemid,
                    ItemName = name,
                    StageID = stageid,
                    ClientID = clientid,
                    CreateTime = DateTime.Now
                });

                return itemid;
            }
            return "";
        }

        public string CreateOpportunityStage(string stagename, decimal probability, int sort, string userid, string agentid, string clientid,out int result)
        {
            string guid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateOpportunityStage(guid, stagename, probability, sort, userid, clientid,out result);
            if (bl)
            {
                if (!OpportunityStages.ContainsKey(clientid))
                {
                    GetOpportunityStages(agentid, clientid);
                }

                var list = OpportunityStages[clientid].Where(m => m.Sort >= sort && m.Status == 1).ToList();
                foreach (var model in list)
                {
                    model.Sort += 1;
                }

                OpportunityStages[clientid].Add(new OpportunityStageEntity()
                {
                    StageID = guid.ToLower(),
                    StageName = stagename,
                    Probability = probability,
                    Status = 1,
                    Sort = sort,
                    CreateTime = DateTime.Now,
                    CreateUserID = userid,
                    CreateUser = OrganizationBusiness.GetUserByUserID(userid, agentid),
                    StageItem = new List<StageItemEntity>(),
                    ClientID = clientid
                });

                return guid;
            }
            return "";
        }

        public string CreateOrderType(string typename, string typecode, string userid, string agentid, string clientid)
        {
            string typeid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateOrderType(typeid, typename, typecode, userid, clientid);
            if (bl)
            {
                if (!OrderTypes.ContainsKey(clientid))
                {
                    GetOrderTypes(agentid, clientid);
                }

                OrderTypes[clientid].Add(new OrderTypeEntity()
                {
                    TypeID = typeid.ToLower(),
                    TypeName = typename,
                    TypeCode = typecode,
                    Status = 1,
                    CreateTime = DateTime.Now,
                    CreateUserID = userid,
                    CreateUser = OrganizationBusiness.GetUserByUserID(userid, agentid),
                    ClientID = clientid
                });

                return typeid;
            }
            return "";
        }

        public string CreateTeam(string teamname, string userid, string agentid, string clientid)
        {
            string teamid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateTeam(teamid, teamname, userid, agentid, clientid);
            if (bl)
            {
                if (!Teams.ContainsKey(agentid))
                {
                    GetTeams(agentid);
                }

                Teams[agentid].Add(new TeamEntity()
                {
                    TeamID = teamid.ToLower(),
                    TeamName = teamname,
                    Status = 1,
                    CreateTime = DateTime.Now,
                    CreateUserID = userid,
                    ClientID = clientid,
                    Users = new List<Users>()
                });

                return teamid;
            }
            return "";
        }

        public string AddWareHouse(string warecode, string name, string shortname, string citycode, int status, string depotcode, string depotname, string description, string operateid, string clientid)
        {
            var id = Guid.NewGuid().ToString();
            if (SystemDAL.BaseProvider.AddWareHouse(id, warecode, name, shortname, citycode, status, depotcode, depotname, description, operateid, clientid))
            {
                if (!WareHouses.ContainsKey(clientid))
                {
                    GetWareHouses(clientid);
                }
                var model = new WareHouse()
                {
                    WareID = id,
                    WareCode = warecode,
                    Name = name,
                    ShortName = shortname,
                    CityCode = citycode,
                    Status = status,
                    Description = description,
                    CreateUserID = operateid,
                    ClientID = clientid,
                    CreateTime = DateTime.Now
                };
                WareHouses[clientid].Add(model);
                return id.ToString();
            }

            

            return string.Empty;
        }

        public string AddDepotSeat(string depotcode, string wareid, string name, int status, string description, string operateid, string clientid)
        {
            var id = Guid.NewGuid().ToString();
            if (SystemDAL.BaseProvider.AddDepotSeat(id, depotcode, wareid, name, status, description, operateid, clientid))
            {
                return id.ToString();
            }
            return string.Empty;
        }

        #endregion

        #region 编辑/删除

        public bool UpdateCustomSource(string sourceid, string sourcename, int ischoose, string userid,string ip, string agentid, string clientid)
        {
            var model = GetCustomSourcesByID(sourceid, agentid, clientid);
            if (string.IsNullOrEmpty(sourcename))
            {
                sourcename = model.SourceName;
            }
            bool bl = SystemDAL.BaseProvider.UpdateCustomSource(sourceid, sourcename, ischoose, clientid);
            if (bl)
            {
                model.SourceName = sourcename;
                model.IsChoose = ischoose;
            }
            return bl;
        }
       
        public bool DeleteCustomSource(string sourceid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetCustomSourcesByID(sourceid, agentid, clientid);
            //系统默认来源不能删除
            if (model.IsSystem == 1)
            {
                return false;
            }
            bool bl = SystemDAL.BaseProvider.DeleteCustomSource(sourceid, clientid);
            if (bl)
            {
                CustomSources[clientid].Remove(model);
            }
            return bl;
        }

        public int UpdateClientIndustry(string agentid, string clientid, string clientindustryid, string name, string desc)
        {
            var model = GetClientIndustryByID(clientindustryid, agentid, clientid);
            if (model == null)
            {
                return -200;
            }
            bool result = SystemDAL.BaseProvider.UpdateClientIndustry(clientid, clientindustryid, name, desc);
            if (result)
            {
                model.Name = name;
                model.Description = desc;
            }
            return result ? 1 : 0;
        }

        public string UpdateClientMemberLevel(string clientid, string levelid, string name, decimal discountfee, decimal integfeemore, string imgurl)
        {
            var model = GetClientMemberLevelByID(levelid, "",clientid);
            if (model == null)
            {
                return "会员等级已被删除,操作失败";
            }
            imgurl=GetUploadImgurl(imgurl);
            string result = SystemDAL.BaseProvider.UpdateClientMemberLevel(clientid, levelid, name, discountfee, integfeemore, imgurl);
            if (string.IsNullOrEmpty(result))
            {
                model.Name = name;
                model.DiscountFee = discountfee;
                model.IntegFeeMore = integfeemore;
                model.ImgUrl = imgurl;
            }
            return result;
        }

        public string DeleteClientMemberLevel(string clientid, string agentid, string levelid)
        {
            var model = GetClientMemberLevelByID(levelid, agentid, clientid);
            string bl = SystemDAL.BaseProvider.DeleteClientMemberLevel(clientid, levelid);
            if (string.IsNullOrEmpty(bl))
            {
                ClientMemberLevelList[clientid].Remove(model);
            }
            return bl;
        }

        public bool DeleteClientIndustry(string clientid,string agentid, string clientindustryid)
        {
            var model = GetClientIndustryByID(clientindustryid, agentid, clientid);
            bool bl = SystemDAL.BaseProvider.DeleteClientIndustry(clientid, clientindustryid);
            if (bl)
            {
                ClientIndustryList[clientid].Remove(model);
            }
            return bl;
        }

        public int UpdateCustomerColor(string agentid, string clientid, int colorid, string colorName, string colorValue, string updateuserid)
        {
            var model = GetCustomerColorsColorID(clientid, colorid);
            if (model == null)
            {
                return -200;
            } 
            bool result = CustomerColorDAL.BaseProvider.UpdateCustomerColor(agentid, clientid, colorid, colorName, colorValue, updateuserid);
            if (result)
            {
                if (!CustomColor.ContainsKey(clientid))
                {
                    GetCustomerColors( clientid);
                }
                else
                { 
                    model.ColorValue = colorValue;
                    model.ColorName = colorName;
                    model.UpdateTime = DateTime.Now;
                    model.UpdateUserID = updateuserid; 
                }
            }
            return result?1:0;
        }

        public int DeleteCutomerColor(int colorid, string agentid, string clientid, string updateuserid)
        {
            var model = GetCustomerColorsColorID(clientid, colorid);
            if (model == null)
            {
                return -200;
            }
            if (CustomColor[clientid].Count == 1)
            {
                return -100;
            }
            bool result = CustomerColorDAL.BaseProvider.DeleteCustomColor(colorid, agentid, clientid, updateuserid);
            if (result)
            {
                if (!CustomColor.ContainsKey(clientid))
                {
                    GetCustomerColors(clientid);
                }
                else
                {
                    CustomColor[clientid].Remove(model);
                }
            }
            return result ? 1 : 10002;
        }

        public bool UpdateOpportunityStage(string stageid, string stagename, decimal probability, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOpportunityStageByID(stageid, agentid, clientid);

            bool bl = SystemDAL.BaseProvider.UpdateOpportunityStage(stageid, stagename, probability, clientid);
            if (bl)
            {
                model.StageName = stagename;
                model.Probability = probability;
            }
            return bl;
        }

        public bool DeleteOpportunityStage(string stageid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOpportunityStageByID(stageid, agentid, clientid);
            //新客户和成交客户不能删除
            if (model.Mark != 0)
            {
                return false;
            }
            bool bl = SystemDAL.BaseProvider.DeleteOpportunityStage(stageid, userid, clientid);
            if (bl)
            {
                OpportunityStages[clientid].Remove(model);

                var list = OpportunityStages[clientid].Where(m => m.Sort > model.Sort && m.Status == 1).ToList();
                foreach (var stage in list)
                {
                    stage.Sort -= 1;
                }
            }
            return bl;
        }

        public bool UpdateStageItem(string itemid, string name, string stageid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOpportunityStageByID(stageid, agentid, clientid);

            bool bl = CommonBusiness.Update("StageItem", "ItemName", name, "ItemID='" + itemid + "'");
            if (bl)
            {
                var item = model.StageItem.Where(m => m.ItemID == itemid).FirstOrDefault();
                item.ItemName = name;
            }
            return bl;
        }

        public bool DeleteStageItem(string itemid, string stageid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOpportunityStageByID(stageid, agentid, clientid);

            bool bl = CommonBusiness.Update("StageItem", "Status", "9", "ItemID='" + itemid + "'");
            if (bl)
            {
                var item = model.StageItem.Where(m => m.ItemID == itemid).FirstOrDefault();
                model.StageItem.Remove(item);
            }
            return bl;
        }

        public bool UpdateOrderType(string typeid, string typename, string typecode, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderTypeByID(typeid, agentid, clientid);

            bool bl = SystemDAL.BaseProvider.UpdateOrderType(typeid, typename, typecode, clientid);
            if (bl)
            {
                model.TypeName = typename;
                model.TypeCode = typecode;
            }
            return bl;
        }

        public bool DeleteOrderType(string typeid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetOrderTypeByID(typeid, agentid, clientid);

            bool bl = SystemDAL.BaseProvider.DeleteOrderType(typeid, clientid);
            if (bl)
            {
                OrderTypes[clientid].Remove(model);
            }
            return bl;
        }

        public bool UpdateTeam(string teamid, string name, string userid, string ip, string agentid, string clientid)
        {
            var model = GetTeamByID(teamid, agentid);

            bool bl = CommonBusiness.Update("Teams", "TeamName", name, "TeamID='" + teamid + "'");
            if (bl)
            {
                model.TeamName = name;
            }
            return bl;
        }

        public bool DeleteTeam(string teamid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetTeamByID(teamid, agentid);

            bool bl = SystemDAL.BaseProvider.DeleteTeam(teamid, userid, agentid);
            if (bl)
            {
                var list = OrganizationBusiness.GetUsers(agentid).Where(m => m.TeamID == teamid).ToList();
                foreach (var user in list)
                {
                    user.TeamID = "";
                }
                Teams[agentid].Remove(model);
            }
            return bl;
        }

        public bool UpdateUserTeamID(string userid, string teamid, string agentid, string operateid, string ip, string clientid)
        {
            var model = OrganizationBusiness.GetUserByUserID(userid, agentid);
            
            bool bl = SystemDAL.BaseProvider.UpdateUserTeamID(userid, teamid, operateid, agentid);
            if (bl)
            {
                
                if (string.IsNullOrEmpty(teamid))
                {
                    var team = GetTeamByID(model.TeamID, agentid);
                    var user = team.Users.Where(m => m.UserID == userid).FirstOrDefault();
                    team.Users.Remove(user);
                }
                else
                {
                    var team = GetTeamByID(teamid, agentid);
                    team.Users.Add(model);
                }

                model.TeamID = teamid;
            }
            return bl;
        }

        public bool UpdateWareHouse(string id, string code, string name, string shortname, string citycode, int status, string description, string operateid, string clientid)
        {
            var bl = SystemDAL.BaseProvider.UpdateWareHouse(id, code, name, shortname, citycode, status, description);
            if (bl)
            {
                var model = GetWareByID(id, clientid);
                model.WareCode = code;
                model.Name = name;
                model.ShortName = shortname;
                model.CityCode = citycode;
                model.Status = status;
                model.Description = description;
            }
            return bl;
        }

        public bool DeleteWareHouse(string wareid, string userid, string clientid, out int result)
        {
            bool bl = SystemDAL.BaseProvider.DeleteWareHouse(wareid, userid, clientid, out result);
            if (bl)
            {
                var model = GetWareByID(wareid, clientid);
                WareHouses[clientid.ToLower()].Remove(model);
            }
            return bl;
        }

        public bool UpdateWareHouseStatus(string id, EnumStatus status, string operateid, string clientid)
        {
            bool bl= CommonBusiness.Update("WareHouse", "Status", (int)status, " WareID='" + id + "'");
            if (bl)
            {
                var model = GetWareByID(id, clientid);
                model.Status = (int)status;
            }
            return bl;
        }

        public bool UpdateDepotSeat(string id, string depotcode, string name, int status, string description, string operateid, string clientid)
        {
            return SystemDAL.BaseProvider.UpdateDepotSeat(id, depotcode, name, status, description);
        }

        public bool UpdateDepotSeatStatus(string id, EnumStatus status, string operateid, string clientid)
        {
            return CommonBusiness.Update("DepotSeat", "Status", (int)status, " DepotID='" + id + "'");
        }

        public bool DeleteDepotSeat(string depotid, string userid, string clientid, out int result)
        {
            bool bl = SystemDAL.BaseProvider.DeleteDepotSeat(depotid, userid, clientid, out result);
            
            return bl;
        }

        #endregion

        public string GetUploadImgurl(string imgurl)
        {
            if (!string.IsNullOrEmpty(imgurl) && imgurl.IndexOf(TempPath) >= 0)
            {
                DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath(FILEPATH));
                if (!directory.Exists)
                {
                    directory.Create();
                }
                if (imgurl.IndexOf("?") > 0)
                {
                    imgurl = imgurl.Substring(0, imgurl.IndexOf("?"));
                }
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(imgurl));
                imgurl = FILEPATH + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(imgurl));
                }
            }
            return imgurl;
        }
    }
}
