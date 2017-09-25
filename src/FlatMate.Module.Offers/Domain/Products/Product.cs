using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FlatMate.Module.Offers.Domain
{
    [Table("Product")]
    public class Product : DboBase
    {
        private readonly List<PriceHistory> _priceHistoryEntries = new List<PriceHistory>();

        [Required]
        public string Brand { get; set; }

        public string Description { get; set; }

        [Required]
        public string ExternalId { get; set; }

        public string ExternalProductCategory { get; set; }

        public string ExternalProductCategoryId { get; set; }

        public string ImageUrl { get; set; }

        [ForeignKey(nameof(MarketId))]
        public Market Market { get; set; }

        [Required]
        public int MarketId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; private set; }

        [InverseProperty(nameof(PriceHistory.Product))]
        public IReadOnlyList<PriceHistory> PriceHistoryEntries => _priceHistoryEntries;

        [ForeignKey(nameof(ProductCategoryId))]
        public ProductCategory ProductCategory { get; set; }

        [Required]
        public int ProductCategoryId { get; set; }

        public string SizeInfo { get; set; }

        public void UpdatePrice(decimal price)
        {
            Price = price;

            if (price > 0)
            {
                var lastHistoryEntry = PriceHistoryEntries.OrderByDescending(x => x.Date).FirstOrDefault();
                if (lastHistoryEntry == null || lastHistoryEntry.Price != price)
                {
                    _priceHistoryEntries.Add(new PriceHistory(price, this));
                }
            }
        }
    }

    public class ProductDto : DtoBase
    {
        public string Brand { get; set; }

        public string Description { get; set; }

        public string ExternalId { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public ProductCategoryDto ProductCategory { get; set; }

        public string SizeInfo { get; set; }
    }

    [Inject]
    public class ProductMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<Product, ProductDto>(MapToDto);
        }

        private ProductDto MapToDto(Product product, MappingContext ctx)
        {
            return new ProductDto
            {
                Brand = product.Brand,
                Description = product.Description,
                ExternalId = product.ExternalId,
                Id = product.Id,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                Price = product.Price,
                ProductCategory = ctx.Mapper.Map<ProductCategoryDto>(product.ProductCategory),
                SizeInfo = product.SizeInfo
            };
        }
    }
}