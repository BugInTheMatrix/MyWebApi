using NZWalks.UI.Models.DTO;

namespace NZWalks.UI.Models
{
    // One row on the Difficulties page: a difficulty and the walks that use it
    public class DifficultyViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<WalkDto> Walks { get; set; } = new();
    }
}
