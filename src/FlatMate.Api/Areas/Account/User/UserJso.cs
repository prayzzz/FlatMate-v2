namespace FlatMate.Api.Areas.Account.User
{
    public class UserJso
    {
        public string Email { get; set; }

        public int Id { get; set; }

        public string UserName { get; set; }
    }

    public class UserInfoJso
    {
        public int Id { get; set; }

        public string UserName { get; set; }
    }

    public class CreateUserJso
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }
    }
}