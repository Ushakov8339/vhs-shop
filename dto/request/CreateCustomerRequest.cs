namespace VhsShop.dto.request;

/// <summary>
/// DTO создания покупателя
/// </summary>
public record CreateCustomerRequest
{
    public Guid? Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

