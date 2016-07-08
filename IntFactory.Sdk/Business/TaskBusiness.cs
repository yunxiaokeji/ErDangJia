using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace IntFactory.Sdk
{
    public class TaskBusiness
    {
        public static TaskBusiness BaseBusiness = new TaskBusiness();

        #region get
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public TaskListResult GetTasks(FilterTasks filter, string userID, string agentID)
        { 
            var paras = new Dictionary<string, object>();
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);
            paras.Add("filter",JsonConvert.SerializeObject(filter).ToString());

            return HttpRequest.RequestServer <TaskListResult>(ApiOption.GetTasks, paras);
        }

        /// <summary>
        /// 获取订单流程列表
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public ProcessListResult GetOrderProcess(string userID, string agentID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);



            return HttpRequest.RequestServer<ProcessListResult>(ApiOption.GetOrderProcess, paras);
        }

        /// <summary>
        /// 获取订单流程阶段列表
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public ProcessStageListResult GetOrderStages(string processID, string userID, string agentID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("processID", processID);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<ProcessStageListResult>(ApiOption.GetOrderStages, paras);
        }

        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public TaskDetailResult GetTaskDetail(string taskID, string userID, string agentID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("taskID", taskID);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<TaskDetailResult>(ApiOption.GetTaskDetail, paras);
        }

        /// <summary>
        /// 获取任务讨论列表
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="stageID"></param>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public TaskReplyListResult GetTaskReplys(string orderID, string stageID, string userID, string agentID, int pageIndex = 1)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("orderID", orderID);
            paras.Add("stageID", stageID);
            paras.Add("pageIndex", pageIndex);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<TaskReplyListResult>(ApiOption.GetTaskReplys, paras);
        }

        /// <summary>
        /// 获取任务日志列表
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public TaskLogListResult GetTaskLogs(string taskID, string userID, string agentID, int pageindex = 1)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("taskID", taskID);
            paras.Add("pageIndex", pageindex);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<TaskLogListResult>(ApiOption.GetTaskLogs, paras);
        }

        /// <summary>
        /// 获取订单基本信息
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public OrderBaseResult GetOrderInfo(string orderID, string userID, string agentID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("orderID", orderID);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<OrderBaseResult>(ApiOption.GetOrderInfo, paras);
        }
        #endregion

        #region update

        /// <summary>
        /// 更新任务到期时间
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="endTime"></param>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <returns>result 0：失败，1：成功，2: 任务已接受,3:没有权限</returns>
        public UpdateResult UpdateTaskEndTime(string taskID, string endTime, string userID, string agentID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("taskID", taskID);
            paras.Add("endTime", endTime);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);
            return HttpRequest.RequestServer<UpdateResult>(ApiOption.UpdateTaskEndTime, paras);
        }

        /// <summary>
        /// 标记完成任务
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <returns>result 0：失败，1：成功，2: 有前面阶段任务未完成,3:没有权限；4：任务没有接受，不能设置完成;5.任务有未完成步骤</returns>
        public UpdateResult FinishTask(string taskID, string userID, string agentID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("taskID", taskID);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<UpdateResult>(ApiOption.FinishTask, paras);
        }

        /// <summary>
        /// 添加任务讨论
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public TaskReplyListResult SavaTaskReply(TaskReplyEntity reply, string userID, string agentID)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("reply", JsonConvert.SerializeObject(reply).ToString() );
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<TaskReplyListResult>(ApiOption.SavaTaskReply, paras);
        }
        #endregion

    }
}
