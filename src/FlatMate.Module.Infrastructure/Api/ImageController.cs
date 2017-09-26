using System;
using System.Threading.Tasks;
using FlatMate.Module.Common;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Infrastructure.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Infrastructure.Api
{
    [Route(RouteTemplate)]
    public class ImageController : ApiController
    {
        private const string RouteTemplate = "/image/";

        private readonly IImageService _imageService;

        public ImageController(IImageService imageService, IMapper mapper) : base(mapper)
        {
            _imageService = imageService;
        }

        [HttpGet("{guid}")]
        public async Task<IActionResult> Get(Guid guid)
        {
            var (result, imageDto) = await _imageService.Get(guid);

            if (result.IsError)
            {
                return result.ErrorType == ErrorType.NotFound ? new NotFoundResult() : new StatusCodeResult(500);
            }

            return new FileContentResult(imageDto.File, imageDto.ContentType);
        }

        public static string GetImageUrl(Guid guid)
        {
            return RouteTemplate + guid;
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Result> Save(byte[] file, string contentType)
        {
            var (result, dto) = await _imageService.Save(file, contentType);

            if (result.IsError)
            {
                return result;
            }

            return new SuccessResult<ImageJso>(Map<ImageJso>(dto));
        }

        /// <summary>
        ///     Accepts files via multi-part HTML form.
        ///     https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
        /// </summary>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Result> SaveFromForm(IFormFile file)
        {
            var (result, dto) = await _imageService.Save(ByteHelper.ReadToEnd(file.OpenReadStream()), file.ContentType);

            if (result.IsError)
            {
                return result;
            }

            return new SuccessResult<ImageJso>(Map<ImageJso>(dto));
        }
    }
}