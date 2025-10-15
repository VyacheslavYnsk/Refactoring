public class FilmResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public AgeRating AgeRating { get; set; }
    public MediaResponse? Poster { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
