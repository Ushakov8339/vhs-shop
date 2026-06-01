using Microsoft.EntityFrameworkCore;
using VhsShop.database;
using VhsShop.interfaces;
using System.Text;

namespace VhsShop.services;

/// <summary>
/// Сервис генерации текстовых чеков по покупкам
/// </summary>
public class ReceiptService(VhsShopDbContext db) : IReceiptService
{
    public async Task<string> GenerateReceiptAsync(Guid purchaseId)
    {
        var purchase = await db.Purchases
            .Include(x => x.PurchaseItems)
            .ThenInclude(x => x.Cassette)
            .FirstOrDefaultAsync(x => x.Id == purchaseId);

        if (purchase is null)
        {
            throw new InvalidOperationException($"Покупка с ID {purchaseId} не найдена.");
        }

        var sb = new StringBuilder();
        sb.AppendLine("╔════════════════════════════════════════╗");
        sb.AppendLine("║     МАГАЗИН VHS КАССЕТ - ЧЕК          ║");
        sb.AppendLine("╚════════════════════════════════════════╝");
        sb.AppendLine();
        sb.AppendLine($"Номер чека: {purchase.Id}");
        sb.AppendLine($"Дата: {purchase.CreatedAtUtc:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Покупатель: {purchase.CustomerName}");
        sb.AppendLine();
        sb.AppendLine("─────────────────────────────────────────");
        sb.AppendLine("ТОВАРЫ:");
        sb.AppendLine("─────────────────────────────────────────");

        foreach (var item in purchase.PurchaseItems)
        {
            var cassette = item.Cassette;
            if (cassette is not null)
            {
                var count = purchase.PurchaseItems.Count(pi => pi.CassetteId == cassette.Id);
                var lineTotal = item.PriceAtPurchase * count;
                sb.AppendLine($"{cassette.MovieTitle}");
                sb.AppendLine($"  ({cassette.Director}, {cassette.ReleaseYear})");
                sb.AppendLine($"  {count}x {item.PriceAtPurchase:F2}₽ = {lineTotal:F2}₽");
                sb.AppendLine();
            }
        }

        sb.AppendLine("─────────────────────────────────────────");
        sb.AppendLine($"ИТОГО: {purchase.Total:F2}₽");
        sb.AppendLine($"БОНУСЫ НАЧИСЛЕНО: +{purchase.EarnedPoints} баллов");
        sb.AppendLine("─────────────────────────────────────────");
        sb.AppendLine();
        sb.AppendLine("Спасибо за покупку!");
        sb.AppendLine("Приходите ещё!");

        return sb.ToString();
    }
}

