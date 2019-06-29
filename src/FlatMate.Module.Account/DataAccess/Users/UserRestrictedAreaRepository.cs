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
        private const string CacheKeyPrefix = "UserRestrictedArea_";
        private readonly IMemoryCache _cache;
        private readonly AccountDbContext _dbContext;

        public UserRestrictedAreaRepository(AccountDbContext dbContext, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public List<string> GetRestrictedAreas(int userId)
        {
            return _cache.GetOrCreate(BuildCacheKey(userId), entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return _dbContext.UserRestrictedAreas.Where(t => t.UserId == userId).Select(a => a.Area).ToList();
            });
        }

        private string BuildCacheKey(int userId)
        {
            return CacheKeyPrefix + userId;
        }
    }
}