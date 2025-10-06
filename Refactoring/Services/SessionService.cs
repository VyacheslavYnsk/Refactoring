using Microsoft.EntityFrameworkCore;

public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _context;

    public SessionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Session> Sessions, int TotalCount)> GetAllAsync(int page, int size, Guid? filmId, DateTime? date)
    {
        var query = _context.Sessions.AsQueryable();

        if (filmId.HasValue)
            query = query.Where(s => s.FilmId == filmId.Value);

        if (date.HasValue)
            query = query.Where(s => s.StartAt.Date == date.Value.Date);

        var total = await query.CountAsync();

        var sessions = await query
            .OrderBy(s => s.StartAt)
            .Skip(page * size)
            .Take(size)
            .ToListAsync();

        return (sessions, total);
    }

    public async Task<Session?> GetByIdAsync(Guid id)
    {
        return await _context.Sessions.FindAsync(id);
    }

    public async Task<Session> CreateAsync(SessionCreate dto)
    {
        var film = await _context.Films.FindAsync(dto.FilmId);
        if (film == null)
            throw new Exception("����� �� ������");

        var session = new Session
        {
            Id = Guid.NewGuid(),
            FilmId = dto.FilmId,
            HallId = dto.HallId,
            StartAt = dto.StartAt,
            Timeslot = new Timeslot
            {
                Start = dto.StartAt,
                End = dto.StartAt.AddMinutes(film.DurationMinutes)
            }
        };

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<Session?> UpdateAsync(Guid id, SessionUpdate dto)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null) return null;

        bool recalcTimeslot = false;

        if (dto.FilmId.HasValue && dto.FilmId.Value != session.FilmId)
        {
            session.FilmId = dto.FilmId.Value;
            recalcTimeslot = true;
        }

        if (dto.HallId.HasValue && dto.HallId.Value != session.HallId)
        {
            session.HallId = dto.HallId.Value;
        }

        if (dto.StartAt.HasValue && dto.StartAt.Value != session.StartAt)
        {
            session.StartAt = dto.StartAt.Value;
            recalcTimeslot = true;
        }

        if (recalcTimeslot)
        {
            var film = await _context.Films.FindAsync(session.FilmId);
            if (film == null)
                throw new Exception("����� �� ������");

            session.Timeslot = new Timeslot
            {
                Start = session.StartAt,
                End = session.StartAt.AddMinutes(film.DurationMinutes)
            };
        }

        await _context.SaveChangesAsync();
        return session;
    }



    public async Task<bool> DeleteAsync(Guid id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null) return false;

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();
        return true;
    }
}
