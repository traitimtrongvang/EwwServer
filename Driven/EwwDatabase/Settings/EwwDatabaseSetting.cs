namespace EwwDatabase.Settings;

public record EwwDatabaseSetting
{
    public required string ConnectionStr { get; init; }
    
    public required string CacheConnectionStr { get; init; }
}