using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace IntFactory.Sdk
{
    public class OauthBusiness
    {
        /// <summary>
        /// 获取用户授权url
        /// </summary>
        public static string GetAuthorizeUrl()
        {
            Dictionary<string, string> paras = new Dictionary<string, string>();
            paras.Add("client_id", AppConfig.AppKey);
            paras.Add("site", "china");
            paras.Add("redirect_uri", AppConfig.CallBackUrl);

            return string.Format("{0}/auth/authorize.htm?client_id={1}&site=china&redirect_uri={2}}",
                AppConfig.ApiUrl, AppConfig.AppKey, AppConfig.CallBackUrl);
        }
        public static string GetAuthorize(string returnurl = "")
        {
            string sign = GetSign("");
            string url = "{0}/Home/authorize?sign={1}&redirect_uri={2}";
            //处理真实返回地址
            if (!string.IsNullOrEmpty(returnurl))
            {
                url += "&status=" + returnurl;
            }
            return string.Format(url, AppConfig.ApiUrl, sign, AppConfig.CallBackUrl);
        }

        public static string GetSign(string returnurl = "")
        {
            return Signature.GetSignature(AppConfig.AppKey, AppConfig.AppSecret, string.IsNullOrEmpty(returnurl) ? AppConfig.CallBackUrl : returnurl);
        }

        /// <summary>
        /// 通过code获取用户token
        /// </summary>
        //public static TokenResult GetUserToken(string code)
        //{
        //    var paras = new Dictionary<string, object>();
        //    paras.Add("code", code);
        //    paras.Add("redirect_uri", AppConfig.CallBackUrl);
        //    paras.Add("grant_type", "authorization_code");
        //    paras.Add("need_refresh_token", true);
        //    paras.Add("client_id", AppConfig.AppKey);
        //    paras.Add("client_secret", AppConfig.AppSecret);

        //    string resultStr = HttpRequest.RequestServer(ApiOption.getToken, paras, RequestType.Post);
        //    return JsonConvert.DeserializeObject<TokenResult>(resultStr);
        //}

        ///// <summary>
        ///// 通过refreshToken获取用户token
        ///// </summary>
        //public static TokenResult GetTokenByRefreshToken(string refreshToken)
        //{
        //    var paras = new Dictionary<string, object>();
        //    paras.Add("refresh_token", refreshToken);
        //    paras.Add("grant_type", "refresh_token");
        //    paras.Add("client_id", AppConfig.AppKey);
        //    paras.Add("client_secret",AppConfig.AppSecret);

        //    string resultStr= HttpRequest.RequestServer(ApiOption.getToken, paras, RequestType.Post);
        //    return JsonConvert.DeserializeObject<TokenResult>(resultStr);
        //}

        ///// <summary>
        ///// 获取用户
        ///// </summary>
        ///// <param name="code"></param>
        ///// <returns></returns>
        //public static MemberResult GetUserInfo(string code)
        //{
        //    var tokenEntity = GetUserToken(code);
        //    var model = UserBusiness.GetMemberDetail(tokenEntity.access_token, tokenEntity.memberId);

        //    return model;
        //}

    }
}
