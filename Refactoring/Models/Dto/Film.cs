using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Film : Entity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    public string Description { get; set; }

    public string? Image { get; set; }


    [Required]
    public int DurationMinutes { get; set; }

    [Required]
    public AgeRating AgeRating { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    public Guid? PosterId { get; set; }

    [ForeignKey(nameof(PosterId))]
    public Media? Poster { get; set; }
}