using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace TestTaskServer
{
    /// <summary>
    /// 用户信息Dal类
    /// </summary>
    public class UserInfoDal
    {
        /// <summary>
        /// 用户信息Dal的内部字段
        /// </summary>
        private static UserInfoDal mInstance = new UserInfoDal();

        /// <summary>
        /// 用户信息Dal实例
        /// </summary>
        public static UserInfoDal Instance
        {
            get
            {
                return mInstance;
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns>获取用户信息</returns>
        public List<UserInfoModel> GetUserInfoData(String userFlag)
        {
            MySqlParameter[] mySqlParameter = new MySqlParameter[]
            {
                new MySqlParameter("@userFlag",MySqlDbType.VarChar,32)       
            };
            mySqlParameter[0].Value = userFlag;

            return MySqlHelper.GetDataList<UserInfoModel>(CommandType.Text, "SELECT * FROM userinfo WHERE UserFlag= @userFlag ", mySqlParameter);
        }

        /// <summary>
        /// 用户抽奖操作
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        /// <param name="lastLotteryDrawTime">最新抽奖时间</param>
        /// <param name="pointsStr">分数变更方式</param>
        public void UserLotteryDrwaOperation(String userFlag, DateTime lastLotteryDrawTime, String pointsStr)
        {
            //事务更新宝石数量和积分
            String updateDiamondNumberStr = String.Format("UPDATE userinfo SET DiamondNumber=DiamondNumber-100 WHERE UserFlag='{0}';", userFlag);
            String lotteryDrawPointStr = String.Format("UPDATE lotterydraw SET POINTS= " + pointsStr + ",LastLotteryDrawTime='{0}' WHERE UserFlag='{1}';", DateTime.Now, userFlag);

            MySqlHelper.ExecuteTranNonQuery(updateDiamondNumberStr, lotteryDrawPointStr);
        }
    }
}