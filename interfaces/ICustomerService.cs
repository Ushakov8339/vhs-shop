using VhsShop.dto.request;
using VhsShop.model;

namespace VhsShop.interfaces;

/// <summary>
/// Контракт сервиса работы с покупателями
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Возвращает всех покупателей
    /// </summary>
    Task<IReadOnlyList<Customer>> GetAllAsync();

    /// <summary>
    /// Возвращает покупателя по идентификатору
    /// </summary>
    Task<Customer?> GetByIdAsync(Guid id);

    /// <summary>
    /// Создаёт покупателя
    /// </summary>
    Task<Customer> AddAsync(CreateCustomerRequest request);

    /// <summary>
    /// Обновляет покупателя, возвращает null, если покупатель не найден
    /// </summary>
    Task<Customer?> UpdateAsync(Guid id, UpdateCustomerRequest request);

    /// <summary>
    /// Удаляет покупателя, возвращает false, если покупатель не найден
    /// </summary>
    Task<bool> DeleteAsync(Guid id);
}

