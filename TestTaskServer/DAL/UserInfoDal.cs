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
        /// <param name="commandParameters">数据库操作参数</param>
        public void UserLotteryDrwaOperation(MySqlParameter[] commandParameters)
        {
            //事务更新宝石数量和积分
            String updateDiamondNumberStr = "UPDATE userinfo SET DiamondNumber=DiamondNumber-100 WHERE UserFlag=userFlag@;";
            String lotteryDrawPointStr = "UPDATE lotterydraw SET POINTS= @pointsStr,LastLotteryDrawTime=NOW() WHERE UserFlag= @userFlag;";

            MySqlHelper.ExecuteTranNonQuery(commandParameters, updateDiamondNumberStr, lotteryDrawPointStr);
        }
    }
}