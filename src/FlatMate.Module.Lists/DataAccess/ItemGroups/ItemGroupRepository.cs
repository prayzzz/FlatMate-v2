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

namespace FlatMate.Module.Lists.DataAccess.ItemGroups
{
    [Inject]
    public class ItemGroupRepository : IItemGroupRepository
    {
        private readonly ListsContext _context;
        private readonly IMapper _mapper;

        public ItemGroupRepository(ListsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Result> DeleteAsync(int id)
        {
            var dbo = await _context.ItemGroups.FindAsync(id);

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

        public async Task<IEnumerable<ItemGroup>> GetAllAsync(int listId)
        {
            var lists = await _context.ItemGroups
                                      .Include(g => g.ItemList)
                                      .Where(g => g.ItemListId == listId)
                                      .ToListAsync();

            return lists.Select(_mapper.Map<ItemGroup>).ToList();
        }

        public async Task<Result<ItemGroup>> GetAsync(int id)
        {
            var dbo = await _context.ItemGroups
                                    .Include(g => g.ItemList)
                                    .FirstOrDefaultAsync(g => g.Id == id);

            if (dbo == null)
            {
                return new ErrorResult<ItemGroup>(ErrorType.NotFound, "Entity not found");
            }

            return new SuccessResult<ItemGroup>(_mapper.Map<ItemGroup>(dbo));
        }

        public async Task<Result<ItemGroup>> SaveAsync(ItemGroup entity)
        {
            ItemGroupDbo dbo;
            if (entity.IsSaved)
            {
                dbo = await _context.ItemGroups.Where(e => e.Id == entity.Id).SingleOrDefaultAsync();

                if (dbo == null)
                {
                    return new ErrorResult<ItemGroup>(ErrorType.NotFound, "Entity not found");
                }
            }
            else
            {
                dbo = _context.ItemGroups.Add(new ItemGroupDbo()).Entity;
            }

            _mapper.Map(entity, dbo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new ErrorResult<ItemGroup>(ErrorType.InternalError, e.Message);
            }

            return new SuccessResult<ItemGroup>(_mapper.Map<ItemGroup>(dbo));
        }
    }
}