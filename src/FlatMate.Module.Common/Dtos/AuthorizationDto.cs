namespace FlatMate.Module.Common.Dtos
{
    public class AuthorizationDto
    {
        public bool CanDelete { get; set; }
        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }
    }
}