namespace MenuServiceAPI.Models.DTO
{
    // Inner result from ImageService
    public class ImageUploadResultDTO
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
    }

    // Outer ApiResponse wrapper from ImageService
    public class ImageServiceApiResponse
    {
        public bool IsSuccess { get; set; }
        public ImageUploadResultDTO? Result { get; set; }
    }
}   