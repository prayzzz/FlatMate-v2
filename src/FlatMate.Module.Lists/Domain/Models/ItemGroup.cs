using System;
using FlatMate.Module.Account.Domain.Models.Interfaces;
using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.Models
{
    public class ItemGroup : Entity, IOwnedEntity
    {
        private static ItemGroup _defaultInstance;

        private ItemGroup(int? id, string name, int owner, ItemList list) : base(id)
        {
            Rename(name);

            Created = Modified = DateTime.Now;
            ItemList = list;
            OwnerId = LastEditorId = owner;
            SortIndex = 0;
        }

        public DateTime Created { get; set; }

        public static ItemGroup Default => _defaultInstance ?? (_defaultInstance = new ItemGroup(null, "Default", 0, ItemList.Default));

        public bool IsPublic => ItemList.IsPublic;

        public ItemList ItemList { get; }

        public int LastEditorId { get; set; }

        public DateTime Modified { get; set; }

        public string Name { get; private set; }

        public int OwnerId { get; }

        public int SortIndex { get; set; }

        /// <summary>
        ///     Creates a new <see cref="Item" />
        /// </summary>
        public static Result<ItemGroup> Create(string name, int ownerId, ItemList list)
        {
            return Create(null, name, ownerId, list);
        }

        /// <summary>
        ///     Creates an exisiting <see cref="Item" />
        /// </summary>
        public static Result<ItemGroup> Create(int? id, string name, int ownerId, ItemList list)
        {
            #region Validation

            var result = ValidateName(name);
            if (!result.IsSuccess)
            {
                return new ErrorResult<ItemGroup>(result);
            }

            if (list == null)
            {
                return new ErrorResult<ItemGroup>(ErrorType.ValidationError, "ItemList cannot be null");
            }

            #endregion

            return new SuccessResult<ItemGroup>(new ItemGroup(id, name, ownerId, list));
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
            return SuccessResult.Default;
        }

        private static Result ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new ErrorResult(ErrorType.ValidationError, $"{nameof(name)} must not be empty.");
            }

            return SuccessResult.Default;
        }
    }
}