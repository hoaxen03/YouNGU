namespace YouNGU.Areas.Admin.Models
{
    public class LogEntryViewModel
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string User { get; set; }
        public string Details { get; set; }
    }
}
