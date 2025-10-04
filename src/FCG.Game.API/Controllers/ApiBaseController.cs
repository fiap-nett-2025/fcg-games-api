using FCG.Game.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCG.Game.API.Controllers
{
    [ApiController]
    public abstract class ApiBaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string? message = null, int statusCode = 200)
        {
            var response = ResponseWrapper<T>.SuccessResponse(data, message, statusCode);
            return StatusCode(statusCode, response);
        }

        protected IActionResult CreatedResponse<T>(T data, string? message = null)
        {
            var response = ResponseWrapper<T>.SuccessResponse(data, message, 201);
            return StatusCode(201, response);
        }

        protected IActionResult UnauthorizedResponse(string message)
        {
            var response = ResponseWrapper<object>.FailResponse(message, 401);
            return Unauthorized(response);
        }

        protected IActionResult Fail(string message, int statusCode = 400)
        {
            var response = ResponseWrapper<string>.FailResponse(message, statusCode);
            return StatusCode(statusCode, response);
        }

        protected IActionResult ValidationFail(List<string> errors)
        {
            var response = ResponseWrapper<List<string>>.ValidationFailResponse(errors);
            return StatusCode(400, response);
        }

        protected Guid GetUserId()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Usuário não autenticado");
            return Guid.Parse(userId);
        }

        protected string GetJwtToken()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                throw new UnauthorizedAccessException("Token JWT não encontrado");

            return authHeader.Replace("Bearer ", "");
        }
    }
}