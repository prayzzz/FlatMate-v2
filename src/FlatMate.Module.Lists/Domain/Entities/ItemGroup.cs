using System;
using System.Collections.Generic;
using FlatMate.Module.Account.Domain.Entities;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Domain.Entities
{
    internal class ItemGroup : Entity
    {
        private readonly List<Item> _items;
        private int _sortIndex;

        /// <summary>
        /// Constructs a new ItemGroup
        /// </summary>
        public ItemGroup(string name, User owner)
            : this(DefaultId, name, owner, DateTime.Now, DateTime.Now)
        {
        }

        /// <summary>
        /// Constructs an exisiting ItemGroup
        /// </summary>
        public ItemGroup(int id, string name, User owner, DateTime creationDate, DateTime modifiedDate)
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
            Owner = owner;
            SortIndex = 0;
            
            _items = new List<Item>();
        }

        public IReadOnlyList<Item> Items => _items;

        public string Name { get; private set; }

        public User Owner { get; }

        public DateTime CreationDate { get; }

        public DateTime ModifiedDate { get; private set; }

        public int SortIndex
        {
            get { return _sortIndex; }
            set
            {
                _sortIndex = value;
                ModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Renames the group to the given <paramref name="name"/>
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
        /// Adds the given <paramref name="item"/> to the group
        /// </summary>
        public Result AddItem(Item item)
        {
            if (item == null)
            {
                return new ErrorResult(ErrorType.ValidationError, $"{nameof(item)} must not be null.");
            }

            _items.Add(item);
            ModifiedDate = DateTime.Now;
            
            return new SuccessResult();
        }
    }
}