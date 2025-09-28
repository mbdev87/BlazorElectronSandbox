namespace LogAnalytics.Client.Services;

public record LogEntry(
    string Timestamp,
    string Level,
    string Message,
    string Source,
    int? ThreadId = null,
    string? BuildId = null
);

public record PagedResult<T>(
    T[] Data,
    int TotalCount,
    int PageIndex,
    int PageSize,
    bool HasMore
);

public record LogStats(
    int TotalRows,
    string[] Columns
);

public interface ILogProcessingService
{
    Task<bool> InitializeAsync();
    Task<bool> LoadLogDataAsync(byte[]? fileData = null);
    Task<PagedResult<LogEntry>> GetPageAsync(int pageIndex, int pageSize = 100);
    Task<PagedResult<LogEntry>> SearchAsync(string query, int pageIndex = 0, int pageSize = 100);
    Task<LogStats?> GetStatsAsync();
    bool IsClientSideCapable { get; }
    string RuntimeContext { get; }
}