using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Model.DTO;
using OrderService.Repository;
using System.Net;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderDetailsController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpPut("{orderDetailsId:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateRating(
            int orderDetailsId,
            [FromBody] OrderDetailUpdateDTO orderDetailsDTO)
        {
            var response = new ApiResponse();
            try
            {
                if (!ModelState.IsValid || orderDetailsId != orderDetailsDTO.OrderDetailId)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Invalid Id or model state.");
                    return BadRequest(response);
                }

                var updated = await _orderRepository.UpdateOrderDetailRatingAsync(
                    orderDetailsId, orderDetailsDTO);

                if (updated is null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Order detail not found");
                    return NotFound(response);
                }

                response.StatusCode = HttpStatusCode.NoContent;
                return Ok(response);
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("An unexpected error occurred.");
                return StatusCode(500, response);
            }
        }
    }
}