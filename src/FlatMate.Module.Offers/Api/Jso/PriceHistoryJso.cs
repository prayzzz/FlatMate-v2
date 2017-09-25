using FlatMate.Module.Offers.Domain;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using System;

namespace FlatMate.Module.Offers.Api.Jso
{
    public class PriceHistoryJso
    {
        public DateTime Date { get; set; }

        public decimal Price { get; set; }

        public int ProductId { get; set; }
    }

    [Inject]
    public class PriceHistoryMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<PriceHistoryDto, PriceHistoryJso>(MapToDto);
        }

        private PriceHistoryJso MapToDto(PriceHistoryDto history, MappingContext ctx)
        {
            return new PriceHistoryJso
            {
                Date = history.Date,
                Price = history.Price,
                ProductId = history.Product.Id.Value
            };
        }
    }
}