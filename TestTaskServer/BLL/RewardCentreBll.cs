using System;
using System.Threading;

namespace TestTaskServer
{
    public class RewardCentreBll
    {
        #region 属性

        /// <summary>
        /// 奖励中心类实例
        /// </summary>
        public volatile static RewardCentreBll Instance = new RewardCentreBll();

        /// <summary>
        /// 抽奖获取结果线程
        /// </summary>
        Thread lotteryDrawthread = null;

        /// <summary>
        /// 防止抽奖结果线程重复运行锁
        /// </summary>
        private static readonly Object lockObj = new Object();

        #endregion

        #region 方法

        /// <summary>
        /// 奖励中心
        /// </summary>
        public void StartRewardCentre()
        {
            lock(lockObj)
            {
                if (lotteryDrawthread == null)
                {
                    lotteryDrawthread = new Thread(LotteryDrawResultCentre);
                    lotteryDrawthread.Start();
                }
            }
        }

        /// <summary>
        /// 抽奖结果打印
        /// </summary>
        private void LotteryDrawResultCentre()
        {
            while (LotteryDrawConfigBll.Instance.CheckDateTimeIsOverLotteryTime())
            {
                int index = 1;
                foreach (LotteryDrawModel lotteryDrawModel in LotteryDrawBll.Instance.GetLotteryDrawChartsData())
                {
                    Console.WriteLine(lotteryDrawModel.UserName + ":" + index + "|" + lotteryDrawModel.Points);
                    index++;
                }
            }
        }

        #endregion
    }
}