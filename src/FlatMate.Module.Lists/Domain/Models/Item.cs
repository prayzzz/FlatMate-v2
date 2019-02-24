using System;
using FlatMate.Module.Account.Shared.Interfaces;
using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.Models
{
    public class Item : Entity, IOwnedEntity
    {
        private Item(int? id, string name, int owner, ItemList list, ItemGroup group, DateTime created) : base(id)
        {
            Rename(name);

            Created = Modified = created;
            ItemList = list;
            ItemGroup = group;
            OwnerId = LastEditorId = owner;
            SortIndex = 0;
        }

        public DateTime Created { get; }

        public bool IsPublic => ItemList.IsPublic;

        public ItemGroup ItemGroup { get; }

        public ItemList ItemList { get; }

        public int LastEditorId { get; set; }

        public DateTime Modified { get; set; }

        public string Name { get; private set; }

        public int OwnerId { get; }

        public int SortIndex { get; set; }

        /// <summary>
        ///     Creates a new <see cref="Item" />
        /// </summary>
        public static (Result, Item) Create(string name, int ownerId, ItemList list)
        {
            return Create(null, name, ownerId, list, DateTime.UtcNow);
        }

        /// <summary>
        ///     Creates a new <see cref="Item" />
        /// </summary>
        public static (Result, Item) Create(string name, int ownerId, ItemGroup group)
        {
            return Create(null, name, ownerId, group, DateTime.UtcNow);
        }

        /// <summary>
        ///     Creates an exisiting <see cref="Item" />
        /// </summary>
        public static (Result, Item) Create(int? id, string name, int ownerId, ItemList list, DateTime created)
        {
            #region Validation

            var result = ValidateName(name);
            if (result.IsError)
            {
                return (result, null);
            }

            if (list == null)
            {
                return (new Result(ErrorType.ValidationError, "ItemList can't be null"), null);
            }

            #endregion

            return (Result.Success, new Item(id, name, ownerId, list, ItemGroup.Default, created));
        }

        /// <summary>
        ///     Creates an exisiting <see cref="Item" />
        /// </summary>
        public static (Result, Item) Create(int? id, string name, int ownerId, ItemGroup group, DateTime created)
        {
            #region Validation

            var result = ValidateName(name);
            if (!result.IsSuccess)
            {
                return (result, null);
            }

            if (group == null)
            {
                return (new Result(ErrorType.ValidationError, "ItemList can't be null"), null);
            }

            #endregion

            return (Result.Success, new Item(id, name, ownerId, group.ItemList, group, created));
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