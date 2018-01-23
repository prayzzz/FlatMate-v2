using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Common.DataAccess;

namespace FlatMate.Module.Account.DataAccess.Users
{
    [Table("UserRestrictedArea")]
    public class UserRestrictedAreaDbo : DboBase
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Area { get; set; }
    }
}