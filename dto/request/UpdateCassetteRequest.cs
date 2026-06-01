namespace VhsShop.dto.request;

/// <summary>
/// DTO обновления кассеты
/// </summary>
public record UpdateCassetteRequest
{
    public string MovieTitle { get; init; } = string.Empty;
    public string Condition { get; init; } = string.Empty;
    public string Language { get; init; } = string.Empty;
    public bool HasSubtitles { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public string Description { get; init; } = string.Empty;
}

