using System.ComponentModel.DataAnnotations;

namespace ProfileService.Models;

public class Photo
{
    [Key]
    public int Id  { get; set; }
    public byte[] Photo { get; set; }
    public string ContentType { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}