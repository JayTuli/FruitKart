namespace ImageService.Models.DTO
{
    public class UpdatedImagesDTO
    {
        public int ImageId { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;   // permanent Cloudinary HTTPS URL
        public string PublicId { get; set; } = string.Empty;   // used for delete
        public DateTime UploadedAt { get; set; }
    }
}
