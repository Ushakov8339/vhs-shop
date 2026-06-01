namespace VhsShop.dto.response;

/// <summary>
/// DTO ответа с данными покупателя
/// </summary>
public record CustomerResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

