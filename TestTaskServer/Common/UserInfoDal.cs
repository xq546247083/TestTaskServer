using System;
using System.Collections.Generic;
using System.Data;

namespace TestTaskServer
{
    public class UserInfoDal
    {
        /// <summary>
        /// 用户信息Dal实例
        /// </summary>
        public volatile static UserInfoDal Instance = new UserInfoDal();

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns>获取用户信息</returns>
        public List<UserInfoModel> GetUserInfoData(string userFlag)
        {
            return MySqlHelper.GetDataList<UserInfoModel>(CommandType.Text, "SELECT * FROM userinfo WHERE UserFlag='" + userFlag + "'");
        }

        public void UpdateUserInfo(string userFlag, DateTime lastLotteryDrawTime, string pointsStr)
        {
            //事务更新宝石数量和积分
            string updateDiamondNumberStr = "UPDATE userinfo SET DiamondNumber=DiamondNumber-100 WHERE UserFlag='" + userFlag + "';";
            string lotteryDrawPointStr = "UPDATE lotterydraw SET POINTS= " + pointsStr + ",LastLotteryDrawTime='" + DateTime.Now + "' WHERE UserFlag='" + userFlag + "';";

            MySqlHelper.ExecuteTranNonQuery(updateDiamondNumberStr, lotteryDrawPointStr);
        }
    }
}