using Microsoft.EntityFrameworkCore;
using Model.Film;

public class FilmService : IFilmService
{
    private readonly ApplicationDbContext _context;

    public FilmService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginationResponse<FilmResponse>> GetListAsync(int page, int size)
    {
        if (page < 0) page = 0;
        if (size < 1) size = 1;
        if (size > 100) size = 100;

        var query = _context.Films
            .Include(f => f.Poster)
            .OrderBy(f => f.Title)
            .Select(f => new FilmResponse
            {
                Id = f.Id,
                Title = f.Title,
                Description = f.Description,
                DurationMinutes = f.DurationMinutes,
                AgeRating = f.AgeRating,
                Poster = f.Poster == null ? null : new MediaResponse
                {
                    Id = f.Poster.Id,
                    Filename = f.Poster.Filename,
                    ContentType = f.Poster.ContentType,
                    MediaType = f.Poster.MediaType,
                    CreatedAt = f.Poster.CreatedAt,
                    UpdatedAt = f.Poster.UpdatedAt
                },
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            });

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)size);

        var data = await query
            .Skip(page * size)
            .Take(size)
            .ToListAsync();

        return new PaginationResponse<FilmResponse>
        {
            Data = data,
            Pagination = new PaginationInfo
            {
                Page = page,
                Limit = size,
                Total = total,
                Pages = totalPages
            }
        };
    }

    public async Task<FilmResponse?> GetByIdAsync(Guid id)
    {
        return await _context.Films
            .Include(f => f.Poster)
            .Where(f => f.Id == id)
            .Select(f => new FilmResponse
            {
                Id = f.Id,
                Title = f.Title,
                Description = f.Description,
                DurationMinutes = f.DurationMinutes,
                AgeRating = f.AgeRating,
                Poster = f.Poster == null ? null : new MediaResponse
                {
                    Id = f.Poster.Id,
                    Filename = f.Poster.Filename,
                    ContentType = f.Poster.ContentType,
                    MediaType = f.Poster.MediaType,
                    CreatedAt = f.Poster.CreatedAt,
                    UpdatedAt = f.Poster.UpdatedAt
                },
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<FilmResponse> CreateAsync(CreateFilm dto)
    {
        var film = new Film
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            DurationMinutes = dto.DurationMinutes,
            AgeRating = dto.AgeRating,
            PosterId = dto.PosterId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Films.Add(film);
        await _context.SaveChangesAsync();

        var poster = await _context.Media.FindAsync(film.PosterId);

        return new FilmResponse
        {
            Id = film.Id,
            Title = film.Title,
            Description = film.Description,
            DurationMinutes = film.DurationMinutes,
            AgeRating = film.AgeRating,
            Poster = poster == null ? null : new MediaResponse
            {
                Id = poster.Id,
                Filename = poster.Filename,
                ContentType = poster.ContentType,
                MediaType = poster.MediaType,
                CreatedAt = poster.CreatedAt,
                UpdatedAt = poster.UpdatedAt
            },
            CreatedAt = film.CreatedAt,
            UpdatedAt = film.UpdatedAt
        };
    }

    public async Task<FilmResponse?> UpdateAsync(Guid id, FilmUpdate dto)
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
        if (dto.PosterId.HasValue)
            film.PosterId = dto.PosterId;

        film.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var poster = await _context.Media.FindAsync(film.PosterId);

        return new FilmResponse
        {
            Id = film.Id,
            Title = film.Title,
            Description = film.Description,
            DurationMinutes = film.DurationMinutes,
            AgeRating = film.AgeRating,
            Poster = poster == null ? null : new MediaResponse
            {
                Id = poster.Id,
                Filename = poster.Filename,
                ContentType = poster.ContentType,
                MediaType = poster.MediaType,
                CreatedAt = poster.CreatedAt,
                UpdatedAt = poster.UpdatedAt
            },
            CreatedAt = film.CreatedAt,
            UpdatedAt = film.UpdatedAt
        };
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
