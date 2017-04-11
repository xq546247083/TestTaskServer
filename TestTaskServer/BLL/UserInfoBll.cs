using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
        /// 缓存用户信息数据
        /// </summary>
        private List<UserInfoModel> userInfoData = new List<UserInfoModel>();

        /// <summary>
        /// 用户抽奖数据缓存锁
        /// </summary>
        private ReaderWriterLockSlim userInfoLock = new ReaderWriterLockSlim();

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
        /// 获取用户
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        /// <returns>用户名</returns>
        public UserInfoModel GetUserInfo(String userFlag)
        {
            //获取用户数据,先从缓存中读取用户数据,如果没有，则从数据库读取数据
            UserInfoModel currentUserInfo = null;

            userInfoLock.ExitReadLock();
            try
            {
                currentUserInfo = userInfoData.FirstOrDefault(r => r.UserFlag.Equals(userFlag));
            }
            finally
            {
                userInfoLock.ExitReadLock();
            }

            if (currentUserInfo == null)
            {
                currentUserInfo = UserInfoDal.Instance.GetUserInfoData(userFlag).FirstOrDefault();
            }

            //给缓存的用户数据添加当前用户
            if (currentUserInfo != null)
            {
                userInfoLock.EnterWriteLock();
                try
                {
                    userInfoData.Add(currentUserInfo);
                }
                finally
                {
                    userInfoLock.ExitWriteLock();
                }
            }
            else
            {
                throw new Exception(CommonConst.NoUserFlag);
            }

            return currentUserInfo;
        }

        /// <summary>
        /// 检测当前用户的宝石数量是否足够
        /// </summary>
        /// <param name="currentUserInfo">用户</param>
        public void CheckUserDiamond(UserInfoModel currentUserInfo)
        {
            if (currentUserInfo.DiamondNumber < 100)
            {
                throw new Exception(CommonConst.NoEnoughDiamond);
            }
        }

        /// <summary>
        /// 更新当前用户的宝石数量
        /// </summary>
        /// <param name="userFlag">用户标志</param>
        internal void UpdateUserDiamondData(string userFlag)
        {
            foreach (UserInfoModel userInfo in userInfoData)
            {
                if (userInfo.UserFlag != userFlag)
                {
                    continue;
                }

                userInfoLock.EnterWriteLock();
                try
                {
                    //修改当前用户的宝石数量
                    userInfo.DiamondNumber -= 10;

                    break;
                }
                finally
                {
                    userInfoLock.ExitWriteLock();
                }
            }
        }

        #endregion
    }
}