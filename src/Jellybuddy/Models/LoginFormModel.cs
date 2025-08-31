using System.ComponentModel.DataAnnotations;
using Syncfusion.Maui.DataForm;

namespace Jellybuddy.Models
{
    public class LoginFormModel
    {
        [Display(Prompt = "Enter Jellyfin server URL", Name = "Jellyfin URL")]
        [DataFormDisplayOptions(ColumnSpan = 3)]
        public string? ServerUrl { get; set; }

        [Display(Prompt = "Enter your username", Name = "Username")]
        [DataFormDisplayOptions(ColumnSpan = 3)]
        public string? Username { get; set; }

        [Display(Prompt = "Enter your password", Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        [DataFormDisplayOptions(ColumnSpan = 3)]
        public string? Password { get; set; }
    }
}