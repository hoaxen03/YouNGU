namespace YouNGU.Areas.Admin.Models
{
    public class LogViewModel
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Category { get; set; }
        public string UserId { get; set; }
        public string Exception { get; set; }
        public string AdditionalInfo { get; set; }
        public string RequestPath { get; set; }
        public string RequestMethod { get; set; }
        public string SourceContext { get; set; }
        public string ActionName { get; set; }
        public string ApplicationName { get; set; }
        public string MachineName { get; set; }
    }

    public class LogsViewModel
    {
        public IEnumerable<LogViewModel> Logs { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SearchTerm { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalLogs { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Levels { get; set; }
    }
}
