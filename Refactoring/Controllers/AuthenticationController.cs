using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Model.Register;

namespace Refactoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Неверные данные",
                        errors = ModelState.Values.SelectMany(v => v.Errors)
                    });
                }

                var result = await _authService.RegisterAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Неверные данные",
                        errors = ModelState.Values.SelectMany(v => v.Errors)
                    });
                }

                var result = await _authService.LoginAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }





    }
}