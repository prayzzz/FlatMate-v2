using FlatMate.Module.Account.Shared.Dtos;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Domain.Repositories
{
    public interface IAuthenticationRepository
    {
        Result<AuthenticationInformationDto> GetById(int userId);

        Result Save(AuthenticationInformationDto dto);
    }
}