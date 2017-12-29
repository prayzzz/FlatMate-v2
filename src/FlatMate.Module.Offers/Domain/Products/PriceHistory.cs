using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Domain
{
    [Table("PriceHistory")]
    public class PriceHistory : DboBase
    {
        private DateTime _date;

        public PriceHistory()
        {
        }

        public PriceHistory(decimal price, Product product, Market market) : this(price, DateTime.Now, product, market)
        {
        }

        private PriceHistory(decimal price, DateTime date, Product product, Market market)
        {
            Price = price;
            Date = date;
            Product = product;
            Market = market;
        }

        [Required]
        public DateTime Date
        {
            get => _date;
            set => _date = value.Date;
        }

        [ForeignKey(nameof(MarketId))]
        public Market Market { get; set; }

        [Required]
        public int MarketId { get; set; }

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

        public MarketDto Market { get; set; }

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

        private static PriceHistoryDto MapToDto(PriceHistory history, MappingContext ctx)
        {
            return new PriceHistoryDto
            {
                Date = history.Date,
                Market = ctx.Mapper.Map<MarketDto>(history.Market),
                Price = history.Price,
                Product = ctx.Mapper.Map<ProductDto>(history.Product)
            };
        }
    }
}