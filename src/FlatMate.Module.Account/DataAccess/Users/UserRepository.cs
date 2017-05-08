using System;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Domain.Models;
using FlatMate.Module.Account.Domain.Repositories;
using FlatMate.Module.Common.DataAccess;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.DataAccess.Users
{
    [Inject]
    public class UserRepository : Repository<User, UserDbo>, IUserRepository, IAuthenticationRepository
    {
        private readonly AccountDbContext _dbContext;

        public UserRepository(AccountDbContext dbContext, IMapper mapper) : base(mapper)
        {
            _dbContext = dbContext;

            CreateFake(); // TODO remove
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

        public async Task<Result<User>> GetByEmailAsync(string email, StringComparison stringComparison)
        {
            var user = await Dbos.FirstOrDefaultAsync(x => string.Equals(x.Email, email, stringComparison));
            if (user == null)
            {
                return new ErrorResult<User>(ErrorType.NotFound, "Not Found");
            }

            return new SuccessResult<User>(Mapper.Map<User>(user));
        }

        public async Task<Result<User>> GetByUserNameAsync(string userName, StringComparison stringComparison)
        {
            var user = await Dbos.FirstOrDefaultAsync(x => string.Equals(x.UserName, userName, stringComparison));
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

        private void CreateFake()
        {
            if (Dbos.Any(u => u.UserName == "fake"))
            {
                return;
            }

            var userDbo = new UserDbo
            {
                UserName = "Fake",
                Created = DateTime.Now,
                Email = "fake@fake.de",
                PasswordHash = "nmiVz6ju+do07UxMxjkYuiAj4s8CA0OB0AJhQulGBl6PG2xcxbvKbTdE/4C4uvv6upAORT/dJuf6ySEARSVsxg==",
                Salt = "E1Uzdr7JKZ96JrOItvWFzg=="
            };

            Context.Add(userDbo);
            Context.SaveChanges();
        }
    }
}