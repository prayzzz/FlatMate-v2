namespace FlatMate.Module.Account.Shared
{
    public class CurrentUser
    {
        private static CurrentUser _anonymous;

        private CurrentUser()
        {
            Id = -1;
        }

        public CurrentUser(int id)
        {
            Id = id;
        }

        public static CurrentUser Anonymous => _anonymous ?? (_anonymous = new CurrentUser());

        public int Id { get; }

        public bool IsAnonymous => Id == -1;
    }
}