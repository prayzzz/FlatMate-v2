using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using prayzzz.Common.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatMate.Module.Offers.Domain.Products
{
    [Table("ProductFavorite")]
    public class ProductFavorite : DboBase
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }

    public class ProductFavoriteDto : DtoBase
    {
        public int UserId { get; set; }

        public int ProductId { get; set; }

        public ProductDto Product { get; set; }
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