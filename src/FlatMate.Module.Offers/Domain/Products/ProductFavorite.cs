using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Domain.Products
{
    [Table("ProductFavorite")]
    public class ProductFavorite : DboBase
    {
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int UserId { get; set; }
    }

    public class ProductFavoriteDto : DtoBase
    {
        public ProductDto Product { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }
    }

    public class ProductFavoriteMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ProductFavorite, ProductFavoriteDto>(MapToDto);
        }

        private ProductFavoriteDto MapToDto(ProductFavorite fav, MappingContext ctx)
        {
            return new ProductFavoriteDto
            {
                Id = fav.Id,
                Product = ctx.Mapper.Map<ProductDto>(fav.Product),
                ProductId = fav.ProductId,
                UserId = fav.UserId
            };
        }
    }
}