using System;
using FlatMate.Module.Common.Dtos;

namespace FlatMate.Module.Account.Dtos
{
    public class UserDto : DtoBase
    {
        public static UserDto Fake { get; } = new UserDto {Id = 1, UserName = "Fake"};

        public DateTime CreationDate { get; set; }

        public string EMail { get; set; }

        public string UserName { get; set; }
    }
}