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
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

    public async Task<Purchase?> GetByIdAsync(Guid id)
        => await db.Purchases
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IReadOnlyList<Purchase>> GetByCustomerIdAsync(Guid customerId)
        => await db.Purchases
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

    public async Task<Purchase> CreateAsync(CreatePurchaseRequest request)
    {
        // Валидация
        if (request.CassetteIds.Length == 0)
            throw new ArgumentException("Необходимо выбрать хотя бы одну кассету.");

        // Проверка существования покупателя
        var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == request.CustomerId);
        if (customer is null)
            throw new InvalidOperationException($"Покупатель с ID {request.CustomerId} не найден.");

        // Получаем инфо по всем кассетам
        var cassetteIds = request.CassetteIds.Distinct().ToArray();
        var cassettesList = await db.Cassettes
            .Where(c => cassetteIds.Contains(c.Id))
            .ToListAsync();

        if (cassettesList.Count != cassetteIds.Length)
            throw new InvalidOperationException("Некоторые кассеты не найдены в каталоге.");

        // Проверяем наличие на складе
        foreach (var cassId in request.CassetteIds)
        {
            var cassette = cassettesList.FirstOrDefault(c => c.Id == cassId);
            if (cassette is null)
                throw new InvalidOperationException($"Кассета {cassId} не найдена.");

            if (cassette.Stock <= 0)
                throw new InvalidOperationException(
                    $"Кассета '{cassette.MovieTitle}' отсутствует на складе.");
        }

        // Рассчитываем базовую цену (сумма цен всех выбранных кассет)
        decimal totalPrice = 0;
        foreach (var cassId in request.CassetteIds)
        {
            var cassette = cassettesList.FirstOrDefault(c => c.Id == cassId);
            if (cassette is not null)
                totalPrice += cassette.Price;
        }

        // Применяем скидку за бонусы (если покупатель захотел их потратить)
        int spentPoints = 0;
        if (request.PointsToSpend > 0)
        {
            spentPoints = await loyalty.SpendPointsAsync(request.CustomerId, request.PointsToSpend);
            totalPrice -= spentPoints; // 1 балл = 1 рубль скидка
            if (totalPrice < 0)
                totalPrice = 0;
        }

        // Списываем со склада все выбранные кассеты
        foreach (var cassId in request.CassetteIds)
        {
            var cassette = await db.Cassettes.FirstOrDefaultAsync(c => c.Id == cassId);
            if (cassette is not null)
            {
                cassette.Stock--;
            }
        }

        // Рассчитываем новые бонусы по финальной цене (после скидки)
        int earnedPoints = loyalty.CalculateEarnedPoints(totalPrice);

        // Создаём запись о покупке
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

        // Начисляем бонусы
        await loyalty.AddPointsAsync(request.CustomerId, earnedPoints);

        await db.SaveChangesAsync();

        return purchase;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var purchase = await db.Purchases.FirstOrDefaultAsync(x => x.Id == id);
        if (purchase is null)
            return false;

        // Возвращаем кассеты на склад
        foreach (var cassId in purchase.CassetteIds)
        {
            var cassette = await db.Cassettes.FirstOrDefaultAsync(c => c.Id == cassId);
            if (cassette is not null)
            {
                cassette.Stock++;
            }
        }

        // Откатываем начисленные бонусы
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

