using Microsoft.AspNetCore.Mvc;
using Model.Film;

namespace Refactoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilmsController : ControllerBase
    {
        private readonly IFilmService _filmService;

        public FilmsController(IFilmService filmService)
        {
            _filmService = filmService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFilms()
        {
            var films = await _filmService.GetAllAsync();
            return Ok(films);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetFilmById(Guid id)
        {
            var film = await _filmService.GetByIdAsync(id);
            if (film == null)
                return NotFound();

            return Ok(film);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFilm([FromBody] CreateFilm dto)
        {
            var film = await _filmService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetFilmById), new { id = film.Id }, film);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateFilm(Guid id, [FromBody] FilmUpdate dto)
        {
            var film = await _filmService.UpdateAsync(id, dto);
            if (film == null)
                return NotFound();

            return Ok(film);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFilm(Guid id)
        {
            var deleted = await _filmService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
