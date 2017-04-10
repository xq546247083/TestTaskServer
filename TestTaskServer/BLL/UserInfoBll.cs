using System;
using System.Collections.Generic;

namespace TestTaskServer
{
    /// <summary>
    /// 用户操作类
    /// </summary>
    public class UserInfoBll
    {
        #region 属性

        /// <summary>
        /// 用户Bll类
        /// </summary>
        private static UserInfoBll mInstance = new UserInfoBll();

        /// <summary>
        /// 用户Bll类的实例
        /// </summary>
        public static UserInfoBll Instance
        {
            get
            {
                return mInstance;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 检测当前用户标志位是否正确或者宝石是否足够,并返回用户名
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        /// <returns>用户名</returns>
        public String CheckUserInfo(String userFlag)
        {
            //用户名
            String userName = "";

            List<UserInfoModel> userInfoDs = UserInfoDal.Instance.GetUserInfoData(userFlag);
            if (userInfoDs != null && userInfoDs.Count > 0)
            {
                userName = userInfoDs[0].UserName;
                if (userInfoDs[0].DiamondNumber < 100)
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