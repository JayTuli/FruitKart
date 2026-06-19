using ImageService.Models;
using ImageService.Models.DTO;
using ImageService.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ImageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        private readonly ApiResponse _response;

        public ImageController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
            _response = new ApiResponse();
        }

        // GET api/image
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _response.Result = await _imageRepository.GetAllAsync();
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        // GET api/image/5
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _imageRepository.GetByIdAsync(id);
            if (result is null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("Image not found.");
                return NotFound(_response);
            }
            _response.Result = result;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        // POST api/image
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] NewImagesDTO newImagesDTO)
        {
            try
            {
                if (newImagesDTO.File is null || newImagesDTO.File.Length == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("File is required.");
                    return BadRequest(_response);
                }

                var result = await _imageRepository.UploadImageAsync(newImagesDTO.File);

                if (result is null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.ErrorMessages.Add("Image upload to Cloudinary failed.");
                    return StatusCode(500, _response);
                }

                _response.Result = result;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add(ex.Message);
                return StatusCode(500, _response);
            }
        }

        // DELETE api/image?publicId=fruitkart/abc123
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string publicId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(publicId))
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("PublicId is required.");
                    return BadRequest(_response);
                }

                var success = await _imageRepository.DeleteImageAsync(publicId);

                if (!success)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.ErrorMessages.Add("Image delete from Cloudinary failed.");
                    return StatusCode(500, _response);
                }

                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add("An unexpected error occurred.");
                return StatusCode(500, _response);
            }
        }
    }
}