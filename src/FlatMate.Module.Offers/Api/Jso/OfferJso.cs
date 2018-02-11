using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FlatMate.Module.Offers.Domain;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Api
{
    public class OfferJso
    {
        public string ExternalId { get; set; }

        public DateTime From { get; set; }

        [ReadOnly(true)]
        public int? Id { get; set; }

        public string ImageUrl { get; set; }

        public int MarketId { get; set; }

        public decimal Price { get; set; }

        public ProductJso Product { get; set; }

        public int ProductId { get; set; }

        public string SizeInfo { get; set; }

        public DateTime To { get; set; }
    }

    [Inject]
    public class OfferMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<OfferDto, OfferJso>(MapToDto);
            mapper.Configure<OfferPeriodDto, OfferPeriodJso>(MapToDto);
        }

        private OfferPeriodJso MapToDto(OfferPeriodDto period, MappingContext ctx)
        {
            return new OfferPeriodJso
            {
                From = period.From,
                To = period.To,
                Offers = period.Offers.Select(ctx.Mapper.Map<OfferJso>)
            };
        }

        private OfferJso MapToDto(OfferDto dto, MappingContext ctx)
        {
            return new OfferJso
            {
                ExternalId = dto.ExternalId,
                From = dto.From,
                Id = dto.Id,
                ImageUrl = dto.ImageUrl,
                MarketId = dto.MarketId,
                Price = dto.Price,
                Product = ctx.Mapper.Map<ProductJso>(dto.Product),
                ProductId = dto.ProductId,
                SizeInfo = dto.SizeInfo,
                To = dto.To
            };
        }
    }

    public class OfferPeriodJso
    {
        public DateTime From { get; set; }

        public IEnumerable<OfferJso> Offers { get; set; }

        public DateTime To { get; set; }
    }
}