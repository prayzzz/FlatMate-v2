using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Shared;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Shared.Dtos;
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

        public Task<Result> DeleteFavorite(int listId)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return Task.FromResult(Result.Unauthorized);
            }

            return _favoriteRepository.DeleteAsync(CurrentUser.Id, listId);
        }

        public async Task<IEnumerable<ItemListDto>> GetFavorites()
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return Enumerable.Empty<ItemListDto>();
            }

            return (await _favoriteRepository.GetFavoritesAsync(CurrentUser.Id)).Select(_mapper.Map<ItemListDto>);
        }

        public async Task<Result> SetAsFavorite(int listId)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return Result.Unauthorized;
            }

            // get itemlist
            var (result, itemList) = await _itemListRepository.GetAsync(listId);
            if (result.IsError)
            {
                return result;
            }

            // save
            return await _favoriteRepository.SaveAsync(CurrentUser.Id, itemList.Id);
        }
    }
}