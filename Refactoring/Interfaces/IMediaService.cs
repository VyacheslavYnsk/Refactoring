public interface IMediaService
{
    Task<MediaResponse> UploadAsync(IFormFile file, MediaType mediaType);
    Task<Media?> GetByIdAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
}
