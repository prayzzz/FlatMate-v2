using System.ComponentModel;
using FlatMate.Module.Offers.Domain;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Api
{
    public class ProductJso
    {
        public string Brand { get; set; }

        public Company CompanyId { get; set; }

        public string Description { get; set; }

        public string ExternalId { get; set; }

        [ReadOnly(true)]
        public int? Id { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public ProductCategoryJso ProductCategory { get; set; }

        public string SizeInfo { get; set; }
    }

    [Inject]
    public class ProductMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ProductDto, ProductJso>(MapToDto);
        }

        private static ProductJso MapToDto(ProductDto dto, MappingContext ctx)
        {
            return new ProductJso
            {
                Brand = dto.Brand,
                CompanyId = dto.CompanyId,
                Description = dto.Description,
                Id = dto.Id,
                ImageUrl = dto.ImageUrl,
                Name = dto.Name,
                ProductCategory = ctx.Mapper.Map<ProductCategoryJso>(dto.ProductCategory),
                SizeInfo = dto.SizeInfo
            };
        }
    }
}