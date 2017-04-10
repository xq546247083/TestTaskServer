using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TestTaskServer
{
    /// <summary>
    /// 用户抽奖类
    /// </summary>
    public class LotteryDrawBll
    {
        #region 属性

        /// <summary>
        /// 用户抽奖bll类
        /// </summary>
        private static LotteryDrawBll mInstance = new LotteryDrawBll();

        /// <summary>
        /// 用户抽奖bll的实例
        /// </summary>
        public static LotteryDrawBll Instance
        {
            get
            {
                return mInstance;
            }
        }

        /// <summary>
        /// 用户抽奖数据缓存锁
        /// </summary>
        private ReaderWriterLockSlim lotteryDrawLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 缓存抽奖数据
        /// </summary>
        private List<LotteryDrawModel> lotteryDrawData = LotteryDrawDal.Instance.GetLotteryDrawData();

        #endregion

        #region 方法

        /// <summary>
        /// 抽奖操作
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        public void LotteryDraw(String userFlag)
        {
            LotteryDrawConfigBll.Instance.CheckLotteryTimeConfig();

            String userName = UserInfoBll.Instance.CheckUserInfoAndGetUserName(userFlag);

            //获取当前用户的最新抽奖时间,以及判断宝石是否足够
            DateTime lastLotteryDrawTime = CheckUserLotteryInfo(userFlag, userName);

            //执行抽奖的数据库操作
            String pointsStr = LotteryDrawConfigBll.Instance.CheckLotteryTimeConfig(lastLotteryDrawTime) ? "POINTS+10 " : "10";
            UserInfoDal.Instance.UserLotteryDrwaOperation(userFlag, lastLotteryDrawTime, pointsStr);

            //更新用户抽奖数据
            UpdateLotteryDrawData(userFlag);
        }

        /// <summary>
        /// 获取排行榜
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        /// <returns>排行榜数据</returns>
        public List<LotteryDrawModel> GetCharts(ref String userFlag)
        {
            //获取当前用户分数
            userFlag = GetUserPoints(userFlag).ToString();

            return GetLotteryDrawChartsData();
        }

        /// <summary>
        /// 查询是否存在抽奖关联数据，不存在则插入数据，并返回当前用户分数
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        /// <param name="userName">用户名</param>
        /// <returns>用户抽奖分数</returns>
        public DateTime CheckUserLotteryInfo(String userFlag, String userName)
        {
            var currentUserInfo = lotteryDrawData.FirstOrDefault(r => r.UserFlag == userFlag);
            if (currentUserInfo == null)
            {
                LotteryDrawModel lotteryDrawModel = new LotteryDrawModel() { UserName = userName, UserFlag = userFlag };
                LotteryDrawDal.Instance.InsertInitLotteryDrawData(lotteryDrawModel);

                //将当前用户抽奖数据写入抽奖数据中
                try
                {
                    lotteryDrawLock.EnterWriteLock();
                    lotteryDrawData.Add(lotteryDrawModel);
                }
                finally
                {
                    lotteryDrawLock.ExitWriteLock();
                }

                return DateTime.MinValue;
            }

            return currentUserInfo.LastLotteryDrawTime;
        }

        /// <summary>
        /// 获取用户抽奖积分
        /// </summary>
        /// <param name="userFlag">传入用户标志位</param>
        /// <returns>抽奖积分</returns>
        public Int32 GetUserPoints(String userFlag)
        {
            //查询积分
            var currentUserInfo = lotteryDrawData.FirstOrDefault(r => r.UserFlag == userFlag);
            if (currentUserInfo != null)
            {
                return currentUserInfo.Points;
            }

            return 0;
        }

        /// <summary>
        /// 获取排行榜
        /// </summary>
        /// <returns>返回排行榜数据</returns>
        public List<LotteryDrawModel> GetLotteryDrawChartsData()
        {
            try
            {
                //获取排行榜数据
                lotteryDrawLock.EnterReadLock();
                List<LotteryDrawModel> lotteryDrawModelResult = lotteryDrawData.Where(
                    r => LotteryDrawConfigBll.Instance.CheckLotteryTimeIsAvailable(r.LastLotteryDrawTime) && r.Points > 0)
                    .OrderByDescending(r => r.Points).Take(20).ToList();

                return lotteryDrawModelResult;
            }
            finally
            {
                lotteryDrawLock.ExitReadLock();
            }
        }

        /// <summary>
        /// 修改用户抽奖积分数据
        /// </summary>
        public void UpdateLotteryDrawData(String userFlag)
        {
            foreach (LotteryDrawModel ldm in lotteryDrawData)
            {
                if (ldm.UserFlag != userFlag) continue;
                try
                {
                    //修改当前用户的抽奖积分，如果上次抽奖时间在当次活动内，则积分叠加，否则，积分置为第一次抽奖积分
                    lotteryDrawLock.EnterWriteLock();

                    if (!LotteryDrawConfigBll.Instance.CheckLotteryTimeConfig(ldm.LastLotteryDrawTime))
                    {
                        ldm.Points = 10;
                    }
                    else
                    {
                        ldm.Points += 10;
                    }

                    ldm.LastLotteryDrawTime = DateTime.Now;

                    break;
                }
                finally
                {
                    lotteryDrawLock.ExitWriteLock();
                }
            }
        }

        #endregion
    }
}