namespace NZWalks.UI.Models
{
    // Used by both the Add and Edit walk forms; Id is only set when editing
    public class WalkFormViewModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double LengthInKm { get; set; }
        public string? WalkImageUrl { get; set; }
        public Guid DifficultyId { get; set; }
        public Guid RegionId { get; set; }
    }
}
