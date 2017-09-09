using FlatMate.Module.Offers.Domain.Offers;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using System;
using System.ComponentModel;

namespace FlatMate.Module.Offers.Api.Jso
{
    public class OfferJso
    {
        public string ExternalId { get; set; }

        public DateTime From { get; set; }

        [ReadOnly(true)]
        public int? Id { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public ProductJso Product { get; set; }

        public DateTime To { get; set; }
    }

    [Inject]
    public class OfferMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<OfferDto, OfferJso>(MapToDto);
        }

        private OfferJso MapToDto(OfferDto dto, MappingContext ctx)
        {
            return new OfferJso
            {
                ExternalId = dto.ExternalId,
                From = dto.From,
                Id = dto.Id,
                ImageUrl = dto.ImageUrl,
                Price = dto.Price,
                Product = ctx.Mapper.Map<ProductJso>(dto.Product),
                To = dto.To
            };
        }
    }
}
