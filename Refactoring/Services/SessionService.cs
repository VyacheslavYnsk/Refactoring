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

    public async Task<IEnumerable<Session>> CreateAsync(SessionCreate dto)
    {
        var film = await _context.Films.FindAsync(dto.FilmId);
        if (film == null)
            throw new Exception("����� �� ������");

        if (dto.StartAt < DateTime.UtcNow)
            throw new InvalidOperationException("������ ��������� ����� � �������");

        if (dto.PeriodicConfig == null)
        {
            var session = new Session
            {
                Id = Guid.NewGuid(),
                FilmId = dto.FilmId,
                HallId = dto.HallId,
                StartAt = dto.StartAt,
                Timeslot = new Timeslot
                {
                    Start = dto.StartAt.AddMinutes(-20),
                    End = dto.StartAt.AddMinutes(film.DurationMinutes + 20)
                }
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();
            return new List<Session> { session };
        }

        if (dto.PeriodicConfig.PeriodGenerationEndsAt < dto.StartAt)
            throw new InvalidOperationException("���� ��������� ��������� ������������� ������� �� ����� ���� ������ ��������� ����");

        var createdSessions = new List<Session>();
        var currentDate = dto.StartAt;

        while (currentDate <= dto.PeriodicConfig.PeriodGenerationEndsAt)
        {
            var session = new Session
            {
                Id = Guid.NewGuid(),
                FilmId = dto.FilmId,
                HallId = dto.HallId,
                StartAt = currentDate,
                Timeslot = new Timeslot
                {
                    Start = currentDate.AddMinutes(-20),
                    End = currentDate.AddMinutes(film.DurationMinutes + 20)
                }
            };

            createdSessions.Add(session);

            currentDate = dto.PeriodicConfig.Period switch
            {
                Period.EVERY_DAY => currentDate.AddDays(1),
                Period.EVERY_WEEK => currentDate.AddDays(7),
                _ => throw new ArgumentException("������������ �������� period")
            };
        }

        _context.Sessions.AddRange(createdSessions);
        await _context.SaveChangesAsync();

        return createdSessions;
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
            if (dto.StartAt.Value < DateTime.UtcNow)
                throw new InvalidOperationException("������ ���������� ����� � �������");

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
                Start = session.StartAt.AddMinutes(-20),
                End = session.StartAt.AddMinutes(film.DurationMinutes + 20)
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
