using System;
using FlatMate.Module.Account.Shared.Dtos;

namespace FlatMate.Module.Lists.Shared.Dtos
{
    public class ItemInputDto
    {
        public int ItemListId { get; set; }

        public string Name { get; set; }

        public int? ParentItemId { get; set; }
    }

    public class ItemDto : OwnedDto
    {
        public DateTime CreationDate { get; set; }

        public int ItemListId { get; set; }

        public int LastEditorId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Name { get; set; }

        public int? ParentItemId { get; set; }

        public int SortIndex { get; set; }
    }
}