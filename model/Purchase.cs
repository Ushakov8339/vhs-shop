namespace VhsShop.model;

/// <summary>
/// Покупка кассет: кто купил, какие кассеты, итоговая цена и бонусы
/// </summary>
public class Purchase
{
    /// <summary>
    /// Идентификатор покупки
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор покупателя
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Имя покупателя, зафиксированное на момент покупки
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Время создания в UTC
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Итоговая сумма покупки
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Список идентификаторов купленных кассет.
    /// Каждый id может повторяться, если одну и ту же кассету купили несколько раз
    /// </summary>
    public Guid[] CassetteIds { get; set; } = [];

    /// <summary>
    /// Количество начисленных бонусных баллов
    /// </summary>
    public int EarnedPoints { get; set; }

    public Customer? Customer { get; set; }

    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
}

