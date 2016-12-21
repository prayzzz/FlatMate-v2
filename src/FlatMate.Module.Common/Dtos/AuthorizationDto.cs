namespace FlatMate.Module.Common.Dtos
{
    public class AuthorizationDto
    {
        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }

        public bool CanDelete { get; set; }
    }
}