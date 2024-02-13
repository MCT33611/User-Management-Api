using System.ComponentModel.DataAnnotations;

namespace User_Management_Api.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? HashedPassword { get; set; }
    }
}
