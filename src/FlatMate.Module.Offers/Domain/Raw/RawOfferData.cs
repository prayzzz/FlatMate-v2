using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using FlatMate.Module.Offers.Domain.Markets;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Domain.Raw
{
    [Table("RawOfferData")]
    public class RawOfferData : DboBase
    {
        [Required]
        public DateTime Created { get; set; }

        [Required]
        public string Data { get; set; }

        [Required]
        public string Hash { get; set; }

        [ForeignKey(nameof(MarketId))]
        public Market Market { get; set; }

        [Required]
        public int MarketId { get; set; }
    }

    public class RawOfferDataDto : DtoBase
    {
        public DateTime Created { get; set; }

        public string Data { get; set; }

        public MarketDto Market { get; set; }

        public int MarketId { get; set; }
    }

    [Inject]
    public class RawOfferDataMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<RawOfferData, RawOfferDataDto>(MapToDto);
        }

        private static RawOfferDataDto MapToDto(RawOfferData offer, MappingContext ctx)
        {
            return new RawOfferDataDto
            {
                Created = offer.Created,
                Data = offer.Data,
                Id = offer.Id,
                Market = ctx.Mapper.Map<MarketDto>(offer.Market),
                MarketId = offer.MarketId
            };
        }
    }
}