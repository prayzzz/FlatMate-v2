using FlatMate.Module.Account.Shared;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Lists.Domain.Models;
using prayzzz.Common.Attributes;

namespace FlatMate.Module.Lists.Domain.Services
{
    public interface IItemListAuthorizationService
    {
        CurrentUser CurrentUser { get; }

        bool CanDelete(Item entity);

        bool CanDelete(ItemList entity);

        bool CanDelete(ItemGroup entity);

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

        public CurrentUser CurrentUser => _authenticationContext.CurrentUser;

        public bool CanDelete(ItemList entity)
        {
            if (entity.OwnerId == CurrentUser.Id)
            {
                return true;
            }

            return false;
        }

        public bool CanDelete(Item entity)
        {
            if (entity.IsPublic)
            {
                return true;
            }

            if (entity.OwnerId == CurrentUser.Id)
            {
                return true;
            }

            return false;
        }

        public bool CanDelete(ItemGroup entity)
        {
            if (entity.IsPublic)
            {
                return true;
            }

            if (entity.OwnerId == CurrentUser.Id)
            {
                return true;
            }

            return false;
        }

        public bool CanEdit(ItemList entity)
        {
            if (entity.OwnerId == CurrentUser.Id)
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

            if (entity.OwnerId == CurrentUser.Id)
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

            if (entity.OwnerId == CurrentUser.Id)
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

            if (entity.OwnerId == CurrentUser.Id)
            {
                return true;
            }

            return false;
        }
    }
}