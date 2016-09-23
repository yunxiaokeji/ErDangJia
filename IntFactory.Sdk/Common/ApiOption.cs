using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IntFactory.Sdk
{
    public enum ApiOption
    {
        [Description("getToken")]
        getToken,

        [Description("/api/user/userLogin")]
        userLogin,

        [Description("/api/user/getUserByUserID")]
        getUserByUserID,

        [Description("/api/task/getTasks")]
        GetTasks,

        [Description("/api/task/getOrderProcess")]
        GetOrderProcess,

        [Description("/api/task/getOrderStages")]
        GetOrderStages,

        [Description("/api/task/getTaskDetail")]
        GetTaskDetail,

        [Description("/api/task/getTaskReplys")]
        GetTaskReplys,

        [Description("/api/task/getTaskLogs")]
        GetTaskLogs,

        [Description("/api/task/getOrderInfo")]
        GetOrderInfo,

        [Description("/api/task/updateTaskEndTime")]
        UpdateTaskEndTime,

        [Description("/api/task/finishTask")]
        FinishTask,

        [Description("/api/task/savaTaskReply")]
        SavaTaskReply,

        [Description("/api/customer/getCustomerByID")]
        GetCustomerByID,

        [Description("/api/customer/getCustomerByMobilePhone")]
        GetCustomerByMobilePhone,

        [Description("/api/customer/setCustomerYXinfo")]
        SetCustomerYXinfo,

        [Description("/api/order/getOrdersByYXClientCode")]
        GetOrdersByYXClientCode,

        [Description("/api/order/getOrderDetailByID")]
        GetOrderDetailByID,

        [Description("/api/order/getOrderAttrsListByGoodsID")]
        GetOrderAttrsList,

        [Description("/api/order/getOrderAttrsListByOrderID")]
        GetOrderAttrsListByOrderID,
        /// <summary>
        /// 创建订单
        /// </summary>
        [Description("/api/order/createOrder")]
        CreateOrder,

        [Description("/api/order/createDHOrder")]
        CreateDHOrder,

        [Description("/api/order/getOrderTasks")]
        GetOrderTasks,

        /// <summary>
        /// 分类
        /// </summary>
        [Description("/api/client/getClientCategorys")]
        GetClientCategorys,
         /// <summary>
        /// 分类
        /// </summary>
        [Description("/api/client/getCategoryID")]
        GetCategoryByID, 
        /// <summary>
        /// 加工品类
        /// </summary>
        [Description("/api/client/getProcessCategorys")]
        GetProcessCategorys,
        /// <summary>
        /// 所有分类
        /// </summary>
        [Description("/api/client/getAllCategorys")]
        GetAllCategorys,

        [Description("/api/client/getClientInfo")]
        GetClientInfo
    }
}
