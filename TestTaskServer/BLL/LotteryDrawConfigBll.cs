using System;
using System.Collections.Generic;

namespace TestTaskServer
{
    /// <summary>
    /// 用户操作类
    /// </summary>
    public class LotteryDrawConfigBll
    {
        #region 属性

        /// <summary>
        /// LotteryDrawConfigBll的实例
        /// </summary>
        public volatile static LotteryDrawConfigBll Instance = new LotteryDrawConfigBll();

        /// <summary>
        /// 上次更新时间
        /// </summary>
        private DateTime lastRefreshDateTime;

        /// <summary>
        /// 抽奖配置表的字段
        /// </summary>
        private List<LotteryDrawConfigModel> lotteryDrawConfigDS = null;

        /// <summary>
        /// 抽奖配置表
        /// </summary>
        private List<LotteryDrawConfigModel> LotteryDrawConfigDS
        {
            get
            {
                GetLotteryDrawConfigDS();
                return lotteryDrawConfigDS;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 检测抽奖时间配置
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        public void CheckLotteryTimeConfig()
        {
            if (LotteryDrawConfigDS != null & LotteryDrawConfigDS.Count > 0)
            {
                // 判断活动时间
                if (LotteryDrawConfigDS[0].BeginTime > DateTime.Now)
                {
                    //如果开始时间大于当前时间，则提示活动未开始
                    throw new Exception(CommonConst.NotBeginActivity);
                }
                else if (LotteryDrawConfigDS[0].EndTime < DateTime.Now)
                {
                    //如果结束时间小于当前时间，则提示活动已结局
                    throw new Exception(CommonConst.ShutDownActivity);
                }
            }
            else
            {
                throw new Exception(CommonConst.NoThisActivity);
            }
        }

        /// <summary>
        /// 检测抽奖时间配置
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        public bool CheckLotteryTimeConfig(DateTime lotteryTime)
        {
            return LotteryDrawConfigDS[0].BeginTime <= lotteryTime && LotteryDrawConfigDS[0].EndTime >= lotteryTime;
        }

        /// <summary>
        /// 检测抽奖时间是否大于等于当前活动的开始时间
        /// </summary>
        /// <param name="lotteryTime">抽奖时间</param>
        /// <returns>结果</returns>
        public bool CheckLotteryTimeIsAvailable(DateTime lotteryTime)
        {
            return LotteryDrawConfigDS[0].BeginTime <= lotteryTime;
        }

        /// <summary>
        /// 获取抽奖配置表
        /// </summary>
        /// <returns></returns>
        private void GetLotteryDrawConfigDS()
        {
            //检测是否在抽奖时间范围内
            if ((lotteryDrawConfigDS == null || lotteryDrawConfigDS.Count == 0) || lastRefreshDateTime.Date != DateTime.Now.Date)
            {
                lastRefreshDateTime = DateTime.Now;
                lotteryDrawConfigDS = LotteryDrawConfigDal.Instance.GetLotteryDrawConfigData();
            }
        }

        #endregion
    }
}