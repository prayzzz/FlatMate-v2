using System;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.Domain.Entities;
using FlatMate.Module.Common.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Common.DataAccess
{
    public abstract class Repository<TEntity, TDbo> : IRepository<TEntity> where TEntity : Entity where TDbo : DboBase, new()
    {
        protected Repository(IMapper mapper)
        {
            Mapper = mapper;
        }

        protected abstract FlatMateDbContext Context { get; }

        protected abstract IQueryable<TDbo> Dbos { get; }

        protected abstract IQueryable<TDbo> DbosIncluded { get; }

        protected IMapper Mapper { get; }

        public async Task<Result> DeleteAsync(int id)
        {
            var dbo = await Dbos.FirstOrDefaultAsync(x => x.Id == id);

            if (dbo == null)
            {
                return new ErrorResult(ErrorType.NotFound, "Entity not found");
            }

            Context.Remove(dbo);

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new ErrorResult(ErrorType.InternalError, e.Message);
            }

            return new SuccessResult();
        }

        public async Task<Result<TEntity>> GetAsync(int id)
        {
            var dbo = await DbosIncluded.FirstOrDefaultAsync(g => g.Id == id);

            if (dbo == null)
            {
                return new ErrorResult<TEntity>(ErrorType.NotFound, "Entity not found");
            }

            return new SuccessResult<TEntity>(Mapper.Map<TEntity>(dbo));
        }

        public async Task<Result<TEntity>> SaveAsync(TEntity entity)
        {
            TDbo dbo;
            if (entity.IsSaved)
            {
                dbo = await Dbos.Where(e => e.Id == entity.Id).SingleOrDefaultAsync();

                if (dbo == null)
                {
                    return new ErrorResult<TEntity>(ErrorType.NotFound, "Entity not found");
                }
            }
            else
            {
                dbo = Context.Add(new TDbo()).Entity;
            }

            Mapper.Map(entity, dbo);

            var save = await SaveChanges();
            if (save.IsError)
            {
                return new ErrorResult<TEntity>(save);
            }

            return new SuccessResult<TEntity>(Mapper.Map<TEntity>(dbo));
        }

        protected async Task<Result> SaveChanges()
        {
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new ErrorResult(ErrorType.InternalError, e.Message);
            }

            return SuccessResult.Default;
        }
    }
}