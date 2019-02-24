using System.ComponentModel.DataAnnotations;
using FlatMate.Module.Offers.Domain.Products;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Api.Jso
{
    public class ProductFavoriteJso
    {
        [Required]
        public int ProductId { get; set; }
    }

    public class ProductFavoriteMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ProductFavoriteDto, ProductFavoriteJso>(MapToJso);
        }

        private ProductFavoriteJso MapToJso(ProductFavoriteDto dto, MappingContext ctx)
        {
            return new ProductFavoriteJso
            {
                ProductId = dto.ProductId,
            };
        }
    }
}