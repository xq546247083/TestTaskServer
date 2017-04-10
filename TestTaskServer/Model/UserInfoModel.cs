using System;

namespace TestTaskServer
{
    /// <summary>
    /// 用户信息Moldel
    /// </summary>
    public class UserInfoModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// 用户标志
        /// </summary>
        public String UserFlag { get; set; }

        /// <summary>
        /// 宝石数量
        /// </summary>
        public Int32 DiamondNumber { get; set; }
    }
}