using Microsoft.EntityFrameworkCore;

public class MediaService : IMediaService
{
    private readonly ApplicationDbContext _context;

    public MediaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MediaResponse> UploadAsync(IFormFile file, MediaType mediaType)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Файл не может быть пустым");

        if (file.Length > 10 * 1024 * 1024)
            throw new InvalidOperationException("Файл слишком большой");

        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var data = ms.ToArray();

        var media = new Media
        {
            Id = Guid.NewGuid(),
            Filename = file.FileName,
            ContentType = file.ContentType,
            MediaType = mediaType,
            Data = data
        };

        _context.Media.Add(media);
        await _context.SaveChangesAsync();

        return new MediaResponse
        {
            Id = media.Id,
            Filename = media.Filename,
            ContentType = media.ContentType,
            MediaType = media.MediaType,
            CreatedAt = media.CreatedAt,
            UpdatedAt = media.UpdatedAt
        };
    }

    public async Task<Media?> GetByIdAsync(Guid id)
    {
        return await _context.Media.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var media = await _context.Media.FindAsync(id);
        if (media == null)
            return false;

        _context.Media.Remove(media);
        await _context.SaveChangesAsync();
        return true;
    }
}
