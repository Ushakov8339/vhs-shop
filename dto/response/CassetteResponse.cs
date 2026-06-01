namespace VhsShop.dto.response;

/// <summary>
/// DTO ответа с данными кассеты
/// </summary>
public record CassetteResponse
{
    public Guid Id { get; init; }
    public string MovieTitle { get; init; } = string.Empty;
    public int ReleaseYear { get; init; }
    public string Director { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public int DurationMinutes { get; init; }
    public string Condition { get; init; } = string.Empty;
    public int CassetteYear { get; init; }
    public string Language { get; init; } = string.Empty;
    public bool HasSubtitles { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
}

