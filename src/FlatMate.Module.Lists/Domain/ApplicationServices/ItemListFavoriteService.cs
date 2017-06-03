using System.Threading.Tasks;
using FlatMate.Module.Account.Shared;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Shared.Interfaces;
using prayzzz.Common.Attributes;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.ApplicationServices
{
    [Inject]
    public class ItemListFavoriteService : IItemListFavoriteService
    {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IItemListFavoriteRepository _favoriteRepository;
        private readonly IItemListRepository _itemListRepository;

        /// <summary>
        ///     TODO: Was passiert wenn eine Liste privat wird?
        /// </summary>
        public ItemListFavoriteService(IItemListFavoriteRepository favoriteRepository,
                                       IItemListRepository itemListRepository,
                                       IAuthenticationContext authenticationContext)
        {
            _favoriteRepository = favoriteRepository;
            _itemListRepository = itemListRepository;
            _authenticationContext = authenticationContext;
        }

        private CurrentUser CurrentUser => _authenticationContext.CurrentUser;

        public async Task<Result> SetAsFavorite(int listId)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult(ErrorType.Unauthorized, "Unauthorized");
            }

            // get item list
            var getItemList = await _itemListRepository.GetAsync(listId);
            if (getItemList.IsError)
            {
                return new ErrorResult(getItemList);
            }

            // save
            return await _favoriteRepository.SaveAsync(CurrentUser.Id, getItemList.Data.Id.Value);
        }

        public Task<Result> DeleteFavorite(int listId)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return Task.FromResult<Result>(new ErrorResult(ErrorType.Unauthorized, "Unauthorized"));
            }

            return _favoriteRepository.DeleteAsync(CurrentUser.Id, listId);
        }
    }
}