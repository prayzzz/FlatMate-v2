using System;
using FlatMate.Module.Account.Domain.Models.Interfaces;
using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.Models
{
    public class Item : Entity, IOwnedEntity
    {
        /// <summary>
        ///     Constructs an exisiting Item
        /// </summary>
        private Item(int? id, string name, int owner, ItemList list, ItemGroup group) : base(id)
        {
            Rename(name);

            Created = Modified = DateTime.Now;
            ItemList = list;
            ItemGroup = group;
            OwnerId = LastEditorId = owner;
            SortIndex = 0;
        }

        public DateTime Created { get; set; }

        public ItemList ItemList { get; set; }

        public ItemGroup ItemGroup { get; set; }

        public int LastEditorId { get; set; }

        public DateTime Modified { get; set; }

        public string Name { get; private set; }

        public int SortIndex { get; set; }

        public bool IsPublic => ItemList.IsPublic;

        public int OwnerId { get; }

        /// <summary>
        ///     Creates a new <see cref="Item" />
        /// </summary>
        public static Result<Item> Create(string name, int ownerId, ItemList list)
        {
            return Create(null, name, ownerId, list);
        }

        /// <summary>
        ///     Creates a new <see cref="Item" />
        /// </summary>
        public static Result<Item> Create(string name, int ownerId, ItemGroup group)
        {
            return Create(null, name, ownerId, group);
        }

        /// <summary>
        ///     Creates an exisiting <see cref="Item" />
        /// </summary>
        public static Result<Item> Create(int? id, string name, int ownerId, ItemList list)
        {
            #region Validation

            var result = ValidateName(name);
            if (result.IsError)
            {
                return new ErrorResult<Item>(result);
            }

            if (list == null)
            {
                return new ErrorResult<Item>(ErrorType.ValidationError, "ItemList should'nt be null");
            }

            #endregion

            return new SuccessResult<Item>(new Item(id, name, ownerId, list, ItemGroup.Default));
        }

        /// <summary>
        ///     Creates an exisiting <see cref="Item" />
        /// </summary>
        public static Result<Item> Create(int? id, string name, int ownerId, ItemGroup group)
        {
            #region Validation

            var result = ValidateName(name);
            if (!result.IsSuccess)
            {
                return new ErrorResult<Item>(result);
            }

            if (group == null)
            {
                return new ErrorResult<Item>(ErrorType.ValidationError, "ItemList cannot be null");
            }

            #endregion

            return new SuccessResult<Item>(new Item(id, name, ownerId, group.ItemList, group));
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

        protected static Result ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new ErrorResult(ErrorType.ValidationError, $"{nameof(name)} must not be empty.");
            }

            return SuccessResult.Default;
        }
    }
}