using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Account.Domain;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Services;
using FlatMate.Module.Lists.Shared.Dtos;
using FlatMate.Module.Lists.Shared.Interfaces;
using prayzzz.Common.Attributes;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Domain.ApplicationServices
{
    [Inject]
    public class ItemListService : IItemListService
    {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IItemListAuthorizationService _authorizationService;
        private readonly IItemListRepository _itemListRepository;
        private readonly IUserService _userService;

        public ItemListService(IItemListRepository itemListRepository,
                               IItemListAuthorizationService authorizationService,
                               IUserService userService,
                               IAuthenticationContext authenticationContext)
        {
            _itemListRepository = itemListRepository;
            _authorizationService = authorizationService;
            _userService = userService;
            _authenticationContext = authenticationContext;
        }

        private UserDto CurrentUser => _authenticationContext.CurrentUser;

        public async Task<Result<ItemListDto>> CreateAsync(ItemListDto dto)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            var createResult = ItemList.Create(dto.Name, CurrentUser.Id.Value);
            if (!createResult.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(createResult);
            }

            var itemList = createResult.Data;
            itemList.Description = dto.Description;
            itemList.IsPublic = dto.IsPublic;

            return await SaveAsync(itemList);
        }

        public async Task<Result<ItemListDto>> GetListAsync(int id)
        {
            var itemList = await _itemListRepository.GetAsync(id);

            return itemList.WithDataAs(ModelToDto);
        }

        public async Task<Result<ItemListDto>> UpdateAsync(int id, ItemListDto dto)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            var getResult = await _itemListRepository.GetAsync(id);
            if (!getResult.IsSuccess)
            {
                return new ErrorResult<ItemListDto>(getResult);
            }

            var itemList = getResult.Data;
            if (itemList.OwnerId != CurrentUser.Id)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            itemList.Rename(dto.Name);
            itemList.Description = dto.Description;
            itemList.IsPublic = dto.IsPublic;

            return await SaveAsync(itemList);
        }

        public async Task<IEnumerable<ItemListDto>> GetListsAsync()
        {
            var lists = await _itemListRepository.GetAllAsync();
            return lists.Where(x => x.IsPublic || x.OwnerId == CurrentUser.Id).Select(ModelToDto);
        }

        public async Task<IEnumerable<ItemListDto>> GetListsAsync(int ownerId)
        {
            var lists = await _itemListRepository.GetAllAsync(ownerId);

            if (ownerId != CurrentUser.Id)
            {
                lists = lists.Where(x => x.IsPublic);
            }

            return lists.Select(ModelToDto);
        }

        public async Task<Result> DeleteAsync(int id)
        {
            if (_authenticationContext.IsAnonymous)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            var getResult = await _itemListRepository.GetAsync(id);

            if (!getResult.IsSuccess)
            {
                return getResult;
            }

            var itemList = getResult.Data;
            if (itemList.OwnerId != CurrentUser.Id)
            {
                return new ErrorResult<ItemListDto>(ErrorType.Unauthorized, "Unauthorized");
            }

            return await _itemListRepository.DeleteAsync(id);
        }

        private async Task<Result<ItemListDto>> SaveAsync(ItemList itemList)
        {
            itemList.Modified = DateTime.Now;
            itemList.LastEditor = CurrentUser.Id.Value;

            var result = await _itemListRepository.SaveAsync(itemList);
            return result.WithDataAs(ModelToDto);
        }

        private ItemListDto ModelToDto(ItemList itemList)
        {
            var dto = new ItemListDto();
            dto.Created = itemList.Created;
            dto.Description = itemList.Description;
            dto.LastEditorId = itemList.LastEditor;
            dto.Id = itemList.Id;
            dto.IsPublic = itemList.IsPublic;
            dto.Modified = itemList.Modified;
            dto.Name = itemList.Name;
            dto.OwnerId = itemList.OwnerId;

            return dto;
        }
    }
}