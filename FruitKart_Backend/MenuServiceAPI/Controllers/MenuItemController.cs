using AutoMapper;
using MenuServiceAPI.Models;
using MenuServiceAPI.Models.DTO;
using MenuServiceAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace MenuServiceAPI.Controllers
{
    [Route("api/MenuItem")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<MenuItemController> _logger;

        public MenuItemController(
            IMenuRepository menuRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<MenuItemController> logger)
        {
            _menuRepository = menuRepository ?? throw new ArgumentNullException(nameof(menuRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new ApiResponse();
            response.Result = await _menuRepository.GetAllAsync();
            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetMenuItem")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ApiResponse();
            if (id <= 0)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Id must be greater than 0.");
                return BadRequest(response);
            }

            var item = await _menuRepository.GetByIdAsync(id);
            if (item is null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add($"MenuItem with id {id} not found.");
                return NotFound(response);
            }

            response.Result = item;
            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] MenuItemCreateDTO createDTO)
        {
            var response = new ApiResponse();

            if (!ModelState.IsValid)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(response);
            }

            if (createDTO.File is null || createDTO.File.Length == 0)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Image file is required.");
                return BadRequest(response);
            }

            var imageUrl = await UploadToImageServiceAsync(createDTO.File);
            if (imageUrl is null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadGateway;
                response.ErrorMessages.Add("Image upload failed. ImageService may be unavailable.");
                return StatusCode(502, response);
            }

            var created = await _menuRepository.CreateAsync(createDTO, imageUrl);
            response.Result = created;
            response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetMenuItem", new { id = created.Id }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] MenuItemUpdateDTO updateDTO)
        {
            var response = new ApiResponse();

            if (!ModelState.IsValid || updateDTO.Id != id)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Id mismatch or invalid model state.");
                return BadRequest(response);
            }

            string? imageUrl = null;

            if (updateDTO.File is { Length: > 0 })
            {
                imageUrl = await UploadToImageServiceAsync(updateDTO.File);
                if (imageUrl is null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadGateway;
                    response.ErrorMessages.Add("Image upload failed.");
                    return StatusCode(502, response);
                }
            }

            var updated = await _menuRepository.UpdateAsync(id, updateDTO, imageUrl);
            if (updated is null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add($"MenuItem with id {id} not found.");
                return NotFound(response);
            }

            response.Result = updated;
            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }
        [HttpPatch("{id:int}/stock")]
        [AllowAnonymous]
        public async Task<IActionResult> DeductStock(int id, [FromBody] int quantity)
        {
            var response = new ApiResponse();

            if (id <= 0 || quantity <= 0)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Invalid id or quantity.");
                return BadRequest(response);
            }

            var (result,stockCount) = await _menuRepository.DeductStockAsync(id, quantity);

            if (result == DeductStockResult.NotFound)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add($"MenuItem {id} not found.");
                return NotFound(response);
            }

            if (result == DeductStockResult.InsufficientStock)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.Conflict;  // 409
                response.ErrorMessages.Add($"Insufficient stock for item {id}.only {stockCount} left ");
                return Conflict(response);
            }

            response.StatusCode = HttpStatusCode.OK;
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new ApiResponse();

            if (id <= 0)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Id must be greater than 0.");
                return BadRequest(response);
            }

            var existing = await _menuRepository.GetByIdAsync(id);
            if (existing is null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add($"MenuItem with id {id} not found.");
                return NotFound(response);
            }

            await _menuRepository.DeleteAsync(id);
            response.StatusCode = HttpStatusCode.NoContent;
            return Ok(response);
        }

        // ── ImageService HTTP helper ─────────────────────────────────────────
        private async Task<string?> UploadToImageServiceAsync(IFormFile file)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ImageService");

                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                using var fileContent = new StreamContent(fileStream);

                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

                content.Add(fileContent, "File", file.FileName);  

                var response = await client.PostAsync("api/image", content); 

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("ImageService returned {Status} on upload.", response.StatusCode);
                    return null;
                }

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<ImageServiceApiResponse>();

                return apiResponse?.Result?.ImageUrl;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "ImageService unreachable during upload.");
                return null;
            }
        }
    }
}