using System;
using FlatMate.Module.Account.Shared.Dtos;

namespace FlatMate.Module.Lists.Shared.Dtos
{
    public class ItemListInputDto
    {
        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public string Name { get; set; }
    }

    public class ItemListDto : OwnedDto
    {
        public DateTime CreationDate { get; set; }

        public string Description { get; set; }

        public int LastEditorId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Name { get; set; }

        public ItemListMetaDto Meta { get; set; }
    }
}