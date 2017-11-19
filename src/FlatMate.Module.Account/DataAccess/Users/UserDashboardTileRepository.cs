using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Account.DataAccess.Users
{
    public interface IUserDashboardTileRepository
    {
        Task<IEnumerable<UserDashboardTileDto>> GetDashboardTiles(int userId);
    }

    [Inject]
    public class UserDashboardTileRepository : IUserDashboardTileRepository
    {
        private readonly AccountDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserDashboardTileRepository(AccountDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDashboardTileDto>> GetDashboardTiles(int userId)
        {
            var tiles = await _dbContext.UserDashboardTiles.Where(t => t.UserId == userId).ToListAsync();
            return tiles.Select(_mapper.Map<UserDashboardTileDto>);
        }
    }
}