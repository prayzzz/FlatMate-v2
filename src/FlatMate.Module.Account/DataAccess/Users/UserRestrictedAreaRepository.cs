using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using prayzzz.Common.Attributes;

namespace FlatMate.Module.Account.DataAccess.Users
{
    public interface IUserRestrictedAreaRepository
    {
        List<string> GetRestrictedAreas(int userId);
    }

    [Inject]
    public class UserRestrictedAreaRepository : IUserRestrictedAreaRepository
    {
        private readonly AccountDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _entryOptions;

        public UserRestrictedAreaRepository(AccountDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;

            _entryOptions = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) };
        }

        public List<string> GetRestrictedAreas(int userId)
        {
            return _cache.GetOrCreate(userId, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return _dbContext.UserRestrictedAreas.Where(t => t.UserId == userId).Select(a => a.Area).ToList();
            });
        }
    }
}