using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("media")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(MediaResponse), 201)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] MediaUploadRequest request)
    {
        try
        {
            var result = await _mediaService.UploadAsync(request.File, request.MediaType);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(413, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var media = await _mediaService.GetByIdAsync(id);
        if (media == null)
            return NotFound();

        return File(media.Data, media.ContentType);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _mediaService.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
