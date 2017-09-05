using FlatMate.Module.Offers.Domain.Offers;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Api.Jso
{
    public class ProductJso
    {
        public string Brand { get; set; }

        public string Description { get; set; }

        public string ExternalId { get; set; }

        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string SizeInfo { get; set; }
    }

    [Inject]
    public class ProductMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ProductDto, ProductJso>(MapToDto);
        }

        private ProductJso MapToDto(ProductDto dto, MappingContext ctx)
        {
            return new ProductJso
            {
                Brand = dto.Brand,
                Description = dto.Description,
                ExternalId = dto.ExternalId,
                Id = dto.Id,
                ImageUrl = dto.ImageUrl,
                Name = dto.Name,
                Price = dto.Price,
                SizeInfo = dto.SizeInfo
            };
        }
    }
}