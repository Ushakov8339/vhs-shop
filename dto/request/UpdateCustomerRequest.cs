namespace VhsShop.dto.request;

/// <summary>
/// DTO обновления покупателя
/// </summary>
public record UpdateCustomerRequest
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

