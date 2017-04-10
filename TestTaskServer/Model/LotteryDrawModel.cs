using System;
namespace TestTaskServer
{
    public class LotteryDrawModel
    {
        /// <summary>
        /// 序列号
        /// </summary>
        public int SerialNumber { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户标志
        /// </summary>
        public string UserFlag { get; set; }

        /// <summary>
        /// 抽奖分数
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 最新抽奖时间
        /// </summary>
        public DateTime LastLotteryDrawTime { get; set; }
    }
}