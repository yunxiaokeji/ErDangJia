using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class SystemDAL : BaseDAL
    {
        public static SystemDAL BaseProvider = new SystemDAL();

        #region 查询

        public DataTable GetCustomSources(string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataTable dt = GetDataTable("select * from CustomSource where ClientID=@ClientID and Status=1 order by IsChoose desc,AutoID", paras, CommandType.Text);

            return dt;
        }

        public DataTable GetCustomSourceByID(string sourceid)
        {
            string sqlText = "select * from CustomSource where SourceID=@SourceID and Status=1";
            SqlParameter[] paras = { 
                                     new SqlParameter("@SourceID",sourceid)
                                   };

            return GetDataTable(sqlText, paras, CommandType.Text);
        }

        public DataTable GetClientIndustry(string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataTable dt = GetDataTable("select * from ClientsIndustry where ClientID=@ClientID and Status=1 order by AutoID", paras, CommandType.Text);

            return dt;
        }

        public DataTable GetClientMemberLevel(string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataTable dt = GetDataTable("select * from ClientMemberLevel where ClientID=@ClientID and Status=1 order by Origin", paras, CommandType.Text);

            return dt;
        }

        public DataTable GetClientMemberLevelByLevelID(string levelID)
        {
            string sqlText = "select * from ClientMemberLevel where LevelID=@LevelID and Status=1";
            SqlParameter[] paras = { 
                                     new SqlParameter("@LevelID",levelID)
                                   };

            return GetDataTable(sqlText, paras, CommandType.Text);
        }
        public DataTable GetClientMemberLevelByOrigin(string origin, string clientid)
        {
            string sqlText = "select * from ClientMemberLevel where origin=@Origin and ClientID=@ClientID and Status=1";
            SqlParameter[] paras = { 
                                     new SqlParameter("@Origin",origin),
                                     new SqlParameter("@ClientID",clientid)
                                   };

            return GetDataTable(sqlText, paras, CommandType.Text);
        }
        public DataTable GetClientIndustryByID(string clientIndustryID)
        {
            string sqlText = "select * from ClientsIndustry where ClientIndustryID=@clientIndustryID and Status=1";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ClientIndustryID",clientIndustryID)
                                   };

            return GetDataTable(sqlText, paras, CommandType.Text);
        }


        public DataSet GetOpportunityStages(string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return GetDataSet("select * from OpportunityStage where ClientID=@ClientID and Status=1 Order by Sort; select * from StageItem where ClientID=@ClientID and Status=1;", paras, CommandType.Text, "Stages|Items");

        }

        public DataSet GetOpportunityStageByID(string stageid)
        {
            string sqlText = "select * from OpportunityStage where StageID=@StageID and Status=1; select * from StageItem where StageID=@StageID and Status=1;";
            SqlParameter[] paras = { 
                                     new SqlParameter("@StageID",stageid)
                                   };

            return GetDataSet(sqlText, paras, CommandType.Text, "Stages|Items");
        }

        public DataSet GetOrderTypes(string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ClientID",clientid)
                                   };

            return GetDataSet("select * from OrderType where ClientID=@ClientID and Status=1 Order by AutoID", paras, CommandType.Text);

        }

        public DataTable GetOrderTypeByID(string typeid)
        {
            string sqlText = "select * from OrderType where TypeID=@TypeID and Status=1";
            SqlParameter[] paras = { 
                                     new SqlParameter("@TypeID",typeid)
                                   };

            return GetDataTable(sqlText, paras, CommandType.Text);
        }

        public DataTable GetTeams(string agentid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@AgentID",agentid)
                                   };

            DataTable dt = GetDataTable("select * from Teams where AgentID=@AgentID and Status=1 ", paras, CommandType.Text);

            return dt;
        }

        public DataTable GetTeamByID(string teamid)
        {
            string sqlText = "select * from Teams where TeamID=@TeamID and Status=1";
            SqlParameter[] paras = { 
                                     new SqlParameter("@TeamID",teamid)
                                   };

            return GetDataTable(sqlText, paras, CommandType.Text);
        }

        public DataSet GetWareHouses(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@keyWords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@ClientID",clientID)
                                       
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetWareHouses", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;

        }

        public DataSet GetWareHouses(string clientID)
        {
            SqlParameter[] paras = { new SqlParameter("@ClientID", clientID) };
            DataSet ds = GetDataSet("select * from WareHouse where Status<>9 and ClientID=@ClientID ; select * from DepotSeat where Status<>9 and ClientID=@ClientID", paras, CommandType.Text);
            return ds;
        }

        public DataSet GetWareByID(string wareid)
        {
            SqlParameter[] paras = { new SqlParameter("@WareID", wareid) };
            DataSet ds = GetDataSet("select * from WareHouse where WareID=@WareID; select * from DepotSeat where Status<>9 and WareID=@WareID", paras, CommandType.Text);
            return ds;
        }

        public DataSet GetDepotSeats(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID, string wareid = "")
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@keyWords",keyWords),
                                       new SqlParameter("@pageSize",pageSize),
                                       new SqlParameter("@pageIndex",pageIndex),
                                       new SqlParameter("@ClientID",clientID),
                                       new SqlParameter("@WareID",wareid)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_GetDepotSeats", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;

        }

        public DataTable GetDepotByID(string depotid)
        {
            SqlParameter[] paras = { new SqlParameter("@DepotID", depotid) };
            DataTable dt = GetDataTable("select * from DepotSeat where DepotID=@DepotID", paras, CommandType.Text);
            return dt;
        }

        public DataTable GetDepotSeatsByWareID(string wareid)
        {
            SqlParameter[] paras = { new SqlParameter("@WareID", wareid) };
            DataTable dt = GetDataTable("select * from DepotSeat where WareID=@WareID and Status<>9 ", paras, CommandType.Text);
            return dt;
        }

        public DataSet GetIntergeFeeList(int changetype, string customerid, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID, string beginTime, string endTime, string agentid = "")
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@totalCount",SqlDbType.Int),
                                       new SqlParameter("@pageCount",SqlDbType.Int),
                                       new SqlParameter("@ChangFeeType",changetype),
                                       new SqlParameter("@CustomerID",customerid),
                                       new SqlParameter("@AgentID",agentid),
                                       new SqlParameter("@BeginTime",beginTime),
                                       new SqlParameter("@EndTime",endTime),
                                       new SqlParameter("@PageSize",pageSize),
                                       new SqlParameter("@PageIndex",pageIndex),
                                       new SqlParameter("@ClientID",clientID)
                                   };
            paras[0].Value = totalCount;
            paras[1].Value = pageCount;

            paras[0].Direction = ParameterDirection.InputOutput;
            paras[1].Direction = ParameterDirection.InputOutput;
            DataSet ds = GetDataSet("P_IntergeFeeChangePageList", paras, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(paras[0].Value);
            pageCount = Convert.ToInt32(paras[1].Value);
            return ds;

        }
        #endregion

        #region 添加

        public bool CreateCustomSource(string sourceid, string sourcecode, string sourcename, int ischoose, string userid, string clientid,out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",result),
                                     new SqlParameter("@SourceID",sourceid),
                                     new SqlParameter("@SourceName",sourcename),
                                     new SqlParameter("@SourceCode",sourcecode),
                                     new SqlParameter("@IsChoose",ischoose),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            bool bl = ExecuteNonQuery("P_InsertCustomSource", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            return bl;
        }

        public bool CreateStageItem(string itemid, string name, string stageid, string userid, string clientid)
        {
            string sqlText = "insert into StageItem(ItemID,ItemName,StageID,CreateUserID,ClientID) " +
                                           " values(@ItemID,@ItemName,@StageID,@CreateUserID,@ClientID) ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ItemID" , itemid),
                                     new SqlParameter("@ItemName" , name),
                                     new SqlParameter("@StageID" , stageid),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool CreateOpportunityStage(string stageid, string stagename, decimal probability, int sort, string userid, string clientid, out int result)
        {
            result = 0;
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",result),
                                     new SqlParameter("@StageID" , stageid),
                                     new SqlParameter("@StageName" , stagename),
                                     new SqlParameter("@Probability" , probability),
                                     new SqlParameter("@Sort",sort),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            bool bl = ExecuteNonQuery("P_CreateOpportunityStage", paras, CommandType.StoredProcedure) > 0;
            result = Convert.ToInt32(paras[0].Value);
            return bl;
        }

        public bool CreateOrderType(string typeid, string typename, string typecode, string userid, string clientid)
        {
            string sqlText = "insert into OrderType(TypeID,TypeName,TypeCode,CreateUserID,ClientID) " +
                                           " values(@TypeID,@TypeName,@TypeCode,@CreateUserID,@ClientID) ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@TypeID" , typeid),
                                     new SqlParameter("@TypeName" , typename),
                                     new SqlParameter("@TypeCode" , typecode),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool CreateTeam(string teamid, string teamname, string userid, string agentid,string clientid)
        {
            string sqlText = "insert into Teams(TeamID,TeamName,AgentID,CreateUserID,ClientID) " +
                                           " values(@TeamID,@TeamName,@AgentID,@CreateUserID,@ClientID) ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@TeamID" , teamid),
                                     new SqlParameter("@TeamName" , teamname),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool AddWareHouse(string id, string warecode, string name, string shortname, string citycode, int status, string depotid, string depotcode, string depotname, string description, string operateid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@WareID" , id),
                                     new SqlParameter("@WareCode" , warecode),
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@ShortName" , shortname),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@DepotID" , depotid),
                                     new SqlParameter("@DepotCode" , depotcode),
                                     new SqlParameter("@DepotName" , depotname),
                                     new SqlParameter("@Description" , description),
                                     new SqlParameter("@CreateUserID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery("P_CreateWareHouse", paras, CommandType.StoredProcedure) > 0;
        }

        public bool AddDepotSeat(string id, string depotcode, string wareid, string name, int status, string description, string operateid, string clientid)
        {
            string sqlText = "insert into DepotSeat(DepotID,DepotCode,WareID,Name,Status,Description,CreateUserID,ClientID) " +
                                            " values(@DepotID,@DepotCode,@WareID,@Name,@Status,@Description,@CreateUserID,@ClientID) ";
            SqlParameter[] paras = { 
                                    
                                     new SqlParameter("@DepotID" , id),
                                     new SqlParameter("@DepotCode" , depotcode),
                                     new SqlParameter("@WareID" , wareid),
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@Description" , description),
                                     new SqlParameter("@CreateUserID" , operateid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool InsertClientIndustry(string clientindustryid, string name, string clientid, string agentid,
            string userid, string description, int status = 1)
        {
            string sqlText = "insert into ClientsIndustry(ClientIndustryID,Name,ClientID,AgentID,Description,Status,CreateUserID) " +
                                           " values(@ClientIndustryID,@Name,@ClientID,@AgentID,@Description,@Status,@CreateUserID) ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ClientIndustryID" , clientindustryid),
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@ClientID" , clientid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@Description" , description),
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@CreateUserID" , userid)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }
        public string InsertClientMemberLevel(string levelid, string name, string clientid, string agentid,
         string userid, decimal discountfee, decimal integfeemore,  int origin, int status = 1, string imgurl="")
        {
           SqlParameter[] paras = { 
                                     new SqlParameter("@result" , SqlDbType.VarChar,300),
                                     new SqlParameter("@LevelID" , levelid),
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@ClientID" , clientid),
                                     new SqlParameter("@AgentID" , agentid),
                                     new SqlParameter("@DiscountFee" , discountfee),
                                     new SqlParameter("@IntegFeeMore" , integfeemore),
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@CreateUserID" , userid),
                                      new SqlParameter("@ImgUrl",imgurl), 
                                    
                                   };
            paras[0].Direction = ParameterDirection.Output;
            origin= ExecuteNonQuery("P_InsertClientMemberLevel", paras, CommandType.StoredProcedure);
            return Convert.ToString(paras[0].Value);
        }

        public bool InsertIntergeFeeChange(int changetype, decimal changefee, string customerid, string remark, string userid, string agentid, string clientid)
        { 
            SqlParameter[] paras = {  
                                     new SqlParameter("@ChangFeeType",changetype),
                                     new SqlParameter("@ChangeFee",changefee),
                                     new SqlParameter("@CustomerID",customerid),
                                     new SqlParameter("@AgentID",agentid),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@ClientID" , clientid),
                                     new SqlParameter("@Reamrk" , remark)
                                   };
            bool bl = ExecuteNonQuery("P_InsertIntergeFeeChange", paras, CommandType.StoredProcedure) > 0;
 
            return bl;
        }

        #endregion

        #region 编辑/删除

        public bool UpdateCustomSource(string sourceid, string sourcename, int ischoose, string clientid)
        {
            string sqltext = "update CustomSource set SourceName=@SourceName,IsChoose=@IsChoose where SourceID=@SourceID and clientid=@ClientID";

            SqlParameter[] paras = { 
                                     new SqlParameter("@SourceID",sourceid),
                                     new SqlParameter("@SourceName",sourcename),
                                     new SqlParameter("@IsChoose",ischoose),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            bool bl = ExecuteNonQuery(sqltext, paras, CommandType.Text) > 0;
            return bl;
        }

        public bool DeleteCustomSource(string sourceid, string clientid)
        {
            string sqltext = "update CustomSource set Status=9 where SourceID=@SourceID and clientid=@ClientID";

            SqlParameter[] paras = { 
                                     new SqlParameter("@SourceID",sourceid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            bool bl = ExecuteNonQuery(sqltext, paras, CommandType.Text) > 0;
            return bl;
        }

        public bool UpdateClientIndustry(string clientid, string clientindustryid, string name, string desc)
        {
            string sqltext = "update ClientsIndustry set Name=@Name,Description=@Desc where ClientIndustryID=@ClientIndustryID and ClientID=@ClientID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@ClientIndustryID",clientindustryid),
                                     new SqlParameter("@Name",name), 
                                     new SqlParameter("@Desc",desc), 
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            bool bl = ExecuteNonQuery(sqltext, paras, CommandType.Text) > 0;
            return bl;
        }
        public bool DeleteClientIndustry(string clientid, string clientindustryid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@ClientIndustryID",clientindustryid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            bool bl = ExecuteNonQuery("P_DeleteClientIndustry", paras, CommandType.StoredProcedure) > 0;
            return bl;
        }
        public string UpdateClientMemberLevel(string clientid, string levelid, string name, decimal discountfee, decimal integfeemore, string imgurl="")
        {  
            SqlParameter[] paras = { 
                                     new SqlParameter("@result" , SqlDbType.VarChar,300),
                                     new SqlParameter("@LevelID",levelid),
                                     new SqlParameter("@Name",name), 
                                     new SqlParameter("@DiscountFee" , discountfee),
                                     new SqlParameter("@IntegFeeMore" , integfeemore),
                                     new SqlParameter("@ClientID" , clientid),
                                     new SqlParameter("@ImgUrl",imgurl), 
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_UpdateClientMemberLevel", paras, CommandType.StoredProcedure);
            return paras[0].Value.ToString(); ;
        }
        public string DeleteClientMemberLevel(string clientid, string levelid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@result" , SqlDbType.VarChar,300),
                                     new SqlParameter("@LevelID",levelid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_DeleteClientMemberLevel", paras, CommandType.StoredProcedure);
            return paras[0].Value.ToString();
        } 

        public bool UpdateCustomStage(string stageid, string stagename, string clientid)
        {
            string sqltext = "update CustomStage set StageName=@StageName where StageID=@StageID and ClientID=@ClientID";

            SqlParameter[] paras = { 
                                     new SqlParameter("@StageID",stageid),
                                     new SqlParameter("@StageName",stagename),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            bool bl = ExecuteNonQuery(sqltext, paras, CommandType.Text) > 0;
            return bl;
        }

        public bool UpdateOpportunityStage(string stageid, string stagename, decimal probability, string clientid)
        {
            string sqltext = "update OpportunityStage set StageName=@StageName,Probability=@Probability where StageID=@StageID and ClientID=@ClientID";

            SqlParameter[] paras = { 
                                     new SqlParameter("@StageID",stageid),
                                     new SqlParameter("@Probability",probability),
                                     new SqlParameter("@StageName",stagename),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            bool bl = ExecuteNonQuery(sqltext, paras, CommandType.Text) > 0;
            return bl;
        }

        public bool DeleteOpportunityStage(string stageid, string userid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@StageID",stageid),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            bool bl = ExecuteNonQuery("P_DeleteOpportunityStage", paras, CommandType.StoredProcedure) > 0;
            return bl;
        }

        public bool UpdateOrderType(string typeid, string typename, string typecode, string clientid)
        {
            string sqltext = "update OrderType set TypeName=@TypeName,TypeCode=@TypeCode where TypeID=@TypeID and ClientID=@ClientID";

            SqlParameter[] paras = { 
                                     new SqlParameter("@TypeID",typeid),
                                     new SqlParameter("@TypeName",typename),
                                     new SqlParameter("@TypeCode",typecode),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            bool bl = ExecuteNonQuery(sqltext, paras, CommandType.Text) > 0;
            return bl;
        }

        public bool DeleteOrderType(string typeid, string clientid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@TypeID",typeid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            bool bl = ExecuteNonQuery("P_DeleteOrderType", paras, CommandType.StoredProcedure) > 0;
            return bl;
        }

        public bool DeleteTeam(string teamid, string userid, string agentid)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@TeamID",teamid),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@AgentID" , agentid)
                                   };
            bool bl = ExecuteNonQuery("P_DeleteTeam", paras, CommandType.StoredProcedure) > 0;
            return bl;
        }

        public bool UpdateUserTeamID(string userid, string teamid, string operateid, string agentid)
        {
            SqlParameter[] paras = { 
                                         new SqlParameter("@UserID",userid),
                                         new SqlParameter("@TeamID",teamid),
                                         new SqlParameter("@AgentID" , agentid),
                                         new SqlParameter("@OperateID" , operateid)
                                   };
            bool bl = ExecuteNonQuery("P_UpdateUserTeamID", paras, CommandType.StoredProcedure) > 0;
            return bl;
        }

        public bool UpdateWareHouse(string id, string code, string name, string shortname, string citycode, int status, string description)
        {
            string sqlText = "Update WareHouse set Name=@Name,WareCode=@WareCode,ShortName=@ShortName,CityCode=@CityCode,Status=@Status,Description=@Description,UpdateTime=getdate() " +
                            "  where WareID=@WareID ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@WareID" , id),
                                     new SqlParameter("@WareCode" , code),
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@ShortName" , shortname),
                                     new SqlParameter("@CityCode" , citycode),
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@Description" , description)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool DeleteWareHouse(string wareid, string userid, string clientid, out int result)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",SqlDbType.Int),
                                     new SqlParameter("@WareID",wareid),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_DeleteWareHouse", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        public bool UpdateDepotSeat(string id, string depotcode, string name, int status, string description)
        {
            string sqlText = "Update DepotSeat set DepotCode=@DepotCode,Name=@Name,Status=@Status,Description=@Description,UpdateTime=getdate() " +
                            "  where DepotID=@DepotID ";
            SqlParameter[] paras = { 
                                     new SqlParameter("@DepotID" , id),
                                     new SqlParameter("@DepotCode" , depotcode),
                                     new SqlParameter("@Name" , name),
                                     new SqlParameter("@Status" , status),
                                     new SqlParameter("@Description" , description)
                                   };
            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        public bool DeleteDepotSeat(string depotid, string userid, string clientid, out int result)
        {
            SqlParameter[] paras = { 
                                     new SqlParameter("@Result",SqlDbType.Int),
                                     new SqlParameter("@DepotID",depotid),
                                     new SqlParameter("@UserID",userid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };
            paras[0].Direction = ParameterDirection.Output;
            ExecuteNonQuery("P_DeleteDepotSeat", paras, CommandType.StoredProcedure);
            result = Convert.ToInt32(paras[0].Value);
            return result == 1;
        }

        #endregion
    }
}
