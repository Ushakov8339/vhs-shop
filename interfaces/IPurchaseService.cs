using VhsShop.dto.request;
using VhsShop.model;

namespace VhsShop.interfaces;

/// <summary>
/// Контракт сервиса покупок
/// </summary>
public interface IPurchaseService
{
    /// <summary>
    /// Возвращает историю всех покупок
    /// </summary>
    Task<IReadOnlyList<Purchase>> GetAllAsync();

    /// <summary>
    /// Возвращает одну покупку по идентификатору
    /// </summary>
    Task<Purchase?> GetByIdAsync(Guid id);

    /// <summary>
    /// Возвращает историю покупок конкретного покупателя
    /// </summary>
    Task<IReadOnlyList<Purchase>> GetByCustomerIdAsync(Guid customerId);

    /// <summary>
    /// Создаёт новую покупку.
    /// Проверяет наличие кассет на складе, списывает со склада, начисляет бонусы
    /// Бросает исключение, если входные данные невалидны
    /// </summary>
    Task<Purchase> CreateAsync(CreatePurchaseRequest request);

    /// <summary>
    /// Удаляет покупку, возвращая кассеты на склад и откатывая бонусы
    /// Возвращает false, если запись не найдена
    /// </summary>
    Task<bool> DeleteAsync(Guid id);
}

