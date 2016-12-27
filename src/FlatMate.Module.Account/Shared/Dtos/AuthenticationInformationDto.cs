namespace FlatMate.Module.Account.Shared.Dtos
{
    public class AuthenticationInformationDto
    {
        public string PasswordHash { get; set; }

        public string Salt { get; set; }

        public int UserId { get; set; }
    }
}