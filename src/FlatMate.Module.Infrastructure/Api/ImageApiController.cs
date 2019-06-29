using System;
using System.Threading.Tasks;
using FlatMate.Module.Common;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Infrastructure.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Module.Infrastructure.Api
{
    [Route(RouteTemplate)]
    public class ImageApiController : ApiController
    {
        private const string RouteTemplate = "/image/";

        private readonly IImageService _imageService;

        public ImageApiController(IImageService imageService, IApiControllerServices services) : base(services)
        {
            _imageService = imageService;
        }

        [HttpGet("{guid}")]
        [ResponseCache(Duration = 60 * 60 * 24)]
        public async Task<IActionResult> Get(Guid guid)
        {
            var (result, imageDto) = await _imageService.Get(guid);

            if (result.IsError)
            {
                return result.ErrorType == ErrorType.NotFound ? new NotFoundResult() : new StatusCodeResult(500);
            }

            return new FileContentResult(imageDto.File, imageDto.ContentType);
        }

        public static string GetImagePath(Guid guid)
        {
            return RouteTemplate + guid;
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<(Result, ImageJso)> Save(byte[] file, string contentType)
        {
            return MapResultTuple(await _imageService.Save(file, contentType), Map<ImageJso>);
        }

        /// <summary>
        ///     Accepts files via multi-part HTML form.
        ///     https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
        /// </summary>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<(Result, ImageJso)> SaveFromForm(IFormFile file)
        {
            return MapResultTuple(await _imageService.Save(ByteHelper.ReadToEnd(file.OpenReadStream()), file.ContentType), Map<ImageJso>);
        }
    }
}