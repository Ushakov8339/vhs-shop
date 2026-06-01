using VhsShop.model;

namespace VhsShop.interfaces;

/// <summary>
/// Контракт сервиса выгрузки текстовых чеков
/// </summary>
public interface IReceiptService
{
    /// <summary>
    /// Генерирует текстовый чек для покупки
    /// </summary>
    Task<string> GenerateReceiptAsync(Guid purchaseId);
}

