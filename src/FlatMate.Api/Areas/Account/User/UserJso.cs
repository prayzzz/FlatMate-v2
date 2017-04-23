using System.ComponentModel;

namespace FlatMate.Api.Areas.Account.User
{
    public class UserJso
    {
        [ReadOnly(true)]
        public string Email { get; set; }

        [ReadOnly(true)]
        public int? Id { get; set; }

        [ReadOnly(true)]
        public string UserName { get; set; }
    }

    public class UserInfoJso
    {
        [ReadOnly(true)]
        public int? Id { get; set; }

        [ReadOnly(true)]
        public string UserName { get; set; }
    }

    public class CreateUserJso
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }
    }
}