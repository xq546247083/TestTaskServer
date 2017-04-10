using System;
using System.Collections.Generic;

namespace TestTaskServer
{
    /// <summary>
    /// 用户操作类
    /// </summary>
    public class UserInfoBll
    {
        #region　属性

        /// <summary>
        /// 用户类实例
        /// </summary>
        private volatile static UserInfoBll Instance = new UserInfoBll();

        #endregion

        #region 方法

        /// <summary>
        /// 抽奖操作
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        public void LotteryDraw(string userFlag)
        {
            LotteryDrawConfigBll.Instance.CheckLotteryTimeConfig();
            string userName = CheckUserInfo(userFlag);

            //获取当前用户的最新抽奖时间,以及判断宝石是否足够
            DateTime lastLotteryDrawTime = LotteryDrawBll.Instance.GetUserLotteryInfo(userFlag, userName);

            //执行抽奖的数据库操作
            string pointsStr = LotteryDrawConfigBll.Instance.CheckLotteryTimeConfig(lastLotteryDrawTime) ? "POINTS+10 " : "10";
            UserInfoDal.Instance.UpdateUserInfo(userFlag, lastLotteryDrawTime, pointsStr);

            //更新用户抽奖数据
            LotteryDrawBll.Instance.UpdateLotteryDrawData(userFlag);
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
            List<UserInfoModel> userInfoDS = UserInfoDal.Instance.GetUserInfoData(userFlag);
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