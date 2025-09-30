using Model.Film;

public interface IFilmService
{
    Task<IEnumerable<Film>> GetAllAsync();
    Task<Film?> GetByIdAsync(Guid id);
    Task<Film> CreateAsync(CreateFilm dto);
    Task<Film?> UpdateAsync(Guid id, FilmUpdate dto);
    Task<bool> DeleteAsync(Guid id);
}
