using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class HallService : IHallService
{
    private readonly ApplicationDbContext _context;

    public HallService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Hall> CreateAsync(HallCreate hallCreate)
    {
        var existingHall = await _context.Halls
            .FirstOrDefaultAsync(h => h.Number == hallCreate.Number);

        if (existingHall != null)
        {
            throw new InvalidOperationException($"Зал с номером {hallCreate.Number} уже существует");
        }

        var hall = new Hall
        {
            Id = Guid.NewGuid(),
            Name = hallCreate.Name,
            Number = hallCreate.Number,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Halls.Add(hall);
        await _context.SaveChangesAsync();

        return hall;
    }

     public async Task<PaginationResponse<Hall>> GetListAsync(int page, int size)
    {
        if (page < 0) page = 0;
        if (size < 1) size = 1;
        if (size > 100) size = 100; 

        var query = _context.Halls.OrderBy(h => h.Number);

        var total = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(total / (double)size);
        
        var skip = page * size;

        var data = await query
            .Skip(skip)
            .Take(size)
            .ToListAsync();

        return new PaginationResponse<Hall>
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

    public async Task<Hall> GetByIdAsync(Guid id)
    {
        var hall = await _context.Halls.FindAsync(id);

        if (hall == null)
        {
            throw new KeyNotFoundException($"Зал с ID {id} не найден");
        }

        return hall;
    }

    public async Task<Hall> EditAsync(HallUpdate hallUpdate, Guid id)
    {

        var hall = await _context.Halls.FirstOrDefaultAsync(h => h.Id == id);

        if (hall == null)
        {
            throw new KeyNotFoundException($"Зал с ID {id} не найден");
        }

        if (hallUpdate.Number.HasValue)
        {
            if (hallUpdate.Number.Value != hall.Number)
            {
                var existingHall = await _context.Halls
                    .FirstOrDefaultAsync(h => h.Number == hallUpdate.Number.Value && h.Id != id);

                if (existingHall != null)
                {
                    throw new InvalidOperationException($"Зал с номером {hallUpdate.Number.Value} уже существует");
                }
            }

            hall.Number = hallUpdate.Number.Value;
        }

        if (!string.IsNullOrEmpty(hallUpdate.Name))
        {
            hall.Name = hallUpdate.Name;
        }



        hall.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return hall;
    }
    
     public async Task<bool> DeleteAsync(Guid id)
    {
        var hall = await _context.Halls.FindAsync(id);
        
        if (hall == null)
        {
            throw new KeyNotFoundException($"Зал с ID {id} не найден");
        }

        _context.Halls.Remove(hall);
        var result = await _context.SaveChangesAsync();
        
        return result > 0;
    }
}