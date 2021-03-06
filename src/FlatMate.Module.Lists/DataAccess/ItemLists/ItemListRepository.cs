﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.DataAccess.ItemLists
{
    [Inject]
    public class ItemListRepository : Repository<ItemList, ItemListDbo>, IItemListRepository
    {
        private readonly ListsDbContext _dbContext;

        public ItemListRepository(ListsDbContext dbContext,
                                  IMapper mapper,
                                  ILogger<ItemListRepository> logger) : base(mapper, logger)
        {
            _dbContext = dbContext;
        }

        protected override FlatMateDbContext Context => _dbContext;

        protected override IQueryable<ItemListDbo> Dbos => _dbContext.ItemLists;

        protected override IQueryable<ItemListDbo> DbosIncluded => Dbos.Include(x => x.Groups).ThenInclude(x => x.Items);

        public async Task<Result> DeleteWithDependenciesAsync(int listId)
        {
            var favorites = await _dbContext.ItemListFavorites.Where(f => f.ItemListId == listId).ToListAsync();
            _dbContext.ItemListFavorites.RemoveRange(favorites);

            return await DeleteAsync(listId);
        }

        public async Task<IEnumerable<ItemList>> GetAllAsync(int? ownerId)
        {
            var set = _dbContext.ItemLists
                                .Select(itemlist => new ItemListWithMetaData
                                {
                                    Created = itemlist.Created,
                                    Description = itemlist.Description,
                                    Id = itemlist.Id,
                                    IsPublic = itemlist.IsPublic,
                                    ItemCount = itemlist.Groups.SelectMany(g => g.Items).Count(),
                                    LastEditorId = itemlist.LastEditorId,
                                    Modified = itemlist.Modified,
                                    Name = itemlist.Name,
                                    OwnerId = itemlist.OwnerId
                                });

            if (ownerId.HasValue)
            {
                set = set.Where(x => x.OwnerId == ownerId);
            }

            return (await set.ToListAsync()).Select(Mapper.Map<ItemList>);
        }
    }
}