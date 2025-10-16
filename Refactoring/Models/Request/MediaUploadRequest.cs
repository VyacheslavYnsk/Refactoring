using System.ComponentModel.DataAnnotations;

public class MediaUploadRequest
{
    [Required]
    public IFormFile File { get; set; }

    [Required]
    public MediaType MediaType { get; set; }
}
