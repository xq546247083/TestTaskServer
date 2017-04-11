using System;
using System.Data;

namespace TestTaskServer
{
    using MySql.Data.MySqlClient;

    /// <summary>
    /// 通用Dal操作类
    /// </summary>
    public class CommonTranDal
    {
        /// <summary>
        /// 通用Dal实例
        /// </summary>
        private static CommonTranDal mInstance = new CommonTranDal();

        /// <summary>
        /// 通用Dal实例
        /// </summary>
        public static CommonTranDal Instance
        {
            get
            {
                return mInstance;
            }
        }

        /// <summary>
        /// 用户抽奖更新数据操作
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <param name="userLotteryInfo">用户抽奖信息</param>
        /// <returns>受影响的行数</returns>
        public Int32 UserLotteryTran(UserInfoModel userInfo, LotteryDrawModel userLotteryInfo)
        {
            MySqlTransaction sqlTransaction = null;

            //打开连接，对用户更新以及用户抽奖更新进行事务处理
            using (MySqlConnection conn = new MySqlConnection(MySqlHelper.Conn))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                try
                {
                    //开始事务
                    sqlTransaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);

                    Int32 returnVal = UserInfoDal.Instance.UpdateUserInfo(sqlTransaction, userInfo);
                    returnVal += LotteryDrawDal.Instance.UpdateLotteryDrawInfo(sqlTransaction, userLotteryInfo);

                    //提交事务
                    sqlTransaction.Commit();

                    return returnVal;
                }
                catch (Exception)
                {
                    //捕获到错误后，先回滚错误，并将错误消息抛出给顶层显示
                    if (sqlTransaction != null)
                    {
                        sqlTransaction.Rollback();
                    }

                    throw;
                }
            }
        }
    }
}