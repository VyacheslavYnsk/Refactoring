public class MediaResponse
{
    public Guid Id { get; set; }
    public string Filename { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
