namespace ImageService.Repository
{
    public interface ICloudinaryService
    {
        Task<(string imageUrl, string publicId)> UploadAsync(IFormFile file);
        Task DeleteAsync(string publicId);
    }
}