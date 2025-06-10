using System;
using System.ComponentModel.DataAnnotations;

namespace YouNGU.Models.Entities
{
    public class Setting
    {
        [Key]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        public string Description { get; set; }

        public string Type { get; set; } = "string"; // string, int, bool, etc.

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
