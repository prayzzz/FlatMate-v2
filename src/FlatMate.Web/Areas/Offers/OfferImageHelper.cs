using FlatMate.Module.Offers.Api;
using FlatMate.Module.Offers.Domain;

namespace FlatMate.Web.Areas.Offers
{
    // TODO replace with taghelper
    public static class OfferImageHelper
    {
        private static readonly int[] PennyImageSizes = new[] { 312, 382, 468, 624, 936, 1080 };

        public static string Get(OfferJso offer, int width)
        {
            if (offer.Market == null || offer.Market.Company == null)
            {
                return offer.ImageUrl;
            }

            switch (offer.Market.Company.Id)
            {
                case (int)Company.Rewe:
                    return $"{offer.ImageUrl}?resize={width}px:{width}px";

                case (int)Company.Penny:
                    foreach (var size in PennyImageSizes)
                    {
                        if (size > width)
                        {
                            return offer.ImageUrl.Replace("/1080/", $"/{size}/");
                        }
                    }

                    return offer.ImageUrl;

                default:
                    return offer.ImageUrl;
            }
        }
    }
}
