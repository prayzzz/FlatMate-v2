using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Models;
using FlatMate.Module.Lists.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.DataAccess.ItemLists
{
    [Inject]
    public class ItemListRepository : IItemListRepository
    {
        private readonly ListsContext _context;
        private readonly IMapper _mapper;

        public ItemListRepository(ListsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Result<ItemList>> SaveAsync(ItemList entity)
        {
            ItemListDbo dbo;
            if (entity.IsSaved)
            {
                dbo = await _context.ItemLists.Where(e => e.Id == entity.Id).SingleOrDefaultAsync();

                if (dbo == null)
                {
                    return new ErrorResult<ItemList>(ErrorType.NotFound, $"ItemList not found: #{entity.Id}");
                }
            }
            else
            {
                dbo = _context.ItemLists.Add(new ItemListDbo()).Entity;
            }

            _mapper.Map(entity, dbo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new ErrorResult<ItemList>(ErrorType.InternalError, e.Message);
            }

            return new SuccessResult<ItemList>(_mapper.Map<ItemList>(dbo));
        }

        public async Task<Result<ItemList>> GetAsync(int id)
        {
            var dbo = await _context.ItemLists.FindAsync(id);

            if (dbo == null)
            {
                return new ErrorResult<ItemList>(ErrorType.NotFound, $"ItemList #{id} not found");
            }

            return new SuccessResult<ItemList>(_mapper.Map<ItemList>(dbo));
        }

        public async Task<IEnumerable<ItemList>> GetAllAsync()
        {
            var lists = await _context.ItemLists.ToListAsync();
            return lists.Select(_mapper.Map<ItemList>).ToList();
        }

        public async Task<IEnumerable<ItemList>> GetAllAsync(int ownerId)
        {
            var lists = await _context.ItemLists.Where(x => x.OwnerId == ownerId).ToListAsync();
            return lists.Select(_mapper.Map<ItemList>).ToList();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var dbo = await _context.ItemLists.FindAsync(id);

            if (dbo == null)
            {
                return new ErrorResult(ErrorType.NotFound, $"ItemList #{id} not found");
            }

            _context.Remove(dbo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new ErrorResult<ItemList>(ErrorType.InternalError, e.Message);
            }

            return new SuccessResult();
        }
    }
}