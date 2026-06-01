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
            .FirstOrDefaultAsync(x => x.Id == purchaseId);

        if (purchase is null)
        {
            throw new InvalidOperationException($"Покупка с ID {purchaseId} не найдена.");
        }

        // Загружаем информацию о кассетах
        var cassettes = await db.Cassettes
            .Where(c => purchase.CassetteIds.Contains(c.Id))
            .ToListAsync();

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

        foreach (var cassette in cassettes)
        {
            var count = purchase.CassetteIds.Count(id => id == cassette.Id);
            var lineTotal = cassette.Price * count;
            sb.AppendLine($"{cassette.MovieTitle}");
            sb.AppendLine($"  ({cassette.Director}, {cassette.ReleaseYear})");
            sb.AppendLine($"  {count}x {cassette.Price:F2}₽ = {lineTotal:F2}₽");
            sb.AppendLine();
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

