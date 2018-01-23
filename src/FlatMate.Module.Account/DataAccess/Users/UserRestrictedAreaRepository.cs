using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public UserRestrictedAreaRepository(AccountDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<string> GetRestrictedAreas(int userId)
        {
            return _dbContext.UserRestrictedAreas.Where(t => t.UserId == userId).Select(a => a.Area).ToList();
        }
    }
}