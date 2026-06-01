using VhsShop.dto;
using VhsShop.dto.request;
using VhsShop.dto.response;
using VhsShop.interfaces;

namespace VhsShop.api;

public static class CassettesEndpoints
{
    /// <summary>
    /// Группа эндпоинтов для работы с каталогом VHS кассет.
    /// </summary>
    public static RouteGroupBuilder MapCassettesEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/cassettes").WithTags("Cassettes");

        group.MapGet("/", async (ICassetteService cassettes, IMapper mapper) =>
            {
                var result = await cassettes.GetAllAsync();
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить список всех кассет")
            .WithDescription("Возвращает весь доступный каталог VHS кассет с текущими остатками.")
            .Produces<IEnumerable<CassetteResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{cassetteId:guid}",
                async (Guid cassetteId, ICassetteService cassettes, IMapper mapper) =>
            {
                var cassette = await cassettes.GetByIdAsync(cassetteId);
                return cassette is null
                    ? Results.NotFound(new ErrorResponse { Message = "Кассета не найдена." })
                    : Results.Ok(mapper.Map(cassette));
            })
            .WithSummary("Получить кассету по ID")
            .WithDescription("Возвращает информацию о конкретной VHS кассете.")
            .Produces<CassetteResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapGet("/genre/{genre}",
                async (string genre, ICassetteService cassettes, IMapper mapper) =>
            {
                var result = await cassettes.GetByGenreAsync(genre);
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить кассеты по жанру")
            .WithDescription("Возвращает все кассеты указанного жанра.")
            .Produces<IEnumerable<CassetteResponse>>(StatusCodes.Status200OK)
            .ExcludeFromDescription();

        group.MapGet("/director/{director}",
                async (string director, ICassetteService cassettes, IMapper mapper) =>
            {
                var result = await cassettes.GetByDirectorAsync(director);
                return Results.Ok(result.Select(mapper.Map));
            })
            .WithSummary("Получить кассеты по режиссёру")
            .WithDescription("Возвращает все кассеты указанного режиссёра.")
            .Produces<IEnumerable<CassetteResponse>>(StatusCodes.Status200OK)
            .ExcludeFromDescription();

        group.MapPost("/", async (CreateCassetteRequest body, ICassetteService cassettes, IMapper mapper) =>
            {
                try
                {
                    var created = await cassettes.AddAsync(body);
                    return Results.Created($"/api/cassettes/{created.Id}", mapper.Map(created));
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Добавить новую VHS кассету")
            .WithDescription("Создаёт новую запись кассеты в каталоге.")
            .Produces<CassetteResponse>(StatusCodes.Status201Created)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPut("/{cassetteId:guid}",
                async (Guid cassetteId, UpdateCassetteRequest body, ICassetteService cassettes, IMapper mapper) =>
            {
                try
                {
                    var updated = await cassettes.UpdateAsync(cassetteId, body);
                    return updated is null
                        ? Results.NotFound(new ErrorResponse { Message = "Кассета не найдена." })
                        : Results.Ok(mapper.Map(updated));
                }
                catch (Exception ex) when (ex is ArgumentException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Обновить информацию о кассете")
            .WithDescription("Изменяет данные существующей кассеты.")
            .Produces<CassetteResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        group.MapDelete("/{cassetteId:guid}", async (Guid cassetteId, ICassetteService cassettes) =>
            {
                try
                {
                    var deleted = await cassettes.DeleteAsync(cassetteId);
                    return deleted
                        ? Results.NoContent()
                        : Results.NotFound(new ErrorResponse { Message = "Кассета не найдена." });
                }
                catch (Exception ex) when (ex is InvalidOperationException)
                {
                    return Results.BadRequest(new ErrorResponse { Message = ex.Message });
                }
            })
            .WithSummary("Удалить VHS кассету")
            .WithDescription("Удаляет кассету из каталога (только если её нет в истории покупок).")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        return group;
    }
}

