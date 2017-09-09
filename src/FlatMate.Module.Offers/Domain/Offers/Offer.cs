using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using FlatMate.Module.Offers.Domain.Markets;
using FlatMate.Module.Offers.Domain.Products;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatMate.Module.Offers.Domain.Offers
{
    [Table("Offer")]
    public class Offer : DboBase
    {
        [Required]
        public string ExternalId { get; set; }

        [Required]
        public DateTime From { get; set; }

        public string ImageUrl { get; set; }

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

        [Required]
        public DateTime To { get; set; }
    }

    public class OfferDto : DtoBase
    {
        public string ExternalId { get; set; }

        public DateTime From { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public ProductDto Product { get; set; }

        public DateTime To { get; set; }
    }

    [Inject]
    public class OfferMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<Offer, OfferDto>(MapToDto);
        }

        private OfferDto MapToDto(Offer offer, MappingContext ctx)
        {
            return new OfferDto
            {
                From = offer.From,
                Id = offer.Id,
                ExternalId = offer.ExternalId,
                ImageUrl = offer.ImageUrl,
                Price = offer.Price,
                Product = ctx.Mapper.Map<ProductDto>(offer.Product),
                To = offer.To
            };
        }
    }
}
