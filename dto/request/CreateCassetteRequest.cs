namespace VhsShop.dto.request;

/// <summary>
/// DTO создания кассеты
/// </summary>
public record CreateCassetteRequest
{
    public Guid? Id { get; init; }
    public string MovieTitle { get; init; } = string.Empty;
    public int ReleaseYear { get; init; }
    public string Director { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public int DurationMinutes { get; init; }
    public string Condition { get; init; } = "Хорошее";
    public int CassetteYear { get; init; }
    public string Language { get; init; } = "Русский";
    public bool HasSubtitles { get; init; }
    public string Rating { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public string Description { get; init; } = string.Empty;
}

