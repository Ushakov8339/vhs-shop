namespace VhsShop.model;

/// <summary>
/// Покупатель в магазине VHS кассет с балансом бонусов лояльности.
/// </summary>
public class Customer
{
    /// <summary>
    /// Идентификатор покупателя
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Полное имя покупателя
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Адрес электронной почты
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Текущее количество бонусных баллов программы лояльности
    /// </summary>
    public int LoyaltyPoints { get; set; }
}

