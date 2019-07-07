using System;
using FlatMate.Module.Account.Shared.Dtos;

namespace FlatMate.Module.Lists.Shared.Dtos
{
    public class ItemListDto : OwnedDto
    {
        public DateTime Created { get; set; }

        public string Description { get; set; }

        public int LastEditorId { get; set; }

        public ItemListMetaDto Meta { get; set; } = new ItemListMetaDto();

        public DateTime Modified { get; set; }

        public string Name { get; set; }
    }
}