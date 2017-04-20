using System;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Domain.Models;
using FlatMate.Module.Account.Domain.Repositories;
using FlatMate.Module.Common.DataAccess;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.DataAccess.User
{
    [Inject(DependencyLifetime.Singleton)]
    public class UserRepository : Repository<Domain.Models.User, UserDbo>, IUserRepository, IAuthenticationRepository
    {
        private readonly AccountContext _context;

        public UserRepository(AccountContext context, IMapper mapper) : base(mapper)
        {
            _context = context;
        }

        protected override FlatMateDbContext Context => _context;

        protected override IQueryable<UserDbo> Dbos => _context.Users;

        protected override IQueryable<UserDbo> DbosIncluded => Dbos;

        public async Task<Result<AuthenticationInformation>> GetAuthenticationAsync(int userId)
        {
            var user = await Dbos.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return new ErrorResult<AuthenticationInformation>(ErrorType.NotFound, "Not Found");
            }

            return new SuccessResult<AuthenticationInformation>(Mapper.Map<AuthenticationInformation>(user));
        }

        public async Task<Result<Domain.Models.User>> GetByEmailAsync(string email, StringComparison stringComparison)
        {
            var user = await Dbos.FirstOrDefaultAsync(x => string.Equals(x.Email, email, stringComparison));

            if (user == null)
            {
                return new ErrorResult<Domain.Models.User>(ErrorType.NotFound, "Not Found");
            }

            return new SuccessResult<Domain.Models.User>(Mapper.Map<Domain.Models.User>(user));
        }

        public async Task<Result<Domain.Models.User>> GetByUserNameAsync(string username, StringComparison stringComparison)
        {
            var user = await Dbos.FirstOrDefaultAsync(x => string.Equals(x.UserName, username, stringComparison));

            if (user == null)
            {
                return new ErrorResult<Domain.Models.User>(ErrorType.NotFound, "Not Found");
            }

            return new SuccessResult<Domain.Models.User>(Mapper.Map<Domain.Models.User>(user));
        }

        public async Task<Result> SaveAsync(AuthenticationInformation entity)
        {
            var user = await Dbos.FirstOrDefaultAsync(x => x.Id == entity.UserId);

            if (user == null)
            {
                return new ErrorResult<AuthenticationInformation>(ErrorType.NotFound, "Not Found");
            }

            user.Salt = entity.Salt;
            user.PasswordHash = entity.PasswordHash;

            return await SaveChanges();
        }
    }
}