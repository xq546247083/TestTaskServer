using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTaskServer
{
    /// <summary>
    /// 用户操作类
    /// </summary>
    public class LotteryDrawBll
    {
        #region 属性

        /// <summary>
        /// 抽奖类实例
        /// </summary>
        public volatile static LotteryDrawBll Instance = new LotteryDrawBll();

        /// <summary>
        /// LotteryDrawData的字段
        /// </summary>
        private List<LotteryDrawModel> lotteryDrawData = null;

        /// <summary>
        /// 抽奖数据信息表,用来暂存用户抽奖信息
        /// </summary>
        private List<LotteryDrawModel> LotteryDrawData
        {
            get
            {
                if (lotteryDrawData == null || lotteryDrawData.Count == 0)
                {
                    lotteryDrawData = LotteryDrawDal.Instance.GetLotteryDrawData();
                }

                return lotteryDrawData;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 获取排行榜
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        /// <returns>排行榜数据</returns>
        public List<LotteryDrawModel> GetCharts(ref string userFlag)
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
        public DateTime GetUserLotteryInfo(string userFlag, string userName)
        {
            var currentUserInfo = LotteryDrawData.Where(r => r.UserFlag == userFlag).ToList();
            if (!(currentUserInfo != null && currentUserInfo.Count > 0))
            {
                LotteryDrawModel LotteryDrawModel = new LotteryDrawModel() { UserName = userName, UserFlag = userFlag };
                LotteryDrawDal.Instance.InsertInitLotteryDrawData(LotteryDrawModel);
                lotteryDrawData.Add(LotteryDrawModel);

                return DateTime.MinValue;
            }
            else
            {
                return currentUserInfo[0].LastLotteryDrawTime;
            }
        }

        /// <summary>
        /// 获取用户抽奖积分
        /// </summary>
        /// <param name="userFlag">传入用户标志位</param>
        /// <returns>抽奖积分</returns>
        public int GetUserPoints(string userFlag)
        {
            //查询积分
            var currentUserInfo = LotteryDrawData.Where(r => r.UserFlag == userFlag).ToList();
            if (currentUserInfo != null & currentUserInfo.Count > 0)
            {
                return currentUserInfo[0].Points;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取排行榜
        /// </summary>
        /// <returns>返回排行榜数据</returns>
        private List<LotteryDrawModel> GetLotteryDrawChartsData()
        {
            return LotteryDrawData.Where(
                r => LotteryDrawConfigBll.Instance.CheckLotteryTimeIsAvailable(r.LastLotteryDrawTime) && r.Points > 0)
                .OrderByDescending(r => r.Points).Take(20).ToList();
        }

        /// <summary>
        /// 修改用户抽奖积分数据
        /// </summary>
        public void UpdateLotteryDrawData(string userFlag)
        {
            foreach (LotteryDrawModel ldm in lotteryDrawData)
            {
                if (ldm.UserFlag == userFlag)
                {
                    if (LotteryDrawConfigBll.Instance.CheckLotteryTimeConfig(ldm.LastLotteryDrawTime))
                        ldm.Points += 10;
                    else
                        ldm.Points = 10;
                    break;
                }
            }
        }

        #endregion
    }
}