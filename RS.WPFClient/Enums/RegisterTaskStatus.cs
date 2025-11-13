using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.WPFClient.Enums
{
    public enum RegisterTaskStatus
    {
        /// <summary>
        /// 注册表单
        /// </summary>
        RegisterForm = 0,
        /// <summary>
        /// 邮箱验证
        /// </summary>
        EmailVerify = 1,
        /// <summary>
        /// 注册成功
        /// </summary>
        RegisterSuccess = 2
    }
}
