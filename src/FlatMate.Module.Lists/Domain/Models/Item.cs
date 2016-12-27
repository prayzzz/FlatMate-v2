using System;
using FlatMate.Module.Account.Domain.Models.Interfaces;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Domain.Models
{
    internal class Item : Entity, IOwnedEntity
    {
        /// <summary>
        /// Constructs an exisiting Item
        /// </summary>
        private Item(int id, string name, UserDto owner, ItemGroup itemGroup)
            : base(id)
        {
            Rename(name);

            CreationDate = ModifiedDate = DateTime.Now;
            ItemGroup = itemGroup;
            Owner = LastEditor = owner;
            SortIndex = 0;
        }

        public DateTime CreationDate { get; set; }

        public ItemGroup ItemGroup { get; }

        public UserDto LastEditor { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Name { get; private set; }

        public int SortIndex { get; set; }

        public bool IsPublic => ItemGroup.IsPublic;

        public UserDto Owner { get; }

        /// <summary>
        /// Creates a new <see cref="Item"/>
        /// </summary>
        internal static Result<Item> Create(string name, UserDto owner, ItemGroup itemGroup)
        {
            return Create(DefaultId, name, owner, itemGroup);
        }

        /// <summary>
        /// Creates an exisiting <see cref="Item"/>
        /// </summary>
        internal static Result<Item> Create(int id, string name, UserDto owner, ItemGroup itemGroup)
        {
            #region Validation

            var result = ValidateName(name);
            if (!result.IsSuccess)
            {
                return new ErrorResult<Item>(result);
            }

            #endregion

            return new SuccessResult<Item>(new Item(id, name, owner, itemGroup));
        }

        /// <summary>
        /// Renames the list to the given <paramref name="name"/>
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

        /// <summary>
        /// Sets the given <paramref name="sortIndex"/>
        /// </summary>
        internal Result SetSortIndex(int sortIndex, UserDto currentUserDto)
        {
            if (sortIndex < 0)
            {
                return new ErrorResult(ErrorType.ValidationError, $"{sortIndex} must be >= 0");
            }

            SortIndex = sortIndex;
            LastEditor = currentUserDto;

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