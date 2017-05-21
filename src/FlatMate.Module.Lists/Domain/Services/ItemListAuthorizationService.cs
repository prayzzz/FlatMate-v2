using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Lists.Domain.Models;
using prayzzz.Common.Attributes;

namespace FlatMate.Module.Lists.Domain.Services
{
    public interface IItemListAuthorizationService
    {
        bool CanDelete(IOwnedEntity entity);

        bool CanEdit(Item entity);

        bool CanEdit(ItemList entity);

        bool CanEdit(ItemGroup entity);

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

        public bool CanEdit(ItemList entity)
        {
            if (entity.OwnerId == _authenticationContext.CurrentUser.Id)
            {
                return true;
            }

            return false;
        }

        public bool CanEdit(ItemGroup entity)
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

        public bool CanEdit(Item entity)
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