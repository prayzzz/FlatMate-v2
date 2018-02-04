using System.Threading.Tasks;
using FlatMate.Module.Account.Domain.Models;
using FlatMate.Module.Common;
using prayzzz.Common.Results;

namespace FlatMate.Module.Account.Domain.Repositories
{
    public interface IAuthenticationRepository
    {
        Task<(Result, AuthenticationInformation)> GetAuthenticationAsync(int userId);

        Task<Result> SaveAsync(AuthenticationInformation authInfo);
    }
}