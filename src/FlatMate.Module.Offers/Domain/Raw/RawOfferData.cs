using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatMate.Module.Offers.Domain
{
    [Table("RawOfferData")]
    public class RawOfferData : DboBase
    {
        [ForeignKey(nameof(CompanyId))]
        public CompanyData Company { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public string Data { get; set; }

        [Required]
        public string Hash { get; set; }
    }

    public class RawOfferDataDto : DtoBase
    {
        public CompanyDto Company { get; set; }

        public int CompanyId { get; set; }

        public DateTime Created { get; set; }

        public string Data { get; set; }
    }

    [Inject]
    public class RawOfferDataMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<RawOfferData, RawOfferDataDto>(MapToDto);
        }

        private RawOfferDataDto MapToDto(RawOfferData offer, MappingContext ctx)
        {
            return new RawOfferDataDto()
            {
                Company = ctx.Mapper.Map<CompanyDto>(offer.Company),
                CompanyId = offer.CompanyId,
                Data = offer.Data,
                Created = offer.Created,
                Id = offer.Id,
            };
        }
    }
}
