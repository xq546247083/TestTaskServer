using System;

namespace TestTaskServer
{
    /// <summary>
    /// 抽奖信息Model
    /// </summary>
    public class LotteryDrawModel
    {
        /// <summary>
        /// 序列号
        /// </summary>
        public Int32 SerialNumber { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// 用户标志
        /// </summary>
        public String UserFlag { get; set; }

        /// <summary>
        /// 抽奖分数
        /// </summary>
        public Int32 Points { get; set; }

        /// <summary>
        /// 最新抽奖时间
        /// </summary>
        public DateTime LastLotteryDrawTime { get; set; }
    }
}