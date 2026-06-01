using VhsShop.model;

namespace VhsShop.interfaces;

/// <summary>
/// Контракт сервиса программы лояльности
/// </summary>
public interface ILoyaltyService
{
    /// <summary>
    /// Рассчитывает количество бонусных баллов для покупки, за каждые 100 рублей расходов начисляется 1 балл
    /// </summary>
    int CalculateEarnedPoints(decimal purchaseTotal);

    /// <summary>
    /// Начисляет бонусные баллы покупателю
    /// </summary>
    Task AddPointsAsync(Guid customerId, int points);

    /// <summary>
    /// Снимает бонусные баллы у покупателя, возвращает количество действительно снятых баллов (не может быть больше текущего баланса).
    /// </summary>
    Task<int> SpendPointsAsync(Guid customerId, int points);
}

