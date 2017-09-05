using System;
using System.ComponentModel;
using FlatMate.Module.Infrastructure.Api;
using FlatMate.Module.Offers.Domain.Companies;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Api.Jso
{
    public class CompanyJso
    {
        [ReadOnly(true)]
        public int? Id { get; set; }

        [ReadOnly(true)]
        public Guid? ImageGuid { get; set; }

        [ReadOnly(true)]
        public string ImageLink { get; set; }

        public string Name { get; set; }
    }

    [Inject]
    public class CompanyMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<CompanyDto, CompanyJso>(MapToJso);
            mapper.Configure<CompanyJso, CompanyDto>(MapToDto);
        }

        private CompanyDto MapToDto(CompanyJso jso, MappingContext ctx)
        {
            return new CompanyDto
            {
                Id = jso.Id ?? 0,
                ImageGuid = jso.ImageGuid,
                Name = jso.Name
            };
        }

        private CompanyJso MapToJso(CompanyDto dto, MappingContext ctx)
        {
            var jso = new CompanyJso
            {
                Id = dto.Id,
                ImageGuid = dto.ImageGuid,
                Name = dto.Name
            };

            if (dto.ImageGuid.HasValue)
            {
                jso.ImageLink = ImageController.GetImageUrl(dto.ImageGuid.Value);
            }

            return jso;
        }
    }
}