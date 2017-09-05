using System;
using FlatMate.Module.Infrastructure.Images;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Infrastructure.Api
{
    public class ImageJso
    {
        public Guid Guid { get; set; }
    }

    [Inject]
    public class ImageMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<ImageDto, ImageJso>(MapToJso);
        }

        private static ImageJso MapToJso(ImageDto imageDto, MappingContext arg3)
        {
            return new ImageJso { Guid = imageDto.Guid };
        }
    }
}