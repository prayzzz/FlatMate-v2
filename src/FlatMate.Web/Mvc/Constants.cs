namespace FlatMate.Web.Mvc
{
    public static class Constants
    {
        public static readonly TempDataConstants TempData = new TempDataConstants();

        public class TempDataConstants
        {
            public readonly string Result = "tmp_result";
        }
    }
}