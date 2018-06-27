using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Shared;
using FlatMate.Module.Common;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Services;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using Microsoft.EntityFrameworkCore.Update;
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
        private readonly IItemListAuthorizationService _authorizationService;
        private readonly IItemGroupRepository _itemGroupRepository;
        private readonly IItemListRepository _itemListRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

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

        public async Task<(Result, ItemListDto)> CreateAsync(ItemListDto dto)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return (Result.Unauthorized, null);
            }

            // create ItemList
            var (result, itemList) = ItemList.Create(dto.Name, CurrentUser.Id);
            if (result.IsError)
            {
                return (result, null);
            }

            // set optional data
            itemList.Description = dto.Description;
            itemList.IsPublic = dto.IsPublic;

            return await SaveAsync(itemList);
        }

        public async Task<Result> DeleteListAsync(int listId)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return Result.Unauthorized;
            }

            // get ItemList
            var (result, itemList) = await _itemListRepository.GetAsync(listId);
            if (result.IsError)
            {
                return result;
            }

            // check permission
            if (!_authorizationService.CanDelete(itemList))
            {
                return Result.NotFound;
            }

            return await _itemListRepository.DeleteWithDependenciesAsync(listId);
        }

        public async Task<(Result, ItemListDto)> GetListAsync(int listId)
        {
            // get ItemList
            var (result, itemList) = await _itemListRepository.GetAsync(listId);
            if (result.IsError)
            {
                return (result, null);
            }

            // check permission
            if (!_authorizationService.CanRead(itemList))
            {
                return (Result.NotFound, null);
            }

            return (Result.Success, _mapper.Map<ItemListDto>(itemList));
        }

        public async Task<IEnumerable<ItemListDto>> GetListsAsync(int? ownerId)
        {
            return (await _itemListRepository.GetAllAsync(ownerId)).Where(l => _authorizationService.CanRead(l))
                                                                   .Select(_mapper.Map<ItemListDto>);
        }

        public async Task<(Result, ItemListDto)> UpdateAsync(int listId, ItemListDto dto)
        {
            // the user must be logged in
            if (CurrentUser.IsAnonymous)
            {
                return (Result.Unauthorized, null);
            }

            // get ItemList
            var (result, itemList) = await _itemListRepository.GetAsync(listId);
            if (result.IsError)
            {
                return (result, null);
            }

            // check permission
            if (!_authorizationService.CanEdit(itemList))
            {
                return (Result.Unauthorized, null);
            }

            // update data
            itemList.Rename(dto.Name);
            itemList.Description = dto.Description;
            itemList.IsPublic = dto.IsPublic;

            return await SaveAsync(itemList);
        }

        private async Task<(Result, ItemListDto)> SaveAsync(ItemList itemList)
        {
            itemList.Modified = DateTime.Now;
            itemList.LastEditorId = CurrentUser.Id;

            var (result, savedItemList) = await _itemListRepository.SaveAsync(itemList);
            if (result.IsError)
            {
                return (result, null);
            }

            return (Result.Success, _mapper.Map<ItemListDto>(savedItemList));
        }
    }
}