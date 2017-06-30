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
            return entity.OwnerId == CurrentUser.Id;
        }

        public bool CanDelete(Item entity)
        {
            return entity.IsPublic || entity.OwnerId == CurrentUser.Id;
        }

        public bool CanDelete(ItemGroup entity)
        {
            return entity.IsPublic || entity.OwnerId == CurrentUser.Id;
        }

        public bool CanEdit(ItemList entity)
        {
            return entity.OwnerId == CurrentUser.Id;
        }

        public bool CanEdit(ItemGroup entity)
        {
            return entity.IsPublic || entity.OwnerId == CurrentUser.Id;
        }

        public bool CanEdit(Item entity)
        {
            return entity.IsPublic || entity.OwnerId == CurrentUser.Id;
        }

        public bool CanRead(IOwnedEntity entity)
        {
            return entity.IsPublic || entity.OwnerId == CurrentUser.Id;
        }
    }
}