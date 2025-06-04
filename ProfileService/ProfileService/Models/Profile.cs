using System.ComponentModel.DataAnnotations;

namespace ProfileService.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
