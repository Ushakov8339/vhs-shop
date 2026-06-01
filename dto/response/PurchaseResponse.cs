namespace VhsShop.dto.response;

/// <summary>
/// DTO ответа покупки
/// </summary>
public record PurchaseResponse
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public decimal Total { get; init; }
    public Guid[] CassetteIds { get; init; } = [];
    public int EarnedPoints { get; init; }
}

