﻿using System.Threading.Tasks;
using FlatMate.Module.Account.Domain.Models;
using FlatMate.Module.Common.Domain.Repositories;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<(Result, User)> GetByEmailAsync(string email);

        Task<(Result, User)> GetByUserNameAsync(string userName);
    }
}