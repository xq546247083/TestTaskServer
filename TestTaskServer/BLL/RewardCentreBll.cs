using System;
using System.Diagnostics;
using System.Threading;

namespace TestTaskServer
{
    /// <summary>
    /// 奖励中心类
    /// </summary>
    public class RewardCentreBll
    {
        #region 属性

        /// <summary>
        /// 奖励中心bll类
        /// </summary>
        private static RewardCentreBll mInstance = new RewardCentreBll();

        /// <summary>
        /// 奖励中心Bll类的实例
        /// </summary>
        public static RewardCentreBll Instance
        {
            get
            {
                return mInstance;
            }
        }

        /// <summary>
        /// 抽奖获取结果线程
        /// </summary>
        private Thread lotteryDrawthread = null;

        /// <summary>
        /// 防止抽奖结果线程重复运行锁
        /// </summary>
        private static readonly Object mLockObj = new Object();

        #endregion

        #region 方法

        /// <summary>
        /// 奖励中心
        /// </summary>
        public void StartRewardCentre()
        {
            lock (mLockObj)
            {
                if (lotteryDrawthread != null)
                {
                    return;
                }

                //开启线程，在活动结束后执行输出操作
                lotteryDrawthread = new Thread(LotteryDrawResultCentre);
                lotteryDrawthread.Start();
            }
        }

        /// <summary>
        /// 抽奖结果打印
        /// </summary>
        private void LotteryDrawResultCentre()
        {
            while (true)
            {
                //如果检测到当前时间在活动结束后，输出抽奖结果
                if (LotteryDrawConfigBll.Instance.CheckDateTimeIsOverLotteryTime())
                {
                    Int32 index = 1;
                    foreach (LotteryDrawModel lotteryDrawModel in LotteryDrawBll.Instance.GetLotteryDrawChartsData())
                    {
                        Debug.WriteLine(lotteryDrawModel.UserName + ":" + index + "|" + lotteryDrawModel.Points);
                        index++;
                    }

                    break;
                }
            }
        }

        #endregion
    }
}