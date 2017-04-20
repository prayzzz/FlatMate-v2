﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FlatMate.Module.Common.Domain.Repositories;
using FlatMate.Module.Lists.Domain.Models;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.Repositories
{
    public interface IItemGroupRepository : IRepository<ItemGroup>
    {
        Task<IEnumerable<ItemGroup>> GetAllAsync(int listId);

        Task<Result<ItemGroup>> GetAsync(int id);

        Task<Result<ItemGroup>> SaveAsync(ItemGroup itemGroup);
    }
}