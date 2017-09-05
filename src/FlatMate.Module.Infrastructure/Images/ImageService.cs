using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Infrastructure.Images
{
    public interface IImageService
    {
        Task<(Result, ImageDto)> Get(Guid hash);

        Task<(Result, ImageDto)> Get(int id);

        Task<(Result, ImageDto)> Save(byte[] file, string contentType);
    }

    [Inject]
    public class ImageService : IImageService
    {
        /// <summary>
        ///     5 Megabyte
        /// </summary>
        private const long MaxImageSize = 5 * 1024 * 1024;

        private static readonly string[] SupportedContentTypes = { "image/jpg", "image/jpeg", "image/png" };

        private readonly InfrastructureDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ImageService> _logger;

        public ImageService(InfrastructureDbContext dbContext, IMapper mapper, ILogger<ImageService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(Result, ImageDto)> Get(int id)
        {
            var image = await _dbContext.Images.FindAsync(id);

            if (image == null)
            {
                return (new ErrorResult(ErrorType.NotFound, "Image not found"), null);
            }

            return (SuccessResult.Default, _mapper.Map<ImageDto>(image));
        }

        public async Task<(Result, ImageDto)> Get(Guid hash)
        {
            var image = await _dbContext.Images.FirstOrDefaultAsync(x => x.Guid == hash);

            if (image == null)
            {
                return (new ErrorResult(ErrorType.NotFound, "Image not found"), null);
            }

            return (SuccessResult.Default, _mapper.Map<ImageDto>(image));
        }

        public async Task<(Result, ImageDto)> Save(byte[] file, string contentType)
        {
            if (file.LongLength > MaxImageSize)
            {
                return (new ErrorResult(ErrorType.ValidationError, "File to large"), null);
            }

            if (!SupportedContentTypes.Contains(contentType, StringComparer.CurrentCultureIgnoreCase))
            {
                return (new ErrorResult(ErrorType.ValidationError, "Unsupported media type"), null);
            }

            var image = new Image { File = file, Guid = Guid.NewGuid(), ContentType = contentType.ToLower() };
            await _dbContext.AddAsync(image);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "Error while saving changes");
                return (new ErrorResult(ErrorType.InternalError, "Datenbankfehler"), null);
            }

            return (SuccessResult.Default, _mapper.Map<ImageDto>(image));
        }
    }
}