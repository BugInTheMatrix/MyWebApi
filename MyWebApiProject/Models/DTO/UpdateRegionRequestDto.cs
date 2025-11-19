using System.ComponentModel.DataAnnotations;

namespace MyWebApiProject.Models.DTO
{
    public class UpdateRegionRequestDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Minimum 3 characters required")]
        [MaxLength(3, ErrorMessage = "Maximum 3 characters required")]
        public string Code { get; set; }
        [Required]
        [MaxLength(100,ErrorMessage ="Name has to be maximumof 100 characters or less")]
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; }
    }
}
