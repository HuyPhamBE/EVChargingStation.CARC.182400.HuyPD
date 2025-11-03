using EVChargingStation.CARC.Application.HuyPD.Services;
using EVChargingStation.CARC.Application.HuyPD.Utils;
using EVChargingStation.CARC.Domain.HuyPD.DTOs.AuthDTO;
using EVChargingStation.CARC.Infrastructure.Commons;
using EVChargingStation.CARC.Infrastructure.HuyPD.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVChargingStation.CARC.WebAPI.HuyPD.Controller
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/auth/")]
    public class UserController : ControllerBase
    {
        public readonly IUserService userService;
        private readonly IClaimsService claimsService;

        public UserController(IUserService userService,IClaimsService claimsService)
        {
            this.userService = userService;
            this.claimsService = claimsService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO, [FromServices] IConfiguration configuration)
        {
            try
            {
                var result = await userService.LoginAsync(loginDTO, configuration);
                return Ok(ApiResult<LoginResponseDTO>.Success(result!, "200", "Login successful."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<LoginResponseDTO>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = claimsService.GetCurrentUserId;
                var result = await userService.Logout(userId);
                return Ok(ApiResult<object>.Success(result!, "200", "Loged out successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<object>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            await userService.RegisterAsync(registerDTO);
            return Ok(new { message = "Register successful" });
        }
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await userService.GetAllUserAsync();
            return Ok(result);
        }
        [HttpGet("getUserById/{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var result = await userService.GetUserById(userId);
            return Ok(result);
        }
        [HttpPost("banUser/{userId}")]
        public async Task<IActionResult> banUser(Guid userId)
        {
            await userService.DeleteUser(userId);
            return Ok(new { message = "User is banned" });
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken, [FromServices] IConfiguration configuration)
        {
            var result = await userService.RefreshTokenAsync(refreshToken, configuration);
            return Ok(result);
        }
    }
}
