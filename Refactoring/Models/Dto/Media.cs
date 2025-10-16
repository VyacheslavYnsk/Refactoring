using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class Media : Entity
{
    [Required]
    public string Filename { get; set; } = string.Empty;

    [Required]
    public string ContentType { get; set; } = string.Empty;

    [Required]
    public MediaType MediaType { get; set; }

    [Required]
    public byte[] Data { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}