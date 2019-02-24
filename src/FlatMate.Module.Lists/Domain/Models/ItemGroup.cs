using System;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.Models
{
    public class ItemGroup : Entity, IOwnedEntity
    {
        private static ItemGroup _defaultInstance;

        private ItemGroup(int? id, string name, int owner, ItemList list, DateTime created) : base(id)
        {
            Rename(name);

            Created = Modified = created;
            ItemList = list;
            OwnerId = LastEditorId = owner;
            SortIndex = 0;
        }

        public DateTime Created { get; }

        public static ItemGroup Default => _defaultInstance ?? (_defaultInstance = new ItemGroup(null, "Default", 0, ItemList.Default, DateTime.UtcNow));

        public bool IsPublic => ItemList.IsPublic;

        public ItemList ItemList { get; }

        public int LastEditorId { get; set; }

        public DateTime Modified { get; set; }

        public string Name { get; private set; }

        public int OwnerId { get; }

        public int SortIndex { get; set; }

        /// <summary>
        ///     Creates a new <see cref="ItemGroup" />
        /// </summary>
        public static (Result, ItemGroup) Create(string name, int ownerId, ItemList list)
        {
            return Create(null, name, ownerId, list, DateTime.UtcNow);
        }

        /// <summary>
        ///     Creates an exisiting <see cref="ItemGroup" />
        /// </summary>
        public static (Result, ItemGroup) Create(int? id, string name, int ownerId, ItemList list, DateTime created)
        {
            #region Validation

            var result = ValidateName(name);
            if (result.IsError)
            {
                return (result, null);
            }

            if (list == null)
            {
                return (new Result(ErrorType.ValidationError, "ItemList cannot be null"), null);
            }

            #endregion

            return (Result.Success, new ItemGroup(id, name, ownerId, list, created));
        }

        /// <summary>
        ///     Renames the list to the given <paramref name="name" />
        /// </summary>
        public Result Rename(string name)
        {
            var validationResult = ValidateName(name);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            Name = name;
            return Result.Success;
        }

        private static Result ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new Result(ErrorType.ValidationError, $"{nameof(name)} must not be empty.");
            }

            return Result.Success;
        }
    }
}