using System.Collections.Generic;
using FlatMate.Module.Account.Domain.Repositories;
using FlatMate.Module.Account.Shared.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Enums;
using prayzzz.Common.Result;

namespace FlatMate.Module.Account.DataAccess.Repositories
{
    [Inject(DependencyLifetime.Singleton)]
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly Dictionary<int, AuthenticationInformationDto> _auth;

        public AuthenticationRepository()
        {
            _auth = new Dictionary<int, AuthenticationInformationDto>();
            _auth.Add(UserDto.Fake.Id, new AuthenticationInformationDto { PasswordHash = "yW85+CZVyIHSMlqgBx71N4h7Ku+qE32f3cWZbBmGSrUS9eXCmMvKIbchXd7G9SxsPus48ximHcKm6YG/UmAh7Q==", Salt = "CwvDrFV38guhMmq0P0rp+w==", UserId = UserDto.Fake.Id });
        }

        public Result<AuthenticationInformationDto> GetById(int userId)
        {
            if (_auth.TryGetValue(userId, out var authInfo))
            {
                return new SuccessResult<AuthenticationInformationDto>(authInfo);
            }

            return new ErrorResult<AuthenticationInformationDto>(ErrorType.NotFound, "Not Found");
        }

        public Result Save(AuthenticationInformationDto dto)
        {
            if (_auth.ContainsKey(dto.UserId))
            {
                _auth[dto.UserId] = dto;
            }
            else
            {
                _auth.Add(dto.UserId, dto);
            }

            return new SuccessResult<AuthenticationInformationDto>(dto);
        }
    }
}