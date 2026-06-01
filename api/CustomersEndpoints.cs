using VhsShop.dto;
using VhsShop.dto.request;
using VhsShop.dto.response;
using VhsShop.interfaces;

namespace VhsShop.api;

public static class CustomersEndpoints
{
    /// <summary>
    /// Группа энодпоинтов для работы с покупателями.
    /// </summary>
    public static RouteGroupBuilder MapCustomersEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/customers").WithTags("Customers");

        group.MapGet("/", async (ICustomerService customers, IMapper mapper) =>
            {
                var result = await customers.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список всех покупателей")
            .WithDescription("Возвращает список всех зарегистрированных покупателей.")
            .Produces<IEnumerable<CustomerResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{customerId:guid}",
                async (Guid customerId, ICustomerService customers, IMapper mapper) =>
            {
                var customer = await customers.GetByIdAsync(customerId);
                return customer is null
                    ? Results.NotFound(new ErrorResponse { Message = "Покупатель не найден." })
                    : Results.Ok(mapper.Map(customer));
            })
            .WithSummary("Получить покупателя по ID")
            .WithDescription("Возвращает информацию о конкретном покупателе.")
            .Produces<CustomerResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/{customerId:guid}/loyalty",
                async (Guid customerId, ICustomerService customers, IMapper mapper) =>
            {
                var customer = await customers.GetByIdAsync(customerId);
                return customer is null
                    ? Results.NotFound(new ErrorResponse { Message = "Покупатель не найден." })
                    : Results.Ok(mapper.MapLoyalty(customer));
            })
            .WithSummary("Получить статус программы лояльности")
            .WithDescription("Возвращает имя покупателя и его текущий баланс бонусных баллов.")
            .Produces<CustomerLoyaltyResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .ExcludeFromDescription();

        group.MapPost("/", async (CreateCustomerRequest body, ICustomerService customers, IMapper mapper) =>
            {
                try
                {
                    var created = await customers.AddAsync(body);
                    return Results.Created($"/api/customers/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Зарегистрировать нового покупателя")
            .WithDescription("Создаёт профиль нового покупателя с начальным балансом 0 баллов.")
            .Produces<CustomerResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{customerId:guid}",
                async (Guid customerId, UpdateCustomerRequest body, ICustomerService customers, IMapper mapper) =>
            {
                try
                {
                    var updated = await customers.UpdateAsync(customerId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Покупатель не найден." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (Exception ex) when (ex is ArgumentException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Обновить профиль покупателя")
            .WithDescription("Изменяет имя и email покупателя.")
            .Produces<CustomerResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{customerId:guid}", async (Guid customerId, ICustomerService customers) =>
            {
                var deleted = await customers.DeleteAsync(customerId);
                return deleted
                    ? Results.NoContent()
                    : Results.NotFound(new ErrorResponse { Message = "Покупатель не найден." });
            })
            .WithSummary("Удалить покупателя")
            .WithDescription("Удаляет профиль покупателя из системы.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return group;
    }
}

