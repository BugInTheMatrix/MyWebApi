using System.ComponentModel.DataAnnotations;

namespace NZWalks.UI.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // The API only issues tokens to users with at least one role
        [Required(ErrorMessage = "Select at least one role.")]
        [MinLength(1, ErrorMessage = "Select at least one role.")]
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}
