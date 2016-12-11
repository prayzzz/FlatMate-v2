using System;
using FlatMate.Module.Account.Domain.Entities;
using prayzzz.Common.Result;

namespace FlatMate.Module.Lists.Domain.Entities
{
    internal class Item : Entity
    {
        private int _sortIndex;

        /// <summary>
        /// Constructs a new Item
        /// </summary>
        public Item(string name, User owner)
            : this(DefaultId, name, owner, DateTime.Now, DateTime.Now)
        {
        }

        /// <summary>
        /// Constructs an exisiting Item
        /// </summary>
        public Item(int id, string name, User owner, DateTime creationDate, DateTime modifiedDate)
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
        }

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
    }
}