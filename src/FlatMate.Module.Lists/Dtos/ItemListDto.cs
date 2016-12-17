using System;
using FlatMate.Module.Account.Dtos;

namespace FlatMate.Module.Lists.Dtos
{
    public class ItemListUpdateDto
    {
        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public string Name { get; set; }
    }

    public class ItemListDto : OwnedDto
    {
        public DateTime CreationDate { get; set; }

        public string Description { get; set; }

        public UserDto LastEditor { get; set; }

        public int LastEditorId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Name { get; set; }
    }
}