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