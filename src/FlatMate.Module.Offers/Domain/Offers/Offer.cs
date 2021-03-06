﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using FlatMate.Module.Offers.Domain.Companies;
using FlatMate.Module.Offers.Domain.Markets;
using FlatMate.Module.Offers.Domain.Products;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Domain.Offers
{
    [Table("Offer")]
    public class Offer : DboBase
    {
        public string BasePrice { get; set; }

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
        [Column(TypeName = "decimal(7,2)")]
        public decimal Price { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Required]
        public int ProductId { get; set; }

        public string SizeInfo { get; set; }

        [Required]
        public DateTime To { get; set; }
    }

    public class OfferDto : DtoBase
    {
        public string BasePrice { get; set; }

        public string ExternalId { get; set; }

        public DateTime From { get; set; }

        public string ImageUrl { get; set; }

        public int MarketId { get; set; }

        public decimal Price { get; set; }

        public ProductDto Product { get; set; }

        public int ProductId { get; set; }

        public string SizeInfo { get; set; }

        public DateTime To { get; set; }
    }

    [Inject]
    public class OfferMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<Offer, OfferDto>(MapToDto);
        }

        private static OfferDto MapToDto(Offer offer, MappingContext ctx)
        {
            return new OfferDto
            {
                BasePrice = offer.BasePrice,
                From = offer.From,
                Id = offer.Id,
                ExternalId = offer.ExternalId,
                ImageUrl = offer.ImageUrl,
                MarketId = offer.MarketId,
                Price = offer.Price,
                Product = ctx.Mapper.Map<ProductDto>(offer.Product),
                ProductId = offer.ProductId,
                SizeInfo = offer.SizeInfo,
                To = offer.To
            };
        }
    }

    public class OfferPeriodDto : DtoBase
    {
        public CompanyDto Company { get; set; }

        public DateTime From { get; set; }

        public IEnumerable<OfferDto> Offers { get; set; }

        public DateTime To { get; set; }
    }
}