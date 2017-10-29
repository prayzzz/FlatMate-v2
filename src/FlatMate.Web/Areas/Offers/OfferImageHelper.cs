using FlatMate.Module.Offers.Domain;

namespace FlatMate.Web.Areas.Offers
{
    // TODO replace with taghelper
    public static class OfferImageHelper
    {
        private static readonly int[] PennyImageSizes = new[] { 312, 382, 468, 624, 936, 1080 };

        public static string Get(string imageUrl, Company company, int width)
        {
            switch (company)
            {
                case Company.Rewe:
                    return $"{imageUrl}?resize={width}px:{width}px";

                case Company.Penny:
                    foreach (var size in PennyImageSizes)
                    {
                        if (size > width)
                        {
                            return imageUrl.Replace("/1080/", $"/{size}/");
                        }
                    }

                    return imageUrl;

                default:
                    return imageUrl;
            }
        }
    }
}
