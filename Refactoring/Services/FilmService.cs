using Microsoft.EntityFrameworkCore;
using Model.Film;


public class FilmService : IFilmService
{
    private readonly ApplicationDbContext _context;

    public FilmService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Film>> GetAllAsync()
    {
        return await _context.Films.ToListAsync();
    }

    public async Task<Film?> GetByIdAsync(Guid id)
    {
        return await _context.Films.FindAsync(id);
    }

    public async Task<Film> CreateAsync(CreateFilm dto)
    {
        var film = new Film
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            DurationMinutes = dto.DurationMinutes,
            AgeRating = dto.AgeRating,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Films.Add(film);
        await _context.SaveChangesAsync();

        return film;
    }

    public async Task<Film?> UpdateAsync(Guid id, FilmUpdate dto)
    {
        var film = await _context.Films.FindAsync(id);
        if (film == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Title))
            film.Title = dto.Title;

        if (!string.IsNullOrWhiteSpace(dto.Description))
            film.Description = dto.Description;

        if (dto.DurationMinutes.HasValue)
            film.DurationMinutes = dto.DurationMinutes.Value;

        if (dto.AgeRating.HasValue)
            film.AgeRating = dto.AgeRating.Value;

        film.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return film;
    }


    public async Task<bool> DeleteAsync(Guid id)
    {
        var film = await _context.Films.FindAsync(id);
        if (film == null) return false;

        _context.Films.Remove(film);
        await _context.SaveChangesAsync();
        return true;
    }
}