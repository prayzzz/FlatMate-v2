using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Offers.Domain.Markets;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Domain.Offers
{
    [Table("Product")]
    public class Product : DboBase
    {
        private readonly List<PriceHistoryEntry> _priceHistory = new List<PriceHistoryEntry>();

        [Required]
        public string Brand { get; set; }

        public string Description { get; set; }

        [Required]
        public string ExternalId { get; set; }

        public string ImageUrl { get; set; }

        [ForeignKey(nameof(MarketId))]
        public Market Market { get; set; }

        [Required]
        public int MarketId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; private set; }

        [InverseProperty(nameof(PriceHistoryEntry.Product))]
        public IEnumerable<PriceHistoryEntry> PriceHistory => _priceHistory;

        public string SizeInfo { get; set; }

        public void UpdatePrice(decimal price)
        {
            Price = price;

            var lastHistoryEntry = PriceHistory.OrderByDescending(x => x.Date).FirstOrDefault();
            if (lastHistoryEntry == null || lastHistoryEntry.Price != price)
            {
                _priceHistory.Add(new PriceHistoryEntry(price, this));
            }
        }
    }

    public class ProductDto
    {
        public string Brand { get; set; }

        public string Description { get; set; }

        public string ExternalId { get; set; }

        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        // public IEnumerable<PriceHistoryEntryDbo> PriceHistory => _priceHistory; TODO

        public string SizeInfo { get; set; }
    }

    [Inject]
    public class ProductMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<Product, ProductDto>(MapToDto);
        }

        private ProductDto MapToDto(Product product, MappingContext mappingContext)
        {
            return new ProductDto
            {
                Brand = product.Brand,
                Description = product.Description,
                ExternalId = product.ExternalId,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                Price = product.Price,
                SizeInfo = product.SizeInfo
            };
        }
    }
}