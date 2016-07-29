using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesBusiness;
using CloudSalesEnum;
using CloudSalesEntity.Manage;
using CloudSalesEntity;

namespace YunXiaoService
{
    public class UserService
    {
        /// <summary>
        /// 是否存在账号 
        /// </summary>
        /// <param name="type">账号类型</param>
        /// <param name="account">账号</param>
        /// <param name="companyid">网络ID（非明道用户可为空）</param>
        /// <returns></returns>
        public static bool IsExistAccount(EnumAccountType type, string account, string companyid)
        {
            switch (type)
            {
                case EnumAccountType.UserName:
                    return OrganizationBusiness.IsExistLoginName(account);
                case EnumAccountType.Mobile:
                    return OrganizationBusiness.IsExistLoginName(account);
                default:
                    return OrganizationBusiness.IsExistOtherAccount(type, account, companyid);

            }
        }

        /// <summary>
        /// 注册二当家
        /// </summary>
        /// <param name="registerType">来源类型</param>
        /// <param name="accountType">账号类型</param>
        /// <param name="account">账号</param>
        /// <param name="loginPwd">密码</param>
        /// <param name="clientName">客户名称</param>
        /// <param name="contactName">联系人</param>
        /// <param name="mobile">联系方式</param>
        /// <param name="email">邮箱</param>
        /// <param name="industry">行业</param>
        /// <param name="citycode">城市</param>
        /// <param name="address">地址</param>
        /// <param name="remark">备注</param>
        /// <param name="companyid">明道网络ID，智能工厂ID</param>
        /// <param name="companyCode">智能工厂Code</param>
        /// <param name="customerid">智能工厂客户ID</param>
        /// <param name="operateid">操作人</param>
        /// <param name="result">返回结果 0失败 1成功 2账号已存在</param>
        /// <param name="userid">返回结果 默认员工ID</param>
        /// <returns></returns>
        public static string InsertClient(EnumRegisterType registerType, EnumAccountType accountType, string account, string loginPwd, string clientName, 
                                          string contactName, string mobile, string email, string industry, string citycode, string address, string remark,
                                          string companyid, string companyCode, string customerid, string operateid, out int result, out string userid)
        {
            return CloudSalesBusiness.Manage.ClientBusiness.InsertClient(registerType, accountType, account, loginPwd, clientName, contactName, mobile, email,
                                                                         industry, citycode, address, remark, companyid, companyCode, customerid, operateid, out result, out userid);
        }

        /// <summary>
        /// 获取客户端信息
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public static Clients GetClientDetail(string clientID)
        {
            return CloudSalesBusiness.Manage.ClientBusiness.GetClientDetail(clientID);
        }

        /// <summary>
        /// 根据账号获取员工信息
        /// </summary>
        /// <param name="accountType">账号类型</param>
        /// <param name="account">账号</param>
        /// <param name="pwd">密码（第三方账号可为空）</param>
        /// <param name="companyid">网络ID（非明道用户为空）</param>
        /// <param name="ip">登录IP</param>
        /// <returns></returns>
        public static Users GetUserByAccount(EnumAccountType accountType, string account, string pwd, string companyid, string ip,out int result)
        {
            result = 0;
            Users model = null;
            switch (accountType)
            {
                case EnumAccountType.UserName:
                    model= OrganizationBusiness.GetUserByUserName(account, pwd, out result, ip);
                    break;
                case EnumAccountType.Mobile:
                    model= OrganizationBusiness.GetUserByUserName(account, pwd, out result, ip);
                    break;
                case EnumAccountType.MingDao:
                case EnumAccountType.WeiXin:
                    model = OrganizationBusiness.GetUserByOtherAccount(account, companyid, ip, (int)accountType);
                    break;
            }
            return model;
        }

        /// <summary>
        /// 根据员工ID获取员工信息
        /// </summary>
        /// <param name="userid">员工ID</param>
        /// <param name="agentid">代理ID</param>
        /// <returns></returns>
        public static Users GetUserByUserID(string userid, string agentid)
        {
            return OrganizationBusiness.GetUserByUserID(userid, agentid);
        }
    }
}
