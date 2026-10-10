using System;
using System.ComponentModel.DataAnnotations;

namespace CloudSupportSystem.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter your name")]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a subject")]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Priority { get; set; } = "Medium";

        [Required]
        public string Status { get; set; } = "New";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
