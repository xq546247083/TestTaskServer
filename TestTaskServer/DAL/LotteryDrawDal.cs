using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TestTaskServer
{
    /// <summary>
    /// 用户操作类
    /// </summary>
    public class LotteryDrawDal
    {
        #region 属性

        private volatile static LotteryDrawDal _instance = null;
        private static readonly object mLockHelper = new object();
        /// <summary>
        /// 单例
        /// </summary>
        public static LotteryDrawDal Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (mLockHelper)
                    {
                        if (_instance == null)
                            _instance = new LotteryDrawDal();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 获取排行榜的互斥锁
        /// </summary>
        private static readonly object mLockObj = new object();

        private DataTable lotteryDrawChartsDT = null;
        /// <summary>
        /// 排行榜
        /// </summary>
        public DataTable LotteryDrawChartsDT
        {
            get
            {
                return lotteryDrawChartsDT;
            }
        }

        /// <summary>
        /// 抽奖数据信息表,用来暂存用户抽奖信息
        /// </summary>
        private List<LotteryDrawModel> lotteryDrawData = null;
        private List<LotteryDrawModel> LotteryDrawData
        {
            get
            {
                if (lotteryDrawData == null || lotteryDrawData.Count == 0)
                {
                    lotteryDrawData = MySqlHelper.GetDataList<LotteryDrawModel>(CommandType.Text, "SELECT * FROM lotterydraw");
                }
                return lotteryDrawData;
            }
        }

        /// <summary>
        /// 排行榜最低分
        /// </summary>
        private int lowestPoints
        {
            get
            {
                if (lotteryDrawChartsDT != null && lotteryDrawChartsDT.Rows.Count > 0)
                {
                    return int.Parse(lotteryDrawChartsDT.Rows[lotteryDrawChartsDT.Rows.Count - 1]["Points"].ToString());
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 查询是否存在抽奖关联数据，不存在则插入数据，并返回当前用户分数
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        /// <param name="userName">用户名</param>
        /// <returns>用户抽奖分数</returns>
        public DateTime GetUserLotteryInfo(string userFlag, string userName, ref int currenUserPoints)
        {
            var currentUserInfo = LotteryDrawData.Where(r => r.UserFlag == userFlag).ToList();
            if (!(currentUserInfo != null && currentUserInfo.Count > 0))
            {
                MySqlHelper.ExecuteNonQuery(CommandType.Text, "INSERT INTO lotterydraw (userName,userflag) VALUES ('" + userName + "','" + userFlag + "')");
                RefreshLotteryDrawData();
                currenUserPoints = 0;
                return DateTime.MinValue;
            }
            else
            {
                if (LotteryDrawConfigDal.Instance.CheckLotteryTimeConfig(currentUserInfo[0].LastLotteryDrawTime))
                {
                    currenUserPoints = currentUserInfo[0].Points;
                }
                else
                {
                    currenUserPoints = 0;
                }

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
        /// 更新排行榜数据
        /// </summary>
        /// <param name="flag">操作提示</param>
        /// <param name="currenUserPoints">用户最低分</param>
        public void RefreshCharts(int flag, int currenUserPoints = 0)
        {
            lock (mLockObj)
            {
                //如果操作为查询排行榜且排行榜为空 或者 如果排行榜人数未满20人或者排行榜最低分小于了当前的分数，则更新排行榜
                if ((lotteryDrawChartsDT == null && flag == 0) ||
                    (((lotteryDrawChartsDT != null && lotteryDrawChartsDT.Rows.Count < 20) || (lowestPoints < currenUserPoints + 10 && lotteryDrawChartsDT != null)) && flag == 1))
                {
                    DataSet lotteryDrawChartsDS = MySqlHelper.GetDataSet(CommandType.Text,
                        @"SELECT  * FROM lotterydraw ld 
                            INNER JOIN lotterydrawconfig ldc ON ld.`LastLotteryDrawTime` >= ldc.`BeginTime` 
                            WHERE ld.points<>0 
                            ORDER BY ld.points DESC
                            LIMIT 0,20");
                    if (lotteryDrawChartsDS != null & lotteryDrawChartsDS.Tables.Count > 0)
                    {
                        lotteryDrawChartsDT = lotteryDrawChartsDS.Tables[0];
                    }
                }
                RefreshLotteryDrawData();
            }
        }

        /// <summary>
        /// 更新用户抽奖数据数据
        /// </summary>
        private void RefreshLotteryDrawData()
        {
            lotteryDrawData = MySqlHelper.GetDataList<LotteryDrawModel>(CommandType.Text, "SELECT * FROM lotterydraw");
        }
        #endregion
    }
}