using System.ComponentModel.DataAnnotations;

namespace MyWebApiProject.Models.DTO
{
    public class RegionDto
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(3)]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; }
    }
}
