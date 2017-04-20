using System;
using FlatMate.Module.Account.Domain.Models.Interfaces;
using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.Models
{
    public class ItemList : Entity, IOwnedEntity
    {
        private static ItemList _defaultInstance;

        private ItemList(int? id, string name, int ownerId)
            : base(id)
        {
            Rename(name);

            Created = Modified = DateTime.Now;
            Description = string.Empty;
            OwnerId = LastEditorId = ownerId;
        }

        public DateTime Created { get; set; }

        public static ItemList Default => _defaultInstance ?? (_defaultInstance = new ItemList(null, "Default", 0));

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public int LastEditorId { get; set; }

        public DateTime Modified { get; set; }

        public string Name { get; private set; }

        public int OwnerId { get; }

        /// <summary>
        ///     Creates a new <see cref="ItemList" />
        /// </summary>
        public static Result<ItemList> Create(string name, int ownerId)
        {
            return Create(null, name, ownerId);
        }

        /// <summary>
        ///     Creates an exisiting <see cref="ItemList" />
        /// </summary>
        public static Result<ItemList> Create(int? id, string name, int ownerId)
        {
            #region Validation

            var result = ValidateName(name);
            if (!result.IsSuccess)
            {
                return new ErrorResult<ItemList>(result);
            }

            #endregion

            return new SuccessResult<ItemList>(new ItemList(id, name, ownerId));
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