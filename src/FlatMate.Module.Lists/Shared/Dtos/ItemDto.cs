using System;
using FlatMate.Module.Account.Shared.Dtos;

namespace FlatMate.Module.Lists.Shared.Dtos
{
    public class ItemDto : OwnedDto
    {
        public DateTime Created { get; set; }

        public int? ItemGroupId { get; set; }

        public int ItemListId { get; set; }

        public int LastEditorId { get; set; }

        public DateTime Modified { get; set; }

        public string Name { get; set; }

        public int SortIndex { get; set; }
    }
}