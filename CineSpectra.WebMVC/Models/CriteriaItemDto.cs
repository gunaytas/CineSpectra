using CineSpectra.Domain.Enums;

namespace CineSpectra.WebMVC.Models
{
    public class CriteriaItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Weight { get; set; }
        public CriteriaTarget Target { get; set; }
    }
}
