public interface ILogService
{
    void LogUserActivity(string userId, string activity);
    void LogVideoActivity(string videoId, string activity);
    void LogSystemActivity(string activity);
}

public class LogService : ILogService
{
    private readonly ILogger<LogService> _logger;

    public LogService(ILogger<LogService> logger)
    {
        _logger = logger;
    }

    public void LogUserActivity(string userId, string activity)
    {
        _logger.LogInformation("User {UserId}: {Activity}", userId, activity);
    }

    public void LogVideoActivity(string videoId, string activity)
    {
        _logger.LogInformation("Video {VideoId}: {Activity}", videoId, activity);
    }

    public void LogSystemActivity(string activity)
    {
        _logger.LogInformation("System: {Activity}", activity);
    }
}
