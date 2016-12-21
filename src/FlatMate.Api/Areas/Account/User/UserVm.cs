using System;

namespace FlatMate.Api.Areas.Account.User
{
    public class UserVm
    {
        public int Id { get; set; }
        
        public DateTime CreationDate { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }
    }
}