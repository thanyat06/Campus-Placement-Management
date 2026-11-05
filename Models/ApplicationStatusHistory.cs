using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusPlacementManagement.Models
{
    public class ApplicationStatusHistory
    {
        [Key]
        public int HistoryId { get; set; }

        public int ApplicationId { get; set; }

        public string? PreviousStatus { get; set; }

        public string NewStatus { get; set; } = string.Empty;

        public string UpdatedByUserId { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ApplicationId")]
        public JobApplication JobApplication { get; set; } = null!;

        [ForeignKey("UpdatedByUserId")]
        public ApplicationUser UpdatedByUser { get; set; } = null!;
    }
}