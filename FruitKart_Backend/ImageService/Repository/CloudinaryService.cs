using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace ImageService.Repository
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(IConfiguration config, ILogger<CloudinaryService> logger)
        {
            _logger = logger;

            var account = new Account(
                config["Cloudinary:CloudName"]
                    ?? throw new InvalidOperationException("Cloudinary:CloudName not configured."),
                config["Cloudinary:ApiKey"]
                    ?? throw new InvalidOperationException("Cloudinary:ApiKey not configured."),
                config["Cloudinary:ApiSecret"]
                    ?? throw new InvalidOperationException("Cloudinary:ApiSecret not configured.")
            );

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true; // always HTTPS
        }

        // publicId is the permanent key stored in DB — like BlobName was before
        public async Task<(string imageUrl, string publicId)> UploadAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "fruitkart",                      // organises in Cloudinary dashboard
                UseFilename = false,                    
                UniqueFilename = true,
                Overwrite = false,
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto")               
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error is not null)
                throw new Exception($"Cloudinary upload error: {result.Error.Message}");

            // SecureUrl is a permanent HTTPS URL — no expiry, no SAS needed
            return (result.SecureUrl.ToString(), result.PublicId);
        }

        public async Task DeleteAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error is not null)
                _logger.LogWarning("Cloudinary delete warning for {PublicId}: {Error}",
                    publicId, result.Error.Message);
        }
    }
}