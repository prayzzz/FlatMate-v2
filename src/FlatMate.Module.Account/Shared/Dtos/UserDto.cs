using System;
using FlatMate.Module.Common.Dtos;

namespace FlatMate.Module.Account.Shared.Dtos
{
    public class UserDto : DtoBase
    {
        public DateTime CreationDate { get; set; }

        public string Email { get; set; }

        public static UserDto Fake { get; } = new UserDto { Id = 1, UserName = "Fake" };

        public string UserName { get; set; }
    }
}