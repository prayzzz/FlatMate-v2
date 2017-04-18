using System.ComponentModel.DataAnnotations;

namespace FlatMate.Module.Common.DataAccess
{
    public abstract class DboBase
    {
        [Key]
        public int Id { get; set; }
    }
}