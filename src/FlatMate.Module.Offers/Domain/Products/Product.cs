using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using FlatMate.Module.Offers.Domain.Companies;
using FlatMate.Module.Offers.Domain.Markets;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Domain.Products
{
    [Table("Product")]
    public class Product : DboBase
    {
        private readonly List<PriceHistory> _priceHistoryEntries = new List<PriceHistory>();

        [Required]
        public string Brand { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public CompanyData CompanyData { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public string Description { get; set; }

        public string ExternalProductCategory { get; set; }

        public string ExternalProductCategoryId { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public string Name { get; set; }

        [InverseProperty(nameof(PriceHistory.Product))]
        public IReadOnlyList<PriceHistory> PriceHistoryEntries => _priceHistoryEntries;

        [ForeignKey(nameof(ProductCategoryId))]
        public ProductCategory ProductCategory { get; set; }

        [Required]
        public int ProductCategoryId { get; set; }

        public void UpdatePrice(decimal price, Market market)
        {
            if (price > 0)
            {
                var lastHistoryEntry = PriceHistoryEntries.OrderByDescending(x => x.Date).FirstOrDefault();
                if (lastHistoryEntry == null || lastHistoryEntry.Price != price)
                {
                    _priceHistoryEntries.Add(new PriceHistory(price, this, market));
                }
            }
        }
    }

    public class ProductDto : DtoBase
    {
        public string Brand { get; set; }

        public Company CompanyId { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public ProductCategoryDto ProductCategory { get; set; }

        public int ProductCategoryId { get; set; }
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
                CompanyId = (Company) product.CompanyId,
                Description = product.Description,
                Id = product.Id,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                ProductCategory = product.ProductCategory != null ? ctx.Mapper.Map<ProductCategoryDto>(product.ProductCategory) : null,
                ProductCategoryId = product.ProductCategoryId,
            };
        }
    }
}