using ImageService.Models.DTO;

namespace ImageService.Repository
{
    public interface IImageRepository
    {
        Task<UpdatedImagesDTO?> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string publicId);
        Task<List<UpdatedImagesDTO>> GetAllAsync();
        Task<UpdatedImagesDTO?> GetByIdAsync(int imageId);
    }
}