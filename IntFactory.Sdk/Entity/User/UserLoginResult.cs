using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk
{
    public class UserLoginResult
    {
        /// <summary>
        /// 0.账号或密码有误；1.成功；2.密码输入错误超过3次，请2小时后再试；3.账号或密码有误；-1：账号已冻结
        /// </summary>
        public int result = 0;

        /// <summary>
        /// 密码错误次数
        /// </summary>
        public int errorCount = 0;

        /// <summary>
        /// 冻结时间 分钟
        /// </summary>
        public int forbidTime = 0;

        public UserBase user;

        public int error_code = 0;

        public string error_message;
    }
}
