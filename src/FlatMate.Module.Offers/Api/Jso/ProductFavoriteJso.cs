using FlatMate.Module.Offers.Domain.Products;
using prayzzz.Common.Mapping;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlatMate.Module.Offers.Api
{
    public class ProductFavoriteJso
    {
        public ProductJso Product { get; internal set; }

        [Required]
        public int ProductId { get; set; }

        [ReadOnly(true)]
        public int UserId { get; set; }
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
                Product = ctx.Mapper.Map<ProductJso>(dto.Product),
                ProductId = dto.ProductId,
                UserId = dto.UserId
            };
        }
    }
}
