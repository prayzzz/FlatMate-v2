using System.Threading.Tasks;
using FlatMate.Module.Account.Shared;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Shared.Interfaces;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.ApplicationServices
{
    [Inject]
    public class ItemListFavoriteService : IItemListFavoriteService
    {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IItemListFavoriteRepository _favoriteRepository;
        private readonly IItemListRepository _itemListRepository;
        private readonly IMapper _mapper;

        /// <summary>
        ///     TODO: Was passiert wenn eine Liste privat wird?
        /// </summary>
        /// <param name="favoriteRepository"></param>
        /// <param name="itemListRepository"></param>
        /// <param name="authenticationContext"></param>
        /// <param name="mapper"></param>
        public ItemListFavoriteService(IItemListFavoriteRepository favoriteRepository,
                                       IItemListRepository itemListRepository,
                                       IAuthenticationContext authenticationContext,
                                       IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _itemListRepository = itemListRepository;
            _authenticationContext = authenticationContext;
            _mapper = mapper;
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

            // create favorite
            var createFavorite = ItemListFavorite.Create(CurrentUser.Id, getItemList.Data);
            if (createFavorite.IsError)
            {
                return new ErrorResult(createFavorite);
            }

            // save
            return await _favoriteRepository.SaveAsync(createFavorite.Data);
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