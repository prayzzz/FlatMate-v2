﻿using System.ComponentModel;
using FlatMate.Module.Offers.Domain.Products;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Api.Jso
{
    public class ProductCategoryJso
    {
        [ReadOnly(true)]
        public int? Id { get; set; }

        public string Name { get; set; }

        public int SortWeight { get; set; }
    }

    [Inject]
    public class ProductCategoryMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ProductCategoryDto, ProductCategoryJso>(MapToDto);
        }

        private ProductCategoryJso MapToDto(ProductCategoryDto category, MappingContext ctx)
        {
            return new ProductCategoryJso
            {
                Name = category.Name,
                Id = category.Id,
                SortWeight = category.SortWeight
            };
        }
    }
}