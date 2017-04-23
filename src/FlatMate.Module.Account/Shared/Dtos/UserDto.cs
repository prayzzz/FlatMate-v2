using System;
using FlatMate.Module.Common.Dtos;

namespace FlatMate.Module.Account.Shared.Dtos
{
    public class UserDto : DtoBase
    {
        public DateTime Created { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }
    }
}