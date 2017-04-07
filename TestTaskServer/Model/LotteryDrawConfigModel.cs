using System;

namespace TestTaskServer
{
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