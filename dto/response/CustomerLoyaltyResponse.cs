namespace VhsShop.dto.response;

/// <summary>
/// DTO ответа со статусом программы лояльности покупателя
/// </summary>
public record CustomerLoyaltyResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public int LoyaltyPoints { get; init; }
}

