﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesEntity;
using CloudSalesDAL.Agents;
using System.Data;
namespace CloudSalesBusiness
{
    public class AgentsBusiness
    {
        #region Cache
        private static Dictionary<string, Agents> _cacheAgents;

        /// <summary>
        /// 缓存代理商
        /// </summary>
        private static Dictionary<string, Agents> Agents
        {
            get
            {
                if (_cacheAgents == null)
                {
                    _cacheAgents = new Dictionary<string, Agents>();
                }
                return _cacheAgents;
            }
            set
            {
                _cacheAgents = value;
            }
        }
        #endregion

        #region  查

        /// <summary>
        /// 是否明道网络已注册
        /// </summary>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public static bool IsExistsMDProject(string projectid)
        {
            var count = CommonBusiness.Select("Agents", "count(0)", "MDProjectID='" + projectid + "'");
            return Convert.ToInt32(count) > 0;
        }


        /// <summary>
        /// 获取代理商详情
        /// </summary>
        /// <param name="agentid"></param>
        /// <returns></returns>
        public static Agents GetAgentDetail(string agentid)
        {
            if (!Agents.ContainsKey(agentid))
            {
                DataTable dt = AgentsDAL.BaseProvider.GetAgentDetail(agentid);
                Agents model = new Agents();
                if (dt.Rows.Count == 1)
                {
                    DataRow row = dt.Rows[0];
                    model.FillData(row);

                    if (!Agents.ContainsKey(model.AgentID))
                    {
                        Agents.Add(model.AgentID, model);
                    }
                    return Agents[agentid];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return Agents[agentid];
            }
            
        }

        public static List<Report_AgentAction_Day> GetAgentActionReport(string keyword,string startDate,string endDate) 
        {
            DataTable dt = AgentsDAL.BaseProvider.GetAgentActionReport(keyword,startDate,endDate);
            List<Report_AgentAction_Day> list = new List<Report_AgentAction_Day>();
            Report_AgentAction_Day model =null;

            if (dt.Rows.Count>0)
            {
                foreach (DataRow dr in dt.Rows) 
                {
                    model = new Report_AgentAction_Day();
                    model.FillData(dr);
                    list.Add(model);
                }
                
            }

            return list;
        }

        #endregion

        #region  编辑
        /// <summary>
        /// 更新代理商缓存
        /// </summary>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public static bool UpdatetAgentCache(string agentID)
        {
            if (Agents.ContainsKey(agentID))
            {
                DataTable dt = AgentsDAL.BaseProvider.GetAgentDetail(agentID);
                Agents model = new Agents();
                if (dt.Rows.Count == 1)
                {
                    DataRow row = dt.Rows[0];
                    model.FillData(row);

                    Agents[agentID] = model;
                }
                else
                    return false;
            }

            return true;
        }

        // <summary>
        /// 代理商授权
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="userQuantity"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static bool ClientAgentAuthorize(string agentID, int userQuantity, DateTime endTime)
        {
            bool flag= AgentsDAL.BaseProvider.ClientAgentAuthorize(agentID, userQuantity, endTime);

            if (flag) {
                if (Agents.ContainsKey(agentID))
                {
                    Agents agent = Agents[agentID];
                    agent.UserQuantity = userQuantity;
                    agent.EndTime = endTime;
                }
            }

            return flag;
        }

        /// <summary>
        /// 代理商新增用户人数
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static bool AddClientAgentUserQuantity(string agentID, int quantity)
        {
            bool flag= AgentsDAL.BaseProvider.AddClientAgentUserQuantity(agentID, quantity);

            if (flag)
            {
                if (Agents.ContainsKey(agentID))
                {
                    Agents agent = Agents[agentID];
                    agent.UserQuantity += quantity;
                }
            }

            return flag;
        }

        /// <summary>
        /// 代理商新增截至时间
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static bool SetClientAgentEndTime(string agentID, DateTime endTime)
        {
            bool flag= AgentsDAL.BaseProvider.SetClientAgentEndTime(agentID, endTime);

            if (flag)
            {
                if (Agents.ContainsKey(agentID))
                {
                    Agents agent = Agents[agentID];
                    agent.EndTime = endTime;
                }
            }

            return flag;
        }

        /// <summary>
        /// 更新代理商密钥
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool UpdateAgentKey(string agentID, string key)
        {
            bool flag = AgentsDAL.BaseProvider.UpdateAgentKey(agentID, key);

            if (flag)
            {
                if (Agents.ContainsKey(agentID))
                {
                    Agents agent = Agents[agentID];
                    agent.AgentKey = key;
                }
            }

            return flag;
        }
        #endregion
    }
}
