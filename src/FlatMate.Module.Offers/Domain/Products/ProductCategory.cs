using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Domain.Products
{
    public enum ProductCategoryEnum
    {
        Other = 1,
        Fruits = 2,
        Convenience = 3,
        Cooling = 4,
        Frozen = 5,
        Breakfast = 6,
        CookingAndBaking = 7,
        Candy = 8,
        Beverages = 9,
        Baby = 10,
        Household = 11,
        PersonalCare = 12
    }

    [Table("ProductCategory")]
    public class ProductCategory : DboBase
    {
        [Required]
        public string Name { get; set; }

        public ProductCategoryEnum ProductCategoryEnum => (ProductCategoryEnum) Id;

        [Required]
        public int SortWeight { get; set; }
    }

    public class ProductCategoryDto : DtoBase
    {
        public string Name { get; set; }

        public int SortWeight { get; set; }
    }

    [Inject]
    public class ProductCategoryMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ProductCategory, ProductCategoryDto>(MapToDto);
        }

        private ProductCategoryDto MapToDto(ProductCategory category, MappingContext mappingContext)
        {
            return new ProductCategoryDto
            {
                Name = category.Name,
                Id = category.Id,
                SortWeight = category.SortWeight,
            };
        }
    }
}