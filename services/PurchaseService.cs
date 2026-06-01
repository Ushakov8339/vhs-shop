using Microsoft.EntityFrameworkCore;
using VhsShop.database;
using VhsShop.dto.request;
using VhsShop.interfaces;
using VhsShop.model;

namespace VhsShop.services;

/// <summary>
/// Сервис покупок. Основная бизнес-логика:
/// 1 Проверка наличия кассет на складе
/// 2 Списание со склада
/// 3 Расчет итоговой цены
/// 4 Использование бонусов лояльности для скидки
/// 5 Начисление новых бонусов
/// </summary>
public class PurchaseService(
    VhsShopDbContext db,
    ILoyaltyService loyalty) : IPurchaseService
{
    public async Task<IReadOnlyList<Purchase>> GetAllAsync()
        => await db.Purchases
            .AsNoTracking()
            .Include(x => x.PurchaseItems)
            .ThenInclude(x => x.Cassette)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

    public async Task<Purchase?> GetByIdAsync(Guid id)
        => await db.Purchases
            .AsNoTracking()
            .Include(x => x.PurchaseItems)
            .ThenInclude(x => x.Cassette)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IReadOnlyList<Purchase>> GetByCustomerIdAsync(Guid customerId)
        => await db.Purchases
            .AsNoTracking()
            .Include(x => x.PurchaseItems)
            .ThenInclude(x => x.Cassette)
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

    public async Task<Purchase> CreateAsync(CreatePurchaseRequest request)
    {
        if (request.CassetteIds.Length == 0)
            throw new ArgumentException("Необходимо выбрать хотя бы одну кассету.");

        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == request.CustomerId);
        if (customer is null)
            throw new InvalidOperationException($"Покупатель с ID {request.CustomerId} не найден.");

        var cassetteIds = request.CassetteIds.Distinct().ToArray();
        var cassettesList = await db.Cassettes
            .Where(c => cassetteIds.Contains(c.Id))
            .ToListAsync();

        if (cassettesList.Count != cassetteIds.Length)
            throw new InvalidOperationException("Некоторые кассеты не найдены в каталоге.");

        foreach (var cassId in request.CassetteIds)
        {
            var cassette = cassettesList.FirstOrDefault(c => c.Id == cassId);
            if (cassette is null)
                throw new InvalidOperationException($"Кассета {cassId} не найдена.");

            if (cassette.Stock <= 0)
                throw new InvalidOperationException(
                    $"Кассета '{cassette.MovieTitle}' отсутствует на складе.");
        }

        decimal totalPrice = 0;
        foreach (var cassId in request.CassetteIds)
        {
            var cassette = cassettesList.FirstOrDefault(c => c.Id == cassId);
            if (cassette is not null)
                totalPrice += cassette.Price;
        }

        int spentPoints = 0;
        if (request.PointsToSpend > 0)
        {
            spentPoints = await loyalty.SpendPointsAsync(request.CustomerId, request.PointsToSpend);
            totalPrice -= spentPoints;
            if (totalPrice < 0)
                totalPrice = 0;
        }

        foreach (var cassId in request.CassetteIds)
        {
            var cassette = await db.Cassettes.FirstOrDefaultAsync(c => c.Id == cassId);
            if (cassette is not null)
            {
                cassette.Stock--;
            }
        }

        int earnedPoints = loyalty.CalculateEarnedPoints(totalPrice);

        var purchase = new Purchase
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            CustomerName = customer.FullName,
            CassetteIds = request.CassetteIds,
            Total = totalPrice,
            CreatedAtUtc = DateTime.UtcNow,
            EarnedPoints = earnedPoints
        };

        db.Purchases.Add(purchase);
        await db.SaveChangesAsync();

        foreach (var cassId in request.CassetteIds)
        {
            var cassette = cassettesList.FirstOrDefault(c => c.Id == cassId);
            if (cassette is not null)
            {
                var purchaseItem = new PurchaseItem
                {
                    PurchaseId = purchase.Id,
                    CassetteId = cassId,
                    PriceAtPurchase = cassette.Price
                };
                db.PurchaseItems.Add(purchaseItem);
            }
        }

        await loyalty.AddPointsAsync(request.CustomerId, earnedPoints);

        await db.SaveChangesAsync();

        return purchase;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var purchase = await db.Purchases
            .Include(x => x.PurchaseItems)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (purchase is null)
            return false;

        foreach (var cassId in purchase.CassetteIds)
        {
            var cassette = await db.Cassettes.FirstOrDefaultAsync(c => c.Id == cassId);
            if (cassette is not null)
            {
                cassette.Stock++;
            }
        }

        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == purchase.CustomerId);
        if (customer is not null)
        {
            customer.LoyaltyPoints -= purchase.EarnedPoints;
            if (customer.LoyaltyPoints < 0)
                customer.LoyaltyPoints = 0;
        }

        db.Purchases.Remove(purchase);
        await db.SaveChangesAsync();

        return true;
    }
}

