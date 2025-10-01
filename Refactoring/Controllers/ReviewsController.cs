using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("films/{filmId:guid}/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews(Guid filmId, int page = 0, int size = 20)
    {
        var (reviews, total) = await _reviewService.GetByFilmAsync(filmId, page, size);

        if (!reviews.Any())
            return NotFound("Фильм не найден или нет отзывов");

        return Ok(new
        {
            data = reviews,
            pagination = new
            {
                page,
                limit = size,
                total,
                pages = (int)Math.Ceiling(total / (double)size)
            }
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview(Guid filmId, [FromBody] ReviewCreate dto)
    {
        var clientId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var review = await _reviewService.CreateAsync(filmId, clientId, dto);

        return CreatedAtAction(nameof(GetReviewById), "Reviews", new { id = review.Id }, review);
    }


    [HttpGet("~/reviews/{id:guid}")]
    public async Task<IActionResult> GetReviewById(Guid id)
    {
        var review = await _reviewService.GetByIdAsync(id);
        if (review == null) return NotFound();
        return Ok(review);
    }

    [HttpPut("~/reviews/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] ReviewUpdate dto)
    {
        var clientId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var review = await _reviewService.UpdateAsync(id, clientId, dto);
        if (review == null) return Forbid();
        return Ok(review);
    }


    [HttpDelete("~/reviews/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var clientId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var deleted = await _reviewService.DeleteAsync(id, clientId);
        if (!deleted) return Forbid();
        return NoContent();
    }
}
