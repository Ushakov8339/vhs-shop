namespace VhsShop.dto.request;

/// <summary>
/// DTO создания покупки
/// Содержит данные о покупателе, выбранных кассетах и скидке лояльности
/// </summary>
public record CreatePurchaseRequest
{
    /// <summary>
    /// ID покупателя
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    /// Массив ID кассет для покупки
    /// </summary>
    public Guid[] CassetteIds { get; init; } = [];

    /// <summary>
    /// Количество бонусных баллов для использования в скидке
    /// Если больше, чем у покупателя, будет использовано максимально доступное
    /// </summary>
    public int PointsToSpend { get; init; } = 0;
}

