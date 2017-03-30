using FlatMate.Module.Account.Domain;
using FlatMate.Module.Account.Shared.Dtos;
using prayzzz.Common.Attributes;

namespace FlatMate.Module.Lists.Domain.Services
{
    public interface IItemListAuthorizationService
    {
        bool CanDelete(OwnedDto getResultData);

        bool CanRead(OwnedDto dto);
    }

    [Inject]
    public class ItemListAuthorizationService : IItemListAuthorizationService
    {
        private readonly IAuthenticationContext _authenticationContext;

        public ItemListAuthorizationService(IAuthenticationContext authenticationContext)
        {
            _authenticationContext = authenticationContext;
        }

        public bool CanDelete(OwnedDto dto)
        {
            if (dto.OwnerId == _authenticationContext.CurrentUser.Id)
            {
                return true;
            }

            return false;
        }

        public bool CanRead(OwnedDto dto)
        {
            if (dto.IsPublic)
            {
                return true;
            }

            if (dto.OwnerId == _authenticationContext.CurrentUser.Id)
            {
                return true;
            }

            return false;
        }
    }
}