using Microsoft.EntityFrameworkCore;
using VhsShop.database;
using VhsShop.interfaces;

namespace VhsShop.services;

/// <summary>
/// Сервис программы лояльности.
/// За каждые 100 рублей расходов начисляется 1 бонусный балл.
/// 1 балл = 1 рубль скидки.
/// </summary>
public class LoyaltyService(VhsShopDbContext db) : ILoyaltyService
{
    private const decimal PointsExchangeRate = 100m; // За каждые 100 рублей 1 балл

    public int CalculateEarnedPoints(decimal purchaseTotal)
    {
        return (int)(purchaseTotal / PointsExchangeRate);
    }

    public async Task AddPointsAsync(Guid customerId, int points)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
        if (customer is null)
        {
            throw new InvalidOperationException($"Покупатель с ID {customerId} не найден.");
        }

        customer.LoyaltyPoints += points;
        await db.SaveChangesAsync();
    }

    public async Task<int> SpendPointsAsync(Guid customerId, int points)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
        if (customer is null)
        {
            throw new InvalidOperationException($"Покупатель с ID {customerId} не найден.");
        }

        var pointsToSpend = Math.Min(points, customer.LoyaltyPoints);
        customer.LoyaltyPoints -= pointsToSpend;
        await db.SaveChangesAsync();

        return pointsToSpend;
    }
}

