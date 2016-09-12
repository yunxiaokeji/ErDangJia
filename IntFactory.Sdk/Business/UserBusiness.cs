﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace IntFactory.Sdk
{
    public class UserBusiness
    {
        /// <summary>
        /// 获取用户详情
        /// </summary>
        public static UserResult GetUserByUserID(string userid, string clientid)
        {
            var paras = new Dictionary<string, object>();
            paras.Add("userid", userid);
            paras.Add("clientid", clientid);

            return HttpRequest.RequestServer<UserResult>(ApiOption.memberDetail, paras);

        }

        public static UserLoginResult UserLogin(string userName, string pwd,string userID,string agentID) 
        {
            var paras = new Dictionary<string, object>();
            paras.Add("userName", userName);
            paras.Add("pwd", pwd);
            paras.Add("userID", userID);
            paras.Add("agentID", agentID);

            return HttpRequest.RequestServer<UserLoginResult>(ApiOption.userLogin, paras);
        }

    }
}
