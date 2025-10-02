using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("sessions")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly IUserService _userService;

    public SessionsController(ISessionService sessionService, IUserService userService)
    {
        _sessionService = sessionService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSessions([FromQuery] int page = 0, [FromQuery] int size = 20, [FromQuery] Guid? filmId = null, [FromQuery] DateTime? date = null)
    {
        try
        {
            var (sessions, total) = await _sessionService.GetAllAsync(page, size, filmId, date);

            return Ok(new
            {
                data = sessions,
                pagination = new
                {
                    page,
                    limit = size,
                    total,
                    pages = (int)Math.Ceiling(total / (double)size)
                }
            });
        }
        catch
        {
            return StatusCode(500, new { success = false, message = "Ошибка при получении списка сеансов" });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSessionById(Guid id)
    {
        try
        {
            var session = await _sessionService.GetByIdAsync(id);
            if (session == null)
            {
                return NotFound(new { success = false, message = $"Сеанс с ID {id} не найден" });
            }
            return Ok(session);
        }
        catch
        {
            return StatusCode(500, new { success = false, message = "Ошибка при получении сеанса" });
        }
    }

[HttpPost]
[Authorize]
public async Task<IActionResult> CreateSession([FromBody] SessionCreate dto)
{
    try
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized(new { success = false, message = "Неверный токен" });

        var userRole = await _userService.GetRoleAsync(Guid.Parse(userId));
        if (userRole != Role.Admin) return BadRequest(new { success = false, message = "Недостаточно прав" });

        var session = await _sessionService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetSessionById), new { id = session.Id }, session);
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { success = false, message = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new { success = false, message = ex.Message });
    }
    catch
    {
        return StatusCode(500, new { success = false, message = "Ошибка при создании сеанса" });
    }
}

[HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateSession(Guid id, [FromBody] SessionUpdate dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { success = false, message = "Неверный токен" });

            var userRole = await _userService.GetRoleAsync(Guid.Parse(userId));
            if (userRole != Role.Admin) return BadRequest(new { success = false, message = "Недостаточно прав" });

            var session = await _sessionService.UpdateAsync(id, dto);
            if (session == null) return NotFound(new { success = false, message = $"Сеанс с ID {id} не найден" });

            return Ok(new { success = true, data = session });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch
        {
            return StatusCode(500, new { success = false, message = "Ошибка при обновлении сеанса" });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteSession(Guid id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { success = false, message = "Неверный токен" });

            var userRole = await _userService.GetRoleAsync(Guid.Parse(userId));
            if (userRole != Role.Admin) return BadRequest(new { success = false, message = "Недостаточно прав" });

            var deleted = await _sessionService.DeleteAsync(id);
            if (!deleted) return NotFound(new { success = false, message = $"Сеанс с ID {id} не найден" });

            return Ok(new { success = true, message = "Сеанс успешно удалён" });
        }
        catch
        {
            return StatusCode(500, new { success = false, message = "Ошибка при удалении сеанса" });
        }
    }
}
