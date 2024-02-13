using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User_Management_Api.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? HashedPassword { get; set; }
        [Required]
        public int RoleId { get; set; }

        
        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        public string PhotoUrl { get; set; } = "nullNow.png";

        public bool IsBlocked { get; set; } = false;
    }
}
