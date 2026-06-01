namespace VhsShop.model;

/// <summary>
/// Модель для работы с текстовыми чеками из покупок.
/// </summary>
public class ReceiptFile
{
    /// <summary>
    /// Номер чека (ID покупки)
    /// </summary>
    public Guid PurchaseId { get; set; }

    /// <summary>
    /// Содержимое чека в виде текста
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания чека
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    public Purchase? Purchase { get; set; }
}

