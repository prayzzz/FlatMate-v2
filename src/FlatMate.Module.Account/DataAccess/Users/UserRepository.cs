using System;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Domain.Models;
using FlatMate.Module.Account.Domain.Repositories;
using FlatMate.Module.Common.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.DataAccess.Users
{
    [Inject]
    public class UserRepository : Repository<User, UserDbo>, IUserRepository, IAuthenticationRepository
    {
        private readonly AccountDbContext _dbContext;

        public UserRepository(AccountDbContext dbContext,
                              IMapper mapper,
                              ILogger<UserRepository> logger) : base(mapper, logger)
        {
            _dbContext = dbContext;
        }

        protected override FlatMateDbContext Context => _dbContext;

        protected override IQueryable<UserDbo> Dbos => _dbContext.Users;

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

        public async Task<Result<User>> GetByEmailAsync(string email)
        {
            var user = await Dbos.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return new ErrorResult<User>(ErrorType.NotFound, "Not Found");
            }

            return new SuccessResult<User>(Mapper.Map<User>(user));
        }

        public async Task<Result<User>> GetByUserNameAsync(string userName)
        {
            var user = await Dbos.FirstOrDefaultAsync(x => x.UserName == userName);
            if (user == null)
            {
                return new ErrorResult<User>(ErrorType.NotFound, "Not Found");
            }

            return new SuccessResult<User>(Mapper.Map<User>(user));
        }

        public async Task<Result> SaveAsync(AuthenticationInformation authInfo)
        {
            var user = await Dbos.FirstOrDefaultAsync(x => x.Id == authInfo.UserId);
            if (user == null)
            {
                return new ErrorResult<AuthenticationInformation>(ErrorType.NotFound, "Not Found");
            }

            Mapper.Map(authInfo, user);
            return await SaveChanges();
        }
    }
}