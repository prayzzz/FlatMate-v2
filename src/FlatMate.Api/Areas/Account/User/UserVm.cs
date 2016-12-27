namespace FlatMate.Api.Areas.Account.User
{
    public class UserVm
    {
        public string Email { get; set; }
        public int Id { get; set; }

        public string UserName { get; set; }
    }

    public class CreateUserVm
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }
    }
}