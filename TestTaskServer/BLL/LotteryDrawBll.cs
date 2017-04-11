using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace TestTaskServer
{
    using MySql.Data.MySqlClient;

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

            //获取用户信息，并判断宝石数量
            UserInfoModel userInfo = UserInfoBll.Instance.GetUserInfo(userFlag);
            UserInfoBll.Instance.CheckUserDiamond(userInfo);

            //检测用户抽奖信息，并获取当前用户的抽奖信息
            LotteryDrawModel userLotteryInfo = CheckUserLotteryInfo(userFlag, userInfo.UserName);

            //执行抽奖的数据库操作
            CommonTranDal.Instance.UserLotteryTran(userInfo, userLotteryInfo);

            //更新用户抽奖数据
            UpdateLotteryDrawData(userFlag);

            //更新用户宝石数据
            UserInfoBll.Instance.UpdateUserDiamondData(userFlag);
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
        /// 查询是否存在抽奖关联数据，不存在则插入数据，并返回当前用户抽奖信息
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        /// <param name="userName">用户名</param>
        /// <returns>用户抽奖分数</returns>
        public LotteryDrawModel CheckUserLotteryInfo(String userFlag, String userName)
        {
            var currentUserLotteryInfo = lotteryDrawData.FirstOrDefault(r => r.UserFlag == userFlag);
            if (currentUserLotteryInfo == null)
            {
                currentUserLotteryInfo = new LotteryDrawModel { UserName = userName, UserFlag = userFlag, Points = 0, LastLotteryDrawTime = DateTime.MinValue };
                LotteryDrawDal.Instance.InsertInitLotteryDrawData(currentUserLotteryInfo);

                //将当前用户抽奖数据写入抽奖数据中
                try
                {
                    lotteryDrawLock.EnterWriteLock();
                    lotteryDrawData.Add(currentUserLotteryInfo);
                }
                finally
                {
                    lotteryDrawLock.ExitWriteLock();
                }
            }

            return currentUserLotteryInfo;
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
                if (ldm.UserFlag != userFlag)
                {
                    continue;
                }

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