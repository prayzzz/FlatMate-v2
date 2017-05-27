using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Shared;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Services;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.ApplicationServices
{
    /// <summary>
    ///     Does orchestration for all task related to ItemLists.
    ///     Returns <see cref="ErrorType.Unauthorized" />, if the task needs loggedin user.
    ///     Returns <see cref="ErrorType.NotFound" />, if the current user is not allowed to execute the task on the entity.
    /// </summary>
    [Inject]
    public partial class ItemListService : IItemListService
    {
        private readonly IMapper _mapper;
        private readonly IItemListAuthorizationService _authorizationService;
        private readonly IItemGroupRepository _itemGroupRepository;
        private readonly IItemListRepository _itemListRepository;
        private readonly IItemRepository _itemRepository;

        public ItemListService(IItemListRepository itemListRepository,
                               IItemGroupRepository itemGroupRepository,
                               IItemRepository itemRepository,
                               IItemListAuthorizationService authorizationService,
                               IMapper mapper)
        {
            _itemListRepository = itemListRepository;
            _itemGroupRepository = itemGroupRepository;
            _itemRepository = itemRepository;
            _authorizationService = authorizationService;
            _mapper = mapper;
        }

        private CurrentUser CurrentUser => _authorizationService.CurrentUser;

        public async Task<Result<ItemListDto>> CreateAsync(ItemListDto dto)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            // create ItemList
            var createList = ItemList.Create(dto.Name, CurrentUser.Id);
            if (createList.IsError)
            {
                return new ErrorResult<ItemListDto>(createList);
            }

            // set optional data
            var itemList = createList.Data;
            itemList.Description = dto.Description;
            itemList.IsPublic = dto.IsPublic;

            return await SaveAsync(itemList);
        }

        public async Task<Result> DeleteListAsync(int listId)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult(ErrorType.Unauthorized, "Unauthorized");
            }

            // get ItemList
            var getList = await _itemListRepository.GetAsync(listId);
            if (getList.IsError)
            {
                return getList;
            }

            // check permission
            if (!_authorizationService.CanDelete(getList.Data))
            {
                return new ErrorResult(ErrorType.NotFound, "Entity not found");
            }

            return await _itemListRepository.DeleteAsync(listId);
        }

        public async Task<Result<ItemListDto>> GetListAsync(int listId)
        {
            // get ItemList
            var getList = await _itemListRepository.GetAsync(listId);
            if (getList.IsError)
            {
                return new ErrorResult<ItemListDto>(getList);
            }

            // check permission
            if (!_authorizationService.CanRead(getList.Data))
            {
                return new ErrorResult<ItemListDto>(ErrorType.NotFound, "Entity not found");
            }

            return getList.WithDataAs(_mapper.Map<ItemListDto>);
        }

        public async Task<IEnumerable<ItemListDto>> GetListsAsync(int? ownerId, bool favoritesOnly)
        {
            return (await _itemListRepository.GetAllAsync(ownerId, favoritesOnly)).Where(l => _authorizationService.CanRead(l))
                                                                   .Select(_mapper.Map<ItemListDto>);
        }

        public async Task<Result<ItemListDto>> UpdateAsync(int listId, ItemListDto dto)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            // get ItemList
            var getResult = await _itemListRepository.GetAsync(listId);
            if (getResult.IsError)
            {
                return new ErrorResult<ItemListDto>(getResult);
            }

            // check permission
            if (!_authorizationService.CanEdit(getResult.Data))
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            // update data
            var itemList = getResult.Data;
            itemList.Rename(dto.Name);
            itemList.Description = dto.Description;
            itemList.IsPublic = dto.IsPublic;

            return await SaveAsync(itemList);
        }

        private Task<Result<ItemListDto>> SaveAsync(ItemList itemList)
        {
            itemList.Modified = DateTime.Now;
            itemList.LastEditorId = CurrentUser.Id;

            return _itemListRepository.SaveAsync(itemList).WithResultDataAs(_mapper.Map<ItemListDto>);
        }
    }
}