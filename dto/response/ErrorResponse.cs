namespace VhsShop.dto.response;

/// <summary>
/// DTO ошибки API
/// </summary>
public record ErrorResponse
{
    public string Message { get; init; } = string.Empty;
}

