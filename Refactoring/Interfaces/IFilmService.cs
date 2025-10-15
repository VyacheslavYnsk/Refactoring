using Model.Film;

public interface IFilmService
{
    Task<PaginationResponse<FilmResponse>> GetListAsync(int page, int size);
    Task<FilmResponse?> GetByIdAsync(Guid id);
    Task<FilmResponse> CreateAsync(CreateFilm dto);
    Task<FilmResponse?> UpdateAsync(Guid id, FilmUpdate dto);
    Task<bool> DeleteAsync(Guid id);
}