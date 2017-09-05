using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Common.DataAccess;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Infrastructure.Images
{
    [Table("Image")]
    public class Image : DboBase
    {
        [Required]
        public string ContentType { get; set; }

        [Required]
        public byte[] File { get; set; }

        [Required]
        public Guid Guid { get; set; }
    }

    public class ImageDto
    {
        public string ContentType { get; set; }

        public byte[] File { get; set; }

        public Guid Guid { get; set; }
    }

    [Inject]
    public class ImageMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<Image, ImageDto>(MapToDto);
        }

        private static ImageDto MapToDto(Image image, MappingContext mappingContext)
        {
            return new ImageDto
            {
                ContentType = image.ContentType,
                File = image.File,
                Guid = image.Guid
            };
        }
    }
}