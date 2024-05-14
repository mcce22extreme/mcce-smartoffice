﻿using Mcce.SmartOffice.Api.Accessors;
using Mcce.SmartOffice.Api.Models;

namespace Mcce.SmartOffice.Api.Managers
{
    public interface IAccountManager
    {
        Task<UserInfoModel> GetAccountInfo();
    }

    public class AccountManager : IAccountManager
    {
        private readonly IAuthContextAccessor _httpContextAccessor;

        public AccountManager(IAuthContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
        }

        public Task<UserInfoModel> GetAccountInfo()
        {
            var userInfo = _httpContextAccessor.GetUserInfo();

            return Task.FromResult(userInfo);
        }
    }
}