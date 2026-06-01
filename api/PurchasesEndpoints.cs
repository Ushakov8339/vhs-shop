using VhsShop.dto;
using VhsShop.dto.request;
using VhsShop.dto.response;
using VhsShop.interfaces;

namespace VhsShop.api;

public static class PurchasesEndpoints
{
    /// <summary>
    /// Группа эндпоинтов для работы с покупками и бизнес-логикой магазина.
    /// </summary>
    public static RouteGroupBuilder MapPurchasesEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/purchases").WithTags("Purchases");

        group.MapGet("/", async (IPurchaseService purchases, IMapper mapper) =>
            {
                var result = await purchases.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить историю всех покупок")
            .WithDescription("Возвращает список всех покупок в системе (отсортировано по дате, новые первыми).")
            .Produces<IEnumerable<PurchaseResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{purchaseId:guid}",
                async (Guid purchaseId, IPurchaseService purchases, IMapper mapper) =>
            {
                var purchase = await purchases.GetByIdAsync(purchaseId);
                return purchase is null
                    ? Results.NotFound(new ErrorResponse { Message = "Покупка не найдена." })
                    : Results.Ok(mapper.Map(purchase));
            })
            .WithSummary("Получить детали покупки")
            .WithDescription("Возвращает полную информацию о конкретной покупке.")
            .Produces<PurchaseResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/customer/{customerId:guid}",
                async (Guid customerId, IPurchaseService purchases, IMapper mapper) =>
            {
                var result = await purchases.GetByCustomerIdAsync(customerId);
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить историю покупок покупателя")
            .WithDescription("Возвращает все покупки конкретного покупателя (новые первыми).")
            .Produces<IEnumerable<PurchaseResponse>>(StatusCodes.Status200OK)
            .ExcludeFromDescription();

        group.MapPost("/", 
                async (CreatePurchaseRequest body, IPurchaseService purchases, ILoyaltyService loyalty, IMapper mapper) =>
            {
                try
                {
                    var created = await purchases.CreateAsync(body);
                    return Results.Created($"/api/purchases/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Создать новую покупку")
            .WithDescription(
                "Основная бизнес-операция: покупка VHS кассет. " +
                "Проверяет наличие на складе, списывает кассеты, применяет скидку (если указаны бонусы), " +
                "начисляет новые бонусы по программе лояльности. " +
                "За каждые 100₽ начисляется 1 бонус, 1 бонус = 1₽ скидка.")
            .Produces<PurchaseResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("/{purchaseId:guid}/receipt",
                async (Guid purchaseId, IReceiptService receipts) =>
            {
                try
                {
                    var receipt = await receipts.GenerateReceiptAsync(purchaseId);
                    return Results.Ok(new { receipt = receipt });
                }
                catch (Exception ex) when (ex is InvalidOperationException)
                {
                    return Results.NotFound(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Получить текстовый чек по покупке")
            .WithDescription("Генерирует и возвращает красиво отформатированный текстовый чек.")
            .Produces<object>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .ExcludeFromDescription();

        group.MapDelete("/{purchaseId:guid}", async (Guid purchaseId, IPurchaseService purchases) =>
            {
                try
                {
                    var deleted = await purchases.DeleteAsync(purchaseId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Покупка не найдена." });
                }
                catch (Exception ex) when (ex is InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Отменить покупку")
            .WithDescription("Удаляет покупку, возвращает кассеты на склад и откатывает начисленные бонусы.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return group;
    }
}

