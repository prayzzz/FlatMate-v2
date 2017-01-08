using System;
using FlatMate.Module.Account.Shared.Dtos;

namespace FlatMate.Module.Lists.Shared.Dtos
{
    public class ItemGroupInputDto
    {
        public string Name { get; set; }
    }

    public class ItemGroupDto : OwnedDto
    {
        public DateTime CreationDate { get; set; }

        public int ItemListId { get; set; }

        public UserDto LastEditor { get; set; }

        public int LastEditorId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Name { get; set; }

        public int SortIndex { get; set; }

        public int ItemCount { get; set; }
    }
}