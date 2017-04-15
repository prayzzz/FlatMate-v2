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

namespace FlatMate.Module.Lists.DataAccess.Items
{
    [Inject]
    public class ItemRepository : IItemRepository
    {
        private readonly ListsContext _context;
        private readonly IMapper _mapper;

        public ItemRepository(ListsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Result> DeleteAsync(int id)
        {
            var dbo = await _context.Items.FindAsync(id);

            if (dbo == null)
            {
                return new ErrorResult(ErrorType.NotFound, "Entity not found");
            }

            _context.Remove(dbo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new ErrorResult(ErrorType.InternalError, e.Message);
            }

            return new SuccessResult();
        }

        public async Task<IEnumerable<Item>> GetAllAsync(int listId)
        {
            var lists = await _context.Items
                                      .Include(g => g.ItemList)
                                      .Include(g => g.ItemGroup)
                                      .Where(g => g.ItemListId == listId)
                                      .ToListAsync();

            return lists.Select(_mapper.Map<Item>).ToList();
        }

        public async Task<Result<Item>> GetAsync(int id)
        {
            var dbo = await _context.Items
                                    .Include(g => g.ItemList)
                                    .Include(g => g.ItemGroup)
                                    .FirstOrDefaultAsync(g => g.Id == id);

            if (dbo == null)
            {
                return new ErrorResult<Item>(ErrorType.NotFound, "Entity not found");
            }

            return new SuccessResult<Item>(_mapper.Map<Item>(dbo));
        }

        public async Task<Result<Item>> SaveAsync(Item entity)
        {
            ItemDbo dbo;
            if (entity.IsSaved)
            {
                dbo = await _context.Items.Where(e => e.Id == entity.Id).SingleOrDefaultAsync();

                if (dbo == null)
                {
                    return new ErrorResult<Item>(ErrorType.NotFound, "Entity not found");
                }
            }
            else
            {
                dbo = _context.Items.Add(new ItemDbo()).Entity;
            }

            _mapper.Map(entity, dbo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new ErrorResult<Item>(ErrorType.InternalError, e.Message);
            }

            return new SuccessResult<Item>(_mapper.Map<Item>(dbo));
        }
    }
}