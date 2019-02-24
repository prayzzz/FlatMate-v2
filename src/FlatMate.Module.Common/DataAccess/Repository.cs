using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common.Domain.Entities;
using FlatMate.Module.Common.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Common.DataAccess
{
    public abstract class Repository
    {
        protected Repository(ILogger logger)
        {
            Logger = logger;
        }

        protected abstract FlatMateDbContext Context { get; }

        protected ILogger Logger { get; }

        protected virtual void AfterSaveChanges()
        {
        }

        protected virtual void BeforeSaveChanges()
        {
        }

        protected async Task<Result> SaveChanges()
        {
            try
            {
                BeforeSaveChanges();
                await Context.SaveChangesAsync();
                AfterSaveChanges();
            }
            catch (Exception e)
            {
                Logger.LogError(0, e, "Error while saving changes");
                return new Result(ErrorType.InternalError, "Datenbankfehler");
            }

            return Result.Success;
        }
    }

    public abstract class Repository<TEntity, TDbo> : Repository, IRepository<TEntity> where TEntity : Entity where TDbo : DboBase, new()
    {
        private readonly Dictionary<int, TEntity> _requestCache;

        protected Repository(IMapper mapper, ILogger logger) : base(logger)
        {
            Mapper = mapper;

            _requestCache = new Dictionary<int, TEntity>();
        }

        protected abstract IQueryable<TDbo> Dbos { get; }

        protected abstract IQueryable<TDbo> DbosIncluded { get; }

        protected IMapper Mapper { get; }

        public async Task<Result> DeleteAsync(int id)
        {
            var dbo = await DbosIncluded.FirstOrDefaultAsync(x => x.Id == id);

            if (dbo == null)
            {
                return new Result(ErrorType.NotFound, "Entity not found");
            }

            Context.Remove(dbo);

            var save = await SaveChanges();
            if (save.IsError)
            {
                return new Result(save);
            }

            return Result.Success;
        }

        public (Result, TEntity) Get(int id)
        {
            if (_requestCache.TryGetValue(id, out var cachedEntity))
            {
                return (Result.Success, cachedEntity);
            }

            var dbo = DbosIncluded.FirstOrDefault(g => g.Id == id);

            if (dbo == null)
            {
                return (Result.NotFound, null);
            }

            var entity = Mapper.Map<TEntity>(dbo);
            _requestCache[dbo.Id] = entity;

            return (Result.Success, entity);
        }

        public async Task<(Result, TEntity)> GetAsync(int id)
        {
            if (_requestCache.TryGetValue(id, out var cachedEntity))
            {
                return (Result.Success, cachedEntity);
            }

            var dbo = await DbosIncluded.FirstOrDefaultAsync(g => g.Id == id);

            if (dbo == null)
            {
                return (Result.NotFound, null);
            }

            var entity = Mapper.Map<TEntity>(dbo);
            _requestCache[dbo.Id] = entity;

            return (Result.Success, entity);
        }

        public async Task<(Result, TEntity)> SaveAsync(TEntity entity)
        {
            TDbo dbo;
            if (entity.IsSaved)
            {
                dbo = await Dbos.Where(e => e.Id == entity.Id).SingleOrDefaultAsync();

                if (dbo == null)
                {
                    return (new Result(ErrorType.InternalError, "Entity not found"), null);
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
                return (new Result(save), null);
            }

            return (Result.Success, Mapper.Map<TEntity>(dbo));
        }

        protected override void BeforeSaveChanges()
        {
            base.BeforeSaveChanges();
            _requestCache.Clear();
        }
    }
}