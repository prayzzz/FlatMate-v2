using FlatMate.Module.Account.DataAccess.Users;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Account.Api.Jso
{
    public class UserDashboardTileJso
    {
        public string DashboardTile { get; set; }

        public string Parameter { get; set; }
    }

    [Inject]
    public class UserDashboardTileMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<UserDashboardTileDto, UserDashboardTileJso>(MapToDto);
        }

        private UserDashboardTileJso MapToDto(UserDashboardTileDto dbo, MappingContext mappingContext)
        {
            return new UserDashboardTileJso
            {
                Parameter = dbo.Parameter,
                DashboardTile = dbo.DashboardTile
            };
        }
    }
}