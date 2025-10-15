namespace Model.Film;

public class CreateFilm
{
    public string Title { get; set; }

    public string Description { get; set; }

    public int DurationMinutes { get; set; }

    public AgeRating AgeRating { get; set; }

    public Guid? PosterId { get; set; }
}