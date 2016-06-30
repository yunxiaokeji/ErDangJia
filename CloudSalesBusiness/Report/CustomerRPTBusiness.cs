using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesEntity;
using System.Data;

using CloudSalesDAL;
using CloudSalesEnum;

namespace CloudSalesBusiness
{
    public class CustomerRPTBusiness
    {
        public static CustomerRPTBusiness BaseBusiness = new CustomerRPTBusiness();

        public List<SourceScaleEntity> GetCustomerSourceScale(string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            List<SourceScaleEntity> list = new List<SourceScaleEntity>();
            int total = 0;
            DataTable dt = CustomerRPTDAL.BaseProvider.GetCustomerSourceScale(begintime, endtime, UserID, TeamID, agentid, clientid);
            foreach (DataRow dr in dt.Rows)
            {
                SourceScaleEntity model = new SourceScaleEntity();
                model.FillData(dr);
                total += model.Value;
                list.Add(model);
            }
            foreach (var model in list)
            {
                if (total > 0)
                {
                    model.Scale = (Convert.ToDecimal(model.Value) / total * 100).ToString("f2") + "%";
                }
                else
                {
                    model.Scale = "0.00%";
                }
            }
            return list;
        }

        public List<SourceDateEntity> GetCustomerSourceDate(EnumDateType type, string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            List<SourceDateEntity> list = new List<SourceDateEntity>();

            DataSet ds = CustomerRPTDAL.BaseProvider.GetCustomerSourceDate((int)type, begintime, endtime, UserID, TeamID, agentid, clientid);
            var sources = SystemBusiness.BaseBusiness.GetCustomSources(agentid, clientid);
            foreach (var source in sources)
            {
                SourceDateEntity model = new SourceDateEntity();
                model.Name = source.SourceName;
                model.Items = new List<SourceDateItem>();
                DataRow[] drs = ds.Tables["SourceData"].Select("SourceID='" + source.SourceID + "'");
                foreach (DataRow dr in ds.Tables["DateName"].Rows)
                {
                    SourceDateItem item = new SourceDateItem();
                    item.Name = dr[0].ToString();
                    if (drs.Where(m => m["Name"].ToString() == item.Name).Count() > 0)
                    {
                        item.Value = Convert.ToInt32(drs.Where(m => m["Name"].ToString() == item.Name).FirstOrDefault()["Value"]);
                    }
                    else
                    {
                        item.Value = 0;
                    }
                    model.Items.Add(item);
                }
                list.Add(model);
            }

            return list;
        }

        public List<ReportCommonEntity> GetCustomerStageRate(string begintime, string endtime, int type, string clientid,string ownerid)
        {
            List<ReportCommonEntity> list = new List<ReportCommonEntity>();
            DataSet ds = CustomerRPTDAL.BaseProvider.GetCustomerStageRPT(begintime, endtime, type, clientid, ownerid);
           

            int[] stages = { 1, 2, 3 };
            int total = 0;
            foreach (var stage in stages)
            {
                ReportCommonEntity model = new ReportCommonEntity();
                List<SourceItem> sourceItems = new List<SourceItem>();
                model.name = stage == 1 ? "新客户" : stage == 2 ? "机会客户" : "成交客户";
                if (ds.Tables["Data"].Select("StageStatus=" + stage).Count() > 0)
                {
                    model.desc += model.name + ":" + ds.Tables["Data"].Select("StageStatus=" + stage)[0]["Value"].ToString();
                    model.iValue = Convert.ToInt32(ds.Tables["Data"].Select("StageStatus=" + stage)[0]["Value"]);
                }
                else 
                {
                    model.iValue = 0;
                    model.desc = "";
                }
                DataRow[] drRow = ds.Tables["Source"].Select("StageStatus=" + stage);
                if (stage == 2)
                { 
                    foreach (var source in SystemBusiness.BaseBusiness.GetOpportunityStages("", clientid))
                    {
                        SourceItem item = new SourceItem();
                        item.Name = source.StageName;
                        item.Value = 0;
                        item.value = "0.00";
                        item.pvalue = "0.00";
                        item.cvalue = "0.00";
                        item.desc = "";
                        int[] oppstatus = { 1, 2, 3 };
                        foreach (var opstatus in oppstatus)
                        {
                            if (drRow.Any())
                            {
                                DataRow[] row =
                                    drRow.Where(x => (x["SourceID"].ToString().ToLower() == source.StageID || string.IsNullOrEmpty(x["SourceID"].ToString())) && Convert.ToInt32(x["SourceName"]) == opstatus).ToArray();
                                if (row.Any() && row.Length > 0)
                                {
                                    item.Value += Convert.ToInt32(row[0]["value"]);
                                    item.desc = item.desc + (opstatus == 1 ? "正常" : opstatus == 2 ? "已成单" : "已关闭") + "<br/>" + row[0]["value"].ToString() + "<br/>";
                                }
                                else
                                {
                                    item.desc = item.desc + (opstatus == 1 ? "正常" : opstatus == 2 ? "已成单" : "已关闭") +
                                                "<br/>0 <br/>";
                                }
                            }
                            else
                            {
                                item.desc = item.desc + (opstatus == 1 ? "正常" : opstatus == 2 ? "已成单" : "已关闭") +
                                            "<br/>0 <br/>";
                            }
                        }
                        item.cvalue =
                                 (Convert.ToDecimal(item.Value) / (model.iValue == 0 ? 1 : model.iValue) * 100).ToString(
                                     "f2");
                        if (list.Count > 0)
                        {
                            int value = list[0].iValue;
                            item.value =
                                (Convert.ToDecimal(item.Value) / (value == 0 ? 1 : model.iValue) * 100).ToString(
                                    "f2");
                        }
                        sourceItems.Add(item);
                    }
                }
                else if (stage == 1)
                {
                    total = model.iValue;
                    if (type == 1)
                    {
                        model.dValue = CustomerRPTDAL.BaseProvider.GetCustomerCountByTime(begintime, endtime, clientid, ownerid);
                        model.desc = model.name + ":" +  model.dValue;
                    }
                    foreach (var source in SystemBusiness.BaseBusiness.GetCustomSources("", clientid))
                    {
                        SourceItem item = new SourceItem();
                        item.Name = source.SourceName;
                        item.Value = 0;
                        item.value = "0.00";
                        item.pvalue = "0.00";
                        item.cvalue = "0.00";
                        if (drRow.Any())
                        {
                            DataRow[] row = drRow.Where(x => x["SourceID"].ToString().ToLower() == source.SourceID).ToArray();
                            if (row.Any() && row.Length > 0)
                            {
                                item.Value = Convert.ToInt32(row[0]["value"]);
                            }
                            item.cvalue = (Convert.ToDecimal(item.Value) / (model.iValue == 0 ? 1 : model.iValue) * 100).ToString("f2");
                        }
                        sourceItems.Add(item);
                    } 
                }
                model.sourceItem = sourceItems;
                list.Add(model);
            }
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (total > 0)
                {
                    list[i].value = (Convert.ToDecimal(list[i].iValue) / total * 100).ToString("f2");
                }
                else 
                {
                    list[i].value = "0.00";
                }
                list[i].name += list[i].iValue;
                if (list[i].desc.Length > 0)
                {
                    list[i].name += " (" + list[i].desc + ") ";
                }
            } 
            return list;
        }
         
        /// <summary>
        /// 获取客户分布统计
        /// </summary>
        /// <param name="type">1:按地区；2、按行业；3、按规模</param>
        public List<DateJson> GetCustomerReport(int type, string begintime, string endtime, string UserID, string TeamID, string agentid, string clientid)
        {
            List<DateJson> list = new List<DateJson>();

            DataTable dt = CustomerRPTDAL.BaseProvider.GetCustomerReport(type, begintime, endtime, UserID, TeamID, agentid, clientid);
            foreach (DataRow dr in dt.Rows)
            {
                DateJson model = new DateJson();
                model.name = dr["name"].ToString();
                model.value = int.Parse(dr["value"].ToString());

                list.Add(model);
            }

            if (type == 1)
            {
                if (CommonBusiness.Citys != null)
                {
                    List<CityEntity> provinces = CommonBusiness.Citys.FindAll(m => m.Level == 1);
                    foreach (CityEntity c in provinces)
                    {
                        DateJson item = list.Find(m => m.name == c.Name);
                        if (item == null)
                        {
                            DateJson model = new DateJson();
                            model.name = c.Name.Replace("市", "").Replace("省", "").Replace("特别行政区", "").Replace("壮族自治区", "").Replace("回族自治区", "").Replace("维吾尔自治区", "").Replace("自治区", "");
                            model.value = 0;
                            list.Add(model);
                        }
                        else
                        {
                            item.name = c.Name.Replace("市", "").Replace("省", "").Replace("特别行政区", "").Replace("壮族自治区", "").Replace("回族自治区", "").Replace("维吾尔自治区", "").Replace("自治区", "");
                        }
                    }
                }
            }





            return list;
        }

        public List<StageCustomerEntity> GetUserCustomers(string userid, string teamid, string begintime, string endtime, string agentid, string clientid)
        {
            List<StageCustomerEntity> list = new List<StageCustomerEntity>();

            DataSet ds = CustomerRPTDAL.BaseProvider.GetUserCustomers(userid, teamid, begintime, endtime, agentid, clientid);

            DataTable dt = ds.Tables["Users"];

            int[] stages = {1, 2, 3};



            if (!string.IsNullOrEmpty(userid))
            {
                #region 统计个人
               
                StageCustomerEntity model = new StageCustomerEntity();
                var usertemp = OrganizationBusiness.GetUserByUserID(userid, agentid);
                model.Name = usertemp.Name;
                var team = SystemBusiness.BaseBusiness.GetTeamByID(usertemp.TeamID, agentid);
                model.Stages = new List<StageCustomerItem>();
                model.PID = usertemp.TeamID;
                model.PName = team==null ?"暂无团队":team.TeamName;
                model.SCSRNum = 0;
                model.TotalNum = 0;
                model.OCSRNum = 0;
                model.NCSRNum = 0;
                foreach (var stage in stages)
                {
                    StageCustomerItem item = new StageCustomerItem();
                    item.Name = stage == 1 ? "客户" : stage == 2 ? "机会客户" : "成交客户";
                    var drs = dt.Select("StageStatus='" + stage + "'");  
                    if (drs.Count() > 0)
                    {
                        item.Count = Convert.ToInt32(drs[0]["Value"]);
                    } else {
                        item.Count = 0;
                    }
                    model.SCSRNum += (stage == 3 ? item.Count : 0);
                    model.OCSRNum += (stage == 2 ? item.Count : 0);
                    model.NCSRNum += (stage == 1 ? item.Count : 0);
                    model.TotalNum += item.Count;
                    model.Stages.Add(item);
                }
                list.Add(model);

                #endregion
            }
            else if (!string.IsNullOrEmpty(teamid))
            {
                #region 统计团队
                var team = SystemBusiness.BaseBusiness.GetTeamByID(teamid, agentid);
                StageCustomerEntity model = new StageCustomerEntity();

                model.Name = team.TeamName;
                model.GUID = team.TeamID;

                model.Stages = new List<StageCustomerItem>();
                model.ChildItems = new List<StageCustomerEntity>();
                if (team.Users.Count == 0)
                {
                    StageCustomerEntity childModel = new StageCustomerEntity();
                    childModel.GUID = "";
                    childModel.Name = "";
                    childModel.PID = team.TeamID;
                    childModel.PName = team.TeamName;
                    childModel.Stages = new List<StageCustomerItem>();
                    childModel.SCSRNum = 0;
                    childModel.OCSRNum = 0;
                    childModel.NCSRNum = 0;
                    childModel.TotalNum = 0;
                    foreach (var stage in stages)
                    {
                        StageCustomerItem childItem = new StageCustomerItem();
                        var stageName = stage == 1 ? "客户" : stage == 2 ? "机会客户" : "成交客户";
                        childItem.Name = stageName;
                        childItem.Count = 0;
                        childModel.Stages.Add(childItem);
                        StageCustomerItem item = new StageCustomerItem();
                        item.Name = stageName;
                        item.StageID = stage.ToString();
                        item.Count = childItem.Count;
                        model.Stages.Add(item);
                    }
                    model.ChildItems.Add(childModel);
                }
                //遍历成员
                foreach (var user in team.Users)
                {
                    StageCustomerEntity childModel = new StageCustomerEntity();
                    childModel.GUID = user.UserID;
                    childModel.PName = team.TeamName;
                    childModel.Name = user.Name;
                    childModel.PID = team.TeamID;
                    childModel.SCSRNum = 0;
                    childModel.OCSRNum = 0;
                    childModel.NCSRNum = 0;
                    childModel.TotalNum = 0;
                    childModel.Stages = new List<StageCustomerItem>();
                    //遍历阶段
                    foreach (var stage in stages)
                    {
                        StageCustomerItem childItem = new StageCustomerItem();
                        var stageName = stage == 1 ? "客户" : stage == 2 ? "机会客户" : "成交客户";
                        childItem.Name = stageName;

                        var drs = dt.Select("StageStatus='" + stage + "' and OwnerID='" + user.UserID + "'");
                        if (drs.Count() > 0)
                        {
                            childItem.Count = Convert.ToInt32(drs[0]["Value"]);
                        }
                        else
                        {
                            childItem.Count = 0;
                        }
                        childModel.SCSRNum += (stage == 3 ? childItem.Count : 0);
                        childModel.OCSRNum += (stage == 2 ? childItem.Count : 0);
                        childModel.NCSRNum += (stage == 1 ? childItem.Count : 0);
                        childModel.TotalNum += childItem.Count;
                        if (model.Stages.Where(m => m.StageID == stage.ToString()).Count() > 0)
                        {
                            model.Stages.Where(m => m.StageID == stage.ToString()).FirstOrDefault().Count += childItem.Count;
                        }
                        else 
                        {
                            StageCustomerItem item = new StageCustomerItem();
                            item.Name = stageName;
                            item.StageID = stage.ToString();
                            item.Count = childItem.Count;
                            model.Stages.Add(item);
                        }

                        childModel.Stages.Add(childItem);
                    }
                    model.ChildItems.Add(childModel);
                }
                model.NCSRNum = model.ChildItems.Sum(x => x.NCSRNum);
                model.OCSRNum = model.ChildItems.Sum(x => x.OCSRNum);
                model.TotalNum = model.ChildItems.Sum(x => x.TotalNum);
                model.SCSRNum = model.ChildItems.Sum(x => x.SCSRNum);
                list.Add(model);

                #endregion
            }
            else
            {
                #region 统计所有
                var teams = SystemBusiness.BaseBusiness.GetTeams(agentid);
                foreach (var team in teams)
                {
                    StageCustomerEntity model = new StageCustomerEntity();

                    model.Name = team.TeamName;
                    model.GUID = team.TeamID;

                    model.Stages = new List<StageCustomerItem>();
                    model.ChildItems = new List<StageCustomerEntity>();
                    if (team.Users.Count == 0)
                    {
                        StageCustomerEntity childModel = new StageCustomerEntity();
                        childModel.GUID = "";
                        childModel.Name ="";
                        childModel.PID = team.TeamID;
                        childModel.PName = team.TeamName;
                        childModel.Stages = new List<StageCustomerItem>();
                        childModel.SCSRNum = 0;
                        childModel.OCSRNum = 0;
                        childModel.NCSRNum = 0;
                        childModel.TotalNum = 0;
                        foreach (var stage in stages)
                        {
                            StageCustomerItem childItem = new StageCustomerItem();
                            var stageName = stage == 1 ? "客户" : stage == 2 ? "机会客户" : "成交客户";
                            childItem.Name = stageName;
                            childItem.Count = 0;
                            childModel.Stages.Add(childItem);
                            StageCustomerItem item = new StageCustomerItem();
                            item.Name = stageName;
                            item.StageID = stage.ToString();
                            item.Count = childItem.Count;
                            model.Stages.Add(item);
                        }
                        model.ChildItems.Add(childModel);
                    }
                    //遍历成员
                    foreach (var user in team.Users)
                    {
                        StageCustomerEntity childModel = new StageCustomerEntity();
                        childModel.GUID = user.UserID;
                        childModel.Name = user.Name;
                        childModel.PID = team.TeamID;
                        childModel.PName = team.TeamName;
                        childModel.Stages = new List<StageCustomerItem>();
                        childModel.SCSRNum = 0;
                        childModel.OCSRNum = 0;
                        childModel.NCSRNum = 0;
                        childModel.TotalNum = 0;
                        //遍历阶段
                        foreach (var stage in stages)
                        {
                            StageCustomerItem childItem = new StageCustomerItem();
                            var stageName = stage == 1 ? "客户" : stage == 2 ? "机会客户" : "成交客户";
                            childItem.Name = stageName;

                            var drs = dt.Select("StageStatus=" + stage + " and OwnerID='" + user.UserID + "'");
                            if (drs.Count() > 0)
                            {
                                childItem.Count = Convert.ToInt32(drs[0]["Value"]);
                            }
                            else
                            {
                                childItem.Count = 0;
                            }
                            childModel.SCSRNum += (stage == 3 ? childItem.Count : 0);
                            childModel.OCSRNum += (stage == 2 ? childItem.Count : 0);
                            childModel.NCSRNum += (stage == 1 ? childItem.Count : 0);
                            childModel.TotalNum += childItem.Count;
                            if (model.Stages.Where(m => m.StageID == stage.ToString()).Count() > 0)
                            {
                                model.Stages.Where(m => m.StageID == stage.ToString()).FirstOrDefault().Count += childItem.Count;
                            }
                            else
                            {
                                StageCustomerItem item = new StageCustomerItem();
                                item.Name = stageName;
                                item.StageID = stage.ToString();
                                item.Count = childItem.Count;
                                model.Stages.Add(item);
                            }

                            childModel.Stages.Add(childItem);
                        }
                        model.ChildItems.Add(childModel);
                    }
                    model.TotalNum = model.ChildItems.Sum(x => x.TotalNum);
                    model.SCSRNum = model.ChildItems.Sum(x => x.SCSRNum);
                    model.NCSRNum = model.ChildItems.Sum(x => x.NCSRNum);
                    model.OCSRNum = model.ChildItems.Sum(x => x.OCSRNum);
                    list.Add(model);
                }

                #endregion
            }
            

            return list;
        }

    }
}
