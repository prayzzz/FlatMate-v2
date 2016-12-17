using System.Collections.Generic;
using FlatMate.Module.Account.Dtos;

namespace FlatMate.Module.Lists.Dtos
{
    public class ItemGroupDto : OwnedDto
    {
        public List<ItemDto> Items { get; set; }
        public string Name { get; set; }
    }
}