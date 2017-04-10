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
        /// 用户操作bll类
        /// </summary>
        private static LotteryDrawConfigBll mInstance = new LotteryDrawConfigBll();

        /// <summary>
        /// 用户操作bll的实例
        /// </summary>
        public static LotteryDrawConfigBll Instance
        {
            get
            {
                return mInstance;
            }
        }

        /// <summary>
        /// 上次更新时间
        /// </summary>
        private DateTime lastRefreshDateTime;

        /// <summary>
        /// 抽奖配置表
        /// </summary>
        private List<LotteryDrawConfigModel> lotteryDrawConfigDS = null;

        #endregion

        #region 方法

        /// <summary>
        /// 检测抽奖时间配置
        /// </summary>
        public void CheckLotteryTimeConfig()
        {
            if (GetLotteryDrawConfigDs() != null && GetLotteryDrawConfigDs().Count > 0)
            {
                // 判断活动时间
                if (GetLotteryDrawConfigDs()[0].BeginTime > DateTime.Now)
                {
                    //如果开始时间大于当前时间，则提示活动未开始
                    throw new Exception(CommonConst.NotBeginActivity);
                }
                else if (GetLotteryDrawConfigDs()[0].EndTime < DateTime.Now)
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
        public Boolean CheckLotteryTimeConfig(DateTime lotteryTime)
        {
            return GetLotteryDrawConfigDs()[0].BeginTime <= lotteryTime && GetLotteryDrawConfigDs()[0].EndTime >= lotteryTime;
        }

        /// <summary>
        /// 检测抽奖时间是否大于等于当前活动的开始时间
        /// </summary>
        /// <param name="lotteryTime">抽奖时间</param>
        /// <returns>结果</returns>
        public Boolean CheckLotteryTimeIsAvailable(DateTime lotteryTime)
        {
            return GetLotteryDrawConfigDs()[0].BeginTime <= lotteryTime;
        }

        /// <summary>
        /// 检测当前时间是否大于等于当前活动的结束时间
        /// </summary>
        /// <returns>结果</returns>
        public Boolean CheckDateTimeIsOverLotteryTime()
        {
            return GetLotteryDrawConfigDs()[0].EndTime <= DateTime.Now;
        }

        /// <summary>
        /// 获取抽奖配置表
        /// </summary>
        /// <returns>返回抽奖配置表数据</returns>
        private List<LotteryDrawConfigModel> GetLotteryDrawConfigDs()
        {
            //检测是否在抽奖时间范围内
            if ((lotteryDrawConfigDS == null || lotteryDrawConfigDS.Count == 0) || lastRefreshDateTime.Date != DateTime.Now.Date)
            {
                lastRefreshDateTime = DateTime.Now;
                lotteryDrawConfigDS = LotteryDrawConfigDal.Instance.GetLotteryDrawConfigData();
            }

            return lotteryDrawConfigDS;
        }

        #endregion
    }
}