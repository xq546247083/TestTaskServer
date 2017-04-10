using System;
using System.Collections.Generic;
using System.Data;

namespace TestTaskServer
{
    /// <summary>
    /// 用户操作类
    /// </summary>
    public class UserInfoDal
    {
        #region　属性

        private volatile static UserInfoDal _instance = null;
        private static readonly object mLockHelper = new object();
        /// <summary>
        /// 单例
        /// </summary>
        public static UserInfoDal Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (mLockHelper)
                    {
                        if (_instance == null)
                            _instance = new UserInfoDal();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 抽奖操作
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        public void LotteryDraw(string userFlag, DateTime lastLotteryDrawTime)
        {
            //事务更新宝石数量和积分
            string updateDiamondNumberStr = "UPDATE userinfo SET DiamondNumber=DiamondNumber-100 WHERE UserFlag='" + userFlag + "';";
            string lotteryDrawPointStr = "";

            //如果上次抽奖时间在活动时间内，则积分增加，如果抽奖时间不在活动时间内，则积分置为第一次抽奖
            if (LotteryDrawConfigDal.Instance.CheckLotteryTimeConfig(lastLotteryDrawTime))
            {
                lotteryDrawPointStr = "UPDATE lotterydraw SET POINTS=POINTS+10 ,LastLotteryDrawTime='" + DateTime.Now + "' WHERE UserFlag='" + userFlag + "';";
            }
            else
            {
                lotteryDrawPointStr = "UPDATE lotterydraw SET POINTS=10 ,LastLotteryDrawTime='" + DateTime.Now + "' WHERE UserFlag='" + userFlag + "';";
            }

            MySqlHelper.ExecuteTranNonQuery(updateDiamondNumberStr, lotteryDrawPointStr);
        }

        /// <summary>
        /// 检测当前用户标志位是否正确或者宝石是否足够,并返回用户名
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        /// <returns>用户名</returns>
        public string CheckUserInfo(string userFlag)
        {
            //用户名
            string userName = "";
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

            return userName;
        }

        #endregion
    }
}