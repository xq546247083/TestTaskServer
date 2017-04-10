using System;
using System.Collections.Generic;
using System.Data;

namespace TestTaskServer
{
    /// <summary>
    /// 用户操作类
    /// </summary>
    public class LotteryDrawConfigDal
    {
        #region 属性

        private volatile static LotteryDrawConfigDal _instance = null;
        private static readonly object mLockHelper = new object();

        /// <summary>
        /// 单例
        /// </summary>
        public static LotteryDrawConfigDal Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (mLockHelper)
                    {
                        if (_instance == null)
                            _instance = new LotteryDrawConfigDal();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 上次更新时间
        /// </summary>
        private DateTime lastRefreshDateTime;

        /// <summary>
        /// 抽奖配置表
        /// </summary>
        private List<LotteryDrawConfigModel> _lotteryDrawConfigDS = null;
        private List<LotteryDrawConfigModel> lotteryDrawConfigDS
        {
            get
            {
                //检测是否在抽奖时间范围内
                if ((_lotteryDrawConfigDS == null || _lotteryDrawConfigDS.Count == 0) || lastRefreshDateTime.Date != DateTime.Now.Date)
                {
                    lastRefreshDateTime = DateTime.Now;
                    _lotteryDrawConfigDS = MySqlHelper.GetDataList<LotteryDrawConfigModel>(CommandType.Text, "SELECT * FROM LotterydrawConfig");
                }

                return _lotteryDrawConfigDS;
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
            if (lotteryDrawConfigDS != null & lotteryDrawConfigDS.Count > 0)
            {
                // 判断活动时间
                if (lotteryDrawConfigDS[0].BeginTime > DateTime.Now)
                {
                    //如果开始时间大于当前时间，这提示活动未开始
                    throw new Exception(CommonConst.NotBeginActivity);
                }
                else if (lotteryDrawConfigDS[0].EndTime < DateTime.Now)
                {
                    //如果结束时间小于当前时间，这提示活动已结局
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
            if (lotteryDrawConfigDS[0].BeginTime > lotteryTime || lotteryDrawConfigDS[0].EndTime < lotteryTime)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}