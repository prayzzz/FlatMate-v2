using FlatMate.Module.Account.Shared.Interfaces;
using prayzzz.Common.Attributes;

namespace FlatMate.Module.Lists.Domain.Services
{
    public interface IItemListAuthorizationService
    {
        bool CanDelete(IOwnedEntity entity);

        bool CanEdit(IOwnedEntity entity);

        bool CanRead(IOwnedEntity entity);
    }

    [Inject]
    public class ItemListAuthorizationService : IItemListAuthorizationService
    {
        private readonly IAuthenticationContext _authenticationContext;

        public ItemListAuthorizationService(IAuthenticationContext authenticationContext)
        {
            _authenticationContext = authenticationContext;
        }

        public bool CanDelete(IOwnedEntity entity)
        {
            if (entity.OwnerId == _authenticationContext.CurrentUser.Id)
            {
                return true;
            }

            return false;
        }

        public bool CanEdit(IOwnedEntity entity)
        {
            if (entity.IsPublic)
            {
                return true;
            }

            if (entity.OwnerId == _authenticationContext.CurrentUser.Id)
            {
                return true;
            }

            return false;
        }

        public bool CanRead(IOwnedEntity entity)
        {
            if (entity.IsPublic)
            {
                return true;
            }

            if (entity.OwnerId == _authenticationContext.CurrentUser.Id)
            {
                return true;
            }

            return false;
        }
    }
}