using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Account.DataAccess.Users
{
    [Table("UserDashboardTile")]
    public class UserDashboardTileDbo
    {
        [Required]
        public string DashboardTile { get; set; }

        [Required]
        public int Id { get; set; }

        [Required]
        public string Parameter { get; set; }

        [ForeignKey("UserId")]
        public UserDbo User { get; set; }

        [Required]
        public int UserId { get; set; }
    }

    public class UserDashboardTileDto
    {
        public string DashboardTile { get; set; }

        public int Id { get; set; }

        public string Parameter { get; set; }

        public int UserId { get; set; }
    }

    [Inject]
    public class UserDashboardTileMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<UserDashboardTileDbo, UserDashboardTileDto>(MapToDto);
        }

        private UserDashboardTileDto MapToDto(UserDashboardTileDbo dbo, MappingContext mappingContext)
        {
            return new UserDashboardTileDto
            {
                Id = dbo.Id,
                Parameter = dbo.Parameter,
                DashboardTile = dbo.DashboardTile,
                UserId = dbo.UserId
            };
        }
    }
}