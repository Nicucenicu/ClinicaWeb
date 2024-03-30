using System.ComponentModel.DataAnnotations;

namespace ClinicaWeb.Shared.Dtos.Users
{
    public class ChangePasswordDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        [Required, StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
