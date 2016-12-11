using System;
using System.Collections.Generic;
using FlatMate.Module.Account.Domain.Entities;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Domain.Entities
{
    internal class ItemList : Entity
    {
        private readonly List<ItemGroup> _groups;
        private string _description;

        /// <summary>
        /// Constructs a new ItemList
        /// </summary>
        public ItemList(string name, User owner)
            : this(DefaultId, name, owner, DateTime.Now, DateTime.Now)
        {
        }

        /// <summary>
        /// Constructs an existing ItemList
        /// </summary>
        public ItemList(int id, string name, User owner, DateTime creationDate, DateTime modifiedDate)
            : base(id)
        {
            #region Validation
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }
            #endregion

            var result = Rename(name);
            if (!result.IsSuccess)
            {
                throw new ArgumentException(result.ErrorMessage);
            }

            CreationDate = creationDate;
            ModifiedDate = modifiedDate;
            Description = string.Empty;
            Owner = owner;
            
            _groups = new List<ItemGroup>();
        }

        public string Name { get; private set; }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                ModifiedDate = DateTime.Now;
            }
        }

        public IReadOnlyList<ItemGroup> Groups => _groups;

        public User Owner { get; }

        public DateTime CreationDate { get; }

        public DateTime ModifiedDate { get; private set; }

        /// <summary>
        /// Renames the list to the given <paramref name="name"/>
        /// </summary>
        public Result Rename(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new ErrorResult(ErrorType.ValidationError, $"{nameof(name)} must not be empty.");
            }

            Name = name;
            ModifiedDate = DateTime.Now;

            return new SuccessResult();
        }

        /// <summary>
        /// Adds the given <paramref name="group"/> to the list
        /// </summary>
        public Result AddGroup(ItemGroup group)
        {
            if (group == null)
            {
                return new ErrorResult(ErrorType.ValidationError, $"{nameof(group)} must not be null.");
            }

            _groups.Add(group);
            ModifiedDate = DateTime.Now;

            return new SuccessResult();
        }
    }
}
