using System.ComponentModel.DataAnnotations;
using FlatMate.Module.Account.Domain.Models;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Account.DataAccess.Users
{
    [Inject]
    public class UserMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<UserDbo, User>(DboToEntity);
            mapper.Configure<User, UserDbo>(EntityToDbo);
            mapper.Configure<AuthenticationInformation, UserDbo>(EntityToDbo);
            mapper.Configure<UserDbo, AuthenticationInformation>(DboToAuthEntity);
        }

        private static AuthenticationInformation DboToAuthEntity(UserDbo dbo, MappingContext ctx)
        {
            var createResult = AuthenticationInformation.Create(dbo.PasswordHash, dbo.Salt, dbo.Id);
            if (!createResult.IsSuccess)
            {
                throw new ValidationException(createResult.Message);
            }

            return createResult.Data;
        }

        private static User DboToEntity(UserDbo dbo, MappingContext ctx)
        {
            var createResult = User.Create(dbo.Id, dbo.UserName, dbo.Email);
            if (!createResult.IsSuccess)
            {
                throw new ValidationException(createResult.Message);
            }

            var user = createResult.Data;
            user.Created = dbo.Created;

            if (dbo.IsActivated)
            {
                user.Activate();
            }

            return user;
        }

        private static UserDbo EntityToDbo(AuthenticationInformation entity, UserDbo dbo, MappingContext ctx)
        {
            dbo.PasswordHash = entity.PasswordHash;
            dbo.Salt = entity.Salt;

            return dbo;
        }

        private static UserDbo EntityToDbo(User entity, UserDbo dbo, MappingContext ctx)
        {
            if (entity.Id.HasValue)
            {
                dbo.Id = entity.Id.Value;
            }

            dbo.Created = entity.Created;
            dbo.Email = entity.Email;
            dbo.UserName = entity.UserName;
            dbo.IsActivated = entity.IsActivated;
            dbo.PasswordHash = "";
            dbo.Salt = "";

            return dbo;
        }
    }
}