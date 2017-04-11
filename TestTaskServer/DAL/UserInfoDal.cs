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
        /// 更新用户信息
        /// </summary>
        /// <param name="mySqlTransaction">事务</param>
        /// <param name="userInfoModel">用户信息</param>
        public Int32 UpdateUserInfo(MySqlTransaction mySqlTransaction, UserInfoModel userInfoModel)
        {
            //参数
            MySqlParameter[] mySqlParameter = new MySqlParameter[]
            {
                new MySqlParameter("@userFlag",MySqlDbType.VarChar,32),
                new MySqlParameter("@diamondNumber",MySqlDbType.VarChar,32)       
            };
            mySqlParameter[0].Value = userInfoModel.UserFlag;
            mySqlParameter[1].Value = userInfoModel.DiamondNumber - 100;

            //更新用户信息
            String updateDiamondNumberStr = "UPDATE userinfo SET DiamondNumber=@diamondNumber WHERE UserFlag=@userFlag;";
            return MySqlHelper.ExecuteNonQuery(mySqlTransaction, CommandType.Text, updateDiamondNumberStr, mySqlParameter);
        }
    }
}