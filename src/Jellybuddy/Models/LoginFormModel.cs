using System.ComponentModel.DataAnnotations;

namespace Jellybuddy.Models
{
    public class LoginFormModel
    {
        [Display(Prompt = "Enter Jellyfin server URL", Name = "Jellyfin URL")]
        public string? ServerUrl { get; set; }

        [Display(Prompt = "Enter your username", Name = "Username")]
        public string? Username { get; set; }

        [Display(Prompt = "Enter your password", Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}