using System;
using System.Collections.Generic;
using FlatMate.Module.Account.Shared.Dtos;

namespace FlatMate.Module.Lists.Shared.Dtos
{
    public class ItemGroupUpdateDto
    {
        public string Name { get; set; }
    }

    public class ItemGroupDto : OwnedDto
    {
        public DateTime CreationDate { get; set; }

        public List<ItemDto> Items { get; set; }

        public UserDto LastEditor { get; set; }

        public int LastEditorId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Name { get; set; }
    }
}