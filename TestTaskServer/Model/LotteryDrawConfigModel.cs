using System;

namespace TestTaskServer
{
    /// <summary>
    /// 抽奖配置Model
    /// </summary>
    public class LotteryDrawConfigModel
    {
        /// <summary>
        /// 抽奖开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 抽奖结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}