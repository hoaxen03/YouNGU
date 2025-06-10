using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YouNGU.Models
{
    [Table("Logs")]
    public class Log
    {
        [Key]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        [Required]
        [StringLength(50)]
        public string Level { get; set; }

        [Required]
        public string Message { get; set; }

        [StringLength(255)]
        public string Category { get; set; }

        [StringLength(255)]
        public string UserId { get; set; }

        public string Exception { get; set; }

        public string AdditionalInfo { get; set; }

        [StringLength(255)]
        public string RequestPath { get; set; }

        [StringLength(50)]
        public string RequestMethod { get; set; }

        [StringLength(50)]
        public string SourceContext { get; set; }

        [StringLength(50)]
        public string ActionName { get; set; }

        [StringLength(50)]
        public string ApplicationName { get; set; }

        [StringLength(50)]
        public string MachineName { get; set; }

        [StringLength(50)]
        public string ThreadId { get; set; }

        [StringLength(50)]
        public string ProcessId { get; set; }
    }
}
