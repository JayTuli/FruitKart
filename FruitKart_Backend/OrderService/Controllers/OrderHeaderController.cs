using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Model.DTO;
using OrderService.Repository;
using OrderService.Services;
using System.Net;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderHeaderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;

        public OrderHeaderController(
            IOrderRepository orderRepository,
            IEmailService emailService)
        {
            _orderRepository = orderRepository
                ?? throw new ArgumentNullException(nameof(orderRepository));
            _emailService = emailService
                ?? throw new ArgumentNullException(nameof(emailService));
        }

        // ─── GET /api/orderheader ─────────────────────────────────────────────
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetOrders(string userId = "")
        {
            var response = new ApiResponse();
            try
            {
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin)
                {
                    //Always pull userId from JWT for non-admins
                    userId = User.FindFirst("UserId")?.Value ?? "";

                    if (string.IsNullOrEmpty(userId))
                    {
                        response.IsSuccess = false;
                        response.StatusCode = HttpStatusCode.Unauthorized;
                        response.ErrorMessages.Add("User identity not found in token.");
                        return Unauthorized(response);
                    }
                }
                else
                {
                    userId = ""; // Admin sees all orders
                }

                response.Result = await _orderRepository.GetAllOrdersAsync(userId);
                response.StatusCode = HttpStatusCode.OK;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                response.ErrorMessages.Add(ex.InnerException?.Message ?? "");
                return StatusCode(500, response);
            }
        }

        // ─── GET /api/orderheader/{orderId} ──────────────────────────────────
        [HttpGet("{orderId:int}", Name = "GetOrder")]
        public async Task<ActionResult<ApiResponse>> GetOrder(int orderId)
        {
            var response = new ApiResponse();
            try
            {
                if (orderId <= 0)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Invalid order Id");
                    return BadRequest(response);
                }

                var order = await _orderRepository.GetOrderByIdAsync(orderId);
                if (order is null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Order not found");
                    return NotFound(response);
                }

                response.Result = order;
                response.StatusCode = HttpStatusCode.OK;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(500, response);
            }
        }

        // ─── POST /api/orderheader ────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateOrder(
            [FromBody] OrderHeaderCreateDTO orderHeaderDTO)
        {
            var response = new ApiResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(response);
                }

                var userId = User.FindFirst("UserId")?.Value ?? "";

                if (string.IsNullOrEmpty(userId))
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.ErrorMessages.Add("User identity not found in token.");
                    return Unauthorized(response);
                }
              // declare authHeader
                var authHeader = Request.Headers["Authorization"].ToString();
                var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? authHeader.Substring(7).Trim()
                    : authHeader.Trim();

                var created = await _orderRepository.CreateOrderAsync(orderHeaderDTO, userId, token);

            
                _ = Task.Run(() => _emailService.SendOrderConfirmationAsync(
                    toEmail: orderHeaderDTO.PickUpEmail,
                    toName: orderHeaderDTO.PickUpName,
                    orderHeaderId: created.OrderHeaderId,
                    orderTotal: created.OrderTotal,
                    totalItems: created.TotalItem,
                    orderDate: created.OrderDate
                ));

                response.Result = created;
                response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute(
                    "GetOrder",
                    new { orderId = created.OrderHeaderId },
                    response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                response.ErrorMessages.Add(ex.InnerException?.Message ?? "");
                return StatusCode(500, response);
            }
        }

        // ─── PUT /api/orderheader/{orderId} ──────────────────────────────────
        [HttpPut("{orderId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> UpdateOrder(
            int orderId,
            [FromBody] OrderHeaderUpdateDTO orderHeaderDTO)
        {
            var response = new ApiResponse();
            try
            {
                if (!ModelState.IsValid || orderId != orderHeaderDTO.OrderHeaderId)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Invalid Id or model state.");
                    return BadRequest(response);
                }

                var updated = await _orderRepository.UpdateOrderAsync(orderId, orderHeaderDTO);
                if (updated is null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Order not found");
                    return NotFound(response);
                }

                response.StatusCode = HttpStatusCode.NoContent;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(500, response);
            }
        }
    }
}
