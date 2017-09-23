using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatMate.Module.Offers.Domain.Products
{
    [Table("PriceHistory")]
    public class PriceHistory : DboBase
    {
        private DateTime _date;

        public PriceHistory()
        {
        }

        public PriceHistory(decimal price, Product product) : this(price, DateTime.Now, product)
        {
        }

        private PriceHistory(decimal price, DateTime date, Product product)
        {
            Price = price;
            Date = date;
            Product = product;
        }

        [Required]
        public DateTime Date { get => _date; set => _date = value.Date; }

        [Required]
        public decimal Price { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Required]
        public int ProductId { get; set; }
    }

    public class PriceHistoryDto : DtoBase
    {
        public DateTime Date { get; set; }

        public decimal Price { get; set; }

        public ProductDto Product { get; set; }
    }

    [Inject]
    public class PriceHistoryMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<PriceHistory, PriceHistoryDto>(MapToDto);
        }

        private PriceHistoryDto MapToDto(PriceHistory history, MappingContext ctx)
        {
            return new PriceHistoryDto
            {
                Date = history.Date,
                Price = history.Price,
                Product = ctx.Mapper.Map<ProductDto>(history.Product)
            };
        }
    }
}
