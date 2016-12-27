using System;
using FlatMate.Module.Account.Domain.Models.Interfaces;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Domain.Models
{
    internal class ItemGroup : Entity, IOwnedEntity
    {
        /// <summary>
        /// Constructs an <see cref="ItemGroup"/>
        /// </summary>
        private ItemGroup(int id, string name, UserDto owner, ItemList itemList)
            : base(id)
        {
            Rename(name);

            CreationDate = ModifiedDate = DateTime.Now;
            ItemList = itemList;
            Owner = LastEditor = owner;
            SortIndex = 0;
        }

        public DateTime CreationDate { get; }

        public ItemList ItemList { get; }

        public UserDto LastEditor { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Name { get; private set; }

        public int SortIndex { get; set; }

        public bool IsPublic => ItemList.IsPublic;

        public UserDto Owner { get; }

        /// <summary>
        /// Creates a new <see cref="Item"/> with the given parameters and adds it to the <see cref="ItemGroup"/>.
        /// </summary>
        internal Result<Item> AddItem(string name, UserDto owner)
        {
            return Item.Create(name, owner, this);
        }

        /// <summary>
        /// Creates a new <see cref="ItemGroup"/>
        /// </summary>
        internal static Result<ItemGroup> Create(string name, UserDto owner, ItemList itemList)
        {
            return Create(DefaultId, name, owner, itemList);
        }

        /// <summary>
        /// Creates an exisiting <see cref="ItemGroup"/>
        /// </summary>
        internal static Result<ItemGroup> Create(int id, string name, UserDto owner, ItemList itemList)
        {
            #region Validation

            var result = ValidateName(name);
            if (!result.IsSuccess)
            {
                return new ErrorResult<ItemGroup>(result);
            }

            #endregion

            return new SuccessResult<ItemGroup>(new ItemGroup(id, name, owner, itemList));
        }

        /// <summary>
        /// Renames the group to the given <paramref name="name"/>
        /// </summary>
        internal Result Rename(string name)
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