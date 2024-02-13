using System.ComponentModel.DataAnnotations;

namespace User_Management_Api.Model
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        public string? RoleName { get; set; }
    }
}
