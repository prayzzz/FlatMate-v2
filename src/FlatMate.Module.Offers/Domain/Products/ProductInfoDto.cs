namespace FlatMate.Module.Offers.Domain
{
    public class ProductInfoDto
    {
        public int Id { get; set; }

        public string Brand { get; set; }

        public int OfferCount { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public int CompanyId { get; set; }

        public string ImageUrl { get; set; }

        public string SizeInfo { get; set; }
    }
}