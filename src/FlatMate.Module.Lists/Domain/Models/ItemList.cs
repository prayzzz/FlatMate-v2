using System;
using FlatMate.Module.Account.Domain.Models.Interfaces;
using FlatMate.Module.Account.Shared.Dtos;
using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Domain.Models
{
    internal class ItemList : Entity, IOwnedEntity
    {
        /// <summary>
        /// Constructs an <see cref="ItemList"/>
        /// </summary>
        private ItemList(int id, string name, UserDto owner)
            : base(id)
        {
            Rename(name);

            CreationDate = ModifiedDate = DateTime.Now;
            Description = string.Empty;
            Owner = LastEditor = owner;
        }

        public DateTime CreationDate { get; set; }

        public string Description { get; set; }

        public UserDto LastEditor { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Name { get; private set; }

        public bool IsPublic { get; set; }

        public UserDto Owner { get; }

        /// <summary>
        /// Creates a new <see cref="ItemList"/>
        /// </summary>
        public static Result<ItemList> Create(string name, UserDto owner)
        {
            return Create(DefaultId, name, owner);
        }

        /// <summary>
        /// Creates an exisiting <see cref="ItemList"/>
        /// </summary>
        public static Result<ItemList> Create(int id, string name, UserDto owner)
        {
            #region Validation

            var result = ValidateName(name);
            if (!result.IsSuccess)
            {
                return new ErrorResult<ItemList>(result);
            }

            #endregion

            return new SuccessResult<ItemList>(new ItemList(id, name, owner));
        }

        /// <summary>
        /// Renames the list to the given <paramref name="name"/>
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