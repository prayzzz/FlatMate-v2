using System;
using System.ComponentModel.DataAnnotations;
using FlatMate.Module.Common.DataAccess;

namespace FlatMate.Module.Account.DataAccess.User
{
    public class UserDbo : DboBase
    {
        [Required]
        public DateTime Created { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Salt { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}