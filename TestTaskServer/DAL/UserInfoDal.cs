using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace TestTaskServer
{
    /// <summary>
    /// 用户操作类
    /// </summary>
    public class UserInfoDal
    {
        //单例
        private volatile static UserInfoDal _instance = null;
        private static readonly object lockHelper = new object();
        public static UserInfoDal Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockHelper)
                    {
                        if (_instance == null)
                            _instance = new UserInfoDal();
                    }
                }
                return _instance;
            }
        }

        //获取排行榜的互斥锁
        private static readonly object lockObj = new object();

        //抽奖配置表
        private List<LotteryDrawConfigModel> lotteryDrawConfigDS = null;

        //排行榜
        private DataTable lotteryDrawChartsDT = null;
        //排行榜最低分
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

        /// <summary>
        /// 抽奖操作
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        public void LotteryDraw(string userFlag)
        {
            try
            {
                //检测是否在抽奖时间范围内
                if (lotteryDrawConfigDS == null || lotteryDrawConfigDS.Count == 0)
                {
                    lotteryDrawConfigDS = MySqlHelper.GetDataList<LotteryDrawConfigModel>(CommandType.Text, "SELECT * FROM LotterydrawConfig");
                }

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

                //用户名
                string userName = "";
                //检测当前用户标志位是否正确或者宝石是否足够
                List<UserInfoModel> userInfoDS = MySqlHelper.GetDataList<UserInfoModel>(CommandType.Text, "SELECT * FROM userinfo WHERE UserFlag='" + userFlag + "'");
                if (userInfoDS != null & userInfoDS.Count > 0)
                {
                    userName = userInfoDS[0].UserName;
                    if (userInfoDS[0].DiamondNumber < 100)
                        throw new Exception(CommonConst.NoEnoughDiamond);
                }
                else
                {
                    throw new Exception(CommonConst.NoUserFlag);
                }

                //查询是否存在抽奖关联数据，不存在则插入数据
                int currenUserPoints = -1;
                List<LotteryDrawModel> lotteryDrawDS = MySqlHelper.GetDataList<LotteryDrawModel>(CommandType.Text, "SELECT * FROM lotterydraw WHERE UserFlag='" + userFlag + "'");
                if (!(lotteryDrawDS != null & lotteryDrawDS.Count > 0))
                {
                    MySqlHelper.ExecuteNonQuery(CommandType.Text, "INSERT INTO lotterydraw (userName,userflag) VALUES ('" + userName + "','" + userFlag + "')");
                    currenUserPoints = 0;
                }
                else
                {
                    currenUserPoints = lotteryDrawDS[0].Points;
                }

                //事务更新宝石数量和积分
                string updateDiamondNumberStr = "UPDATE userinfo SET DiamondNumber=DiamondNumber-100 WHERE UserFlag='" + userFlag + "';";
                string lotteryDrawPointStr = "UPDATE lotterydraw SET POINTS=POINTS+10 WHERE UserFlag='" + userFlag + "';";
                MySqlHelper.ExecuteTranNonQuery(updateDiamondNumberStr, lotteryDrawPointStr);

                //异步更新排行榜
                ThreadPool.QueueUserWorkItem((obj) => { GetCharts(1, currenUserPoints); });
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取排行榜
        /// </summary>
        /// <param name="userFlag">传入用户标志位，返回用户的积分</param>
        /// <returns></returns>
        public DataTable GetCharts(ref string userFlag)
        {
            //查询积分
            List<LotteryDrawModel> lotteryDrawDS = MySqlHelper.GetDataList<LotteryDrawModel>(CommandType.Text, "SELECT * FROM lotterydraw WHERE UserFlag='" + userFlag + "'");
            if (lotteryDrawDS != null & lotteryDrawDS.Count > 0)
            {
                userFlag = lotteryDrawDS[0].Points.ToString();
            }
            else
            {
                userFlag = "0";
            }

            GetCharts(0);
            return lotteryDrawChartsDT;
        }

        /// <summary>
        /// 更新排行榜数据
        /// </summary>
        /// <returns></returns>
        private void GetCharts(int flag, int currenUserPoints = 0)
        {
            lock (lockObj)
            {
                //如果操作为查询排行榜且排行榜为空 或者 如果排行榜人数未满20人或者排行榜最低分小于了当前的分数，则更新排行榜
                if ((lotteryDrawChartsDT == null && flag == 0) ||
                    (((lotteryDrawChartsDT != null && lotteryDrawChartsDT.Rows.Count < 20) || (lowestPoints < currenUserPoints + 10 && lotteryDrawChartsDT != null)) && flag == 1))
                {
                    DataSet lotteryDrawChartsDS = MySqlHelper.GetDataSet(CommandType.Text, "SELECT  * FROM lotterydraw ORDER BY points desc LIMIT 0,20");
                    if (lotteryDrawChartsDS != null & lotteryDrawChartsDS.Tables.Count > 0)
                    {
                        lotteryDrawChartsDT = lotteryDrawChartsDS.Tables[0];
                    }
                }
            }
        }
    }
}