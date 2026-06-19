using ImageService.Data;
using ImageService.Models;
using ImageService.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace ImageService.Repository
{
    public class ImageRepository : IImageRepository
    {
        private readonly FruitImageDbContext _db;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(
            FruitImageDbContext db,
            ICloudinaryService cloudinaryService,
            ILogger<ImageRepository> logger)
        {
            _db = db;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        // ── UPLOAD ──────────────────────────────────────────────────────────
        public async Task<UpdatedImagesDTO?> UploadImageAsync(IFormFile file)
        {
            try
            {
                var (imageUrl, publicId) = await _cloudinaryService.UploadAsync(file);

                var image = new Images
                {
                    ImageName = file.FileName,
                    ImageUrl = imageUrl,   // permanent — no expiry
                    PublicId = publicId,
                    UploadedAt = DateTime.UtcNow
                };

                _db.Images.Add(image);
                await _db.SaveChangesAsync();

                return new UpdatedImagesDTO
                {
                    ImageId = image.ImageId,
                    ImageName = image.ImageName,
                    ImageUrl = image.ImageUrl,
                    PublicId = image.PublicId,
                    UploadedAt = image.UploadedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload failed.");
                return null;
            }
        }

        // ── DELETE ──────────────────────────────────────────────────────────
        public async Task<bool> DeleteImageAsync(string publicId)
        {
            try
            {
                await _cloudinaryService.DeleteAsync(publicId);

                var image = await _db.Images.FirstOrDefaultAsync(i => i.PublicId == publicId);
                if (image is not null)
                {
                    _db.Images.Remove(image);
                    await _db.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete failed. PublicId: {PublicId}", publicId);
                return false;
            }
        }

        // ── GET ALL ─────────────────────────────────────────────────────────
        // Cloudinary URLs are permanent — just return what's stored in DB
        public async Task<List<UpdatedImagesDTO>> GetAllAsync()
        {
            var images = await _db.Images.AsNoTracking().ToListAsync();

            return images.Select(img => new UpdatedImagesDTO
            {
                ImageId = img.ImageId,
                ImageName = img.ImageName,
                ImageUrl = img.ImageUrl,  // permanent, no regeneration needed
                PublicId = img.PublicId,
                UploadedAt = img.UploadedAt
            }).ToList();
        }

        // ── GET BY ID ───────────────────────────────────────────────────────
        public async Task<UpdatedImagesDTO?> GetByIdAsync(int imageId)
        {
            var img = await _db.Images.AsNoTracking()
                                      .FirstOrDefaultAsync(i => i.ImageId == imageId);
            if (img is null) return null;

            return new UpdatedImagesDTO
            {
                ImageId = img.ImageId,
                ImageName = img.ImageName,
                ImageUrl = img.ImageUrl,
                PublicId = img.PublicId,
                UploadedAt = img.UploadedAt
            };
        }
    }
}