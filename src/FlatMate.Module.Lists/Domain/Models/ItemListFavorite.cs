using FlatMate.Module.Common.Domain.Entities;
using prayzzz.Common.Results;

namespace FlatMate.Module.Lists.Domain.Models
{
    public class ItemListFavorite : Entity
    {
        private static ItemListFavorite _defaultInstance;

        private ItemListFavorite(int? id, int userId, ItemList list) : base(id)
        {
            ItemList = list;
            UserId = userId;
        }

        public static ItemListFavorite Default => _defaultInstance ?? (_defaultInstance = new ItemListFavorite(null, 0, ItemList.Default));

        public int UserId { get; }

        public ItemList ItemList { get; }

        /// <summary>
        ///     Creates a new <see cref="ItemListFavorite" />
        /// </summary>
        public static Result<ItemListFavorite> Create(int userId, ItemList list)
        {
            return Create(null, userId, list);
        }

        /// <summary>
        ///     Creates an exisiting <see cref="ItemListFavorite" />
        /// </summary>
        public static Result<ItemListFavorite> Create(int? id, int userId, ItemList list)
        {
            #region Validation

            if (list == null)
            {
                return new ErrorResult<ItemListFavorite>(ErrorType.ValidationError, "ItemList cannot be null");
            }

            #endregion

            return new SuccessResult<ItemListFavorite>(new ItemListFavorite(id, userId, list));
        }
    }
}