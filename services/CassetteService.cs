using Microsoft.EntityFrameworkCore;
using VhsShop.database;
using VhsShop.dto.request;
using VhsShop.interfaces;
using VhsShop.model;

namespace VhsShop.services;

/// <summary>
/// Сервис доступа к VHS кассетам через EF Core
/// </summary>
public class CassetteService(VhsShopDbContext db) : ICassetteService
{
    public async Task<IReadOnlyList<Cassette>> GetAllAsync()
        => await db.Cassettes
            .AsNoTracking()
            .OrderBy(x => x.MovieTitle)
            .ToListAsync();

    public async Task<Cassette?> GetByIdAsync(Guid id)
        => await db.Cassettes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IReadOnlyList<Cassette>> GetByGenreAsync(string genre)
        => await db.Cassettes
            .AsNoTracking()
            .Where(x => x.Genre.ToLower().Contains(genre.ToLower()))
            .OrderBy(x => x.MovieTitle)
            .ToListAsync();

    public async Task<IReadOnlyList<Cassette>> GetByDirectorAsync(string director)
        => await db.Cassettes
            .AsNoTracking()
            .Where(x => x.Director.ToLower().Contains(director.ToLower()))
            .OrderBy(x => x.MovieTitle)
            .ToListAsync();

    public async Task<Cassette> AddAsync(CreateCassetteRequest request)
    {
        ValidateCassetteFields(request);

        var id = request.Id ?? Guid.NewGuid();
        if (await db.Cassettes.AnyAsync(x => x.Id == id))
        {
            throw new InvalidOperationException($"Кассета с идентификатором {id} уже существует.");
        }

        var entity = new Cassette
        {
            Id = id,
            MovieTitle = request.MovieTitle.Trim(),
            ReleaseYear = request.ReleaseYear,
            Director = request.Director.Trim(),
            Genre = request.Genre.Trim(),
            DurationMinutes = request.DurationMinutes,
            Condition = request.Condition,
            CassetteYear = request.CassetteYear,
            Language = request.Language,
            HasSubtitles = request.HasSubtitles,
            Rating = request.Rating,
            Price = request.Price,
            Stock = request.Stock,
            Description = request.Description.Trim()
        };

        db.Cassettes.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Cassette?> UpdateAsync(Guid id, UpdateCassetteRequest request)
    {
        var entity = await db.Cassettes.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(request.MovieTitle))
            entity.MovieTitle = request.MovieTitle.Trim();

        if (!string.IsNullOrWhiteSpace(request.Condition))
            entity.Condition = request.Condition;

        if (!string.IsNullOrWhiteSpace(request.Language))
            entity.Language = request.Language;

        entity.HasSubtitles = request.HasSubtitles;
        entity.Price = request.Price;
        entity.Stock = request.Stock;

        if (!string.IsNullOrWhiteSpace(request.Description))
            entity.Description = request.Description.Trim();

        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await db.Cassettes.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return false;
        }

        // Проверка: нельзя удалить кассету, если она есть в истории покупок
        var inPurchases = await db.Purchases
            .AnyAsync(p => p.CassetteIds.Contains(id));

        if (inPurchases)
        {
            throw new InvalidOperationException(
                "Нельзя удалить кассету, которая входит в историю покупок.");
        }

        db.Cassettes.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    private void ValidateCassetteFields(CreateCassetteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.MovieTitle))
            throw new ArgumentException("Название фильма не может быть пустым.");

        if (request.ReleaseYear < 1900 || request.ReleaseYear > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Год выпуска фильма некорректен.");

        if (request.CassetteYear < 1980 || request.CassetteYear > DateTime.UtcNow.Year)
            throw new ArgumentException("Год выпуска кассеты должен быть от 1980 до текущего года.");

        if (request.Price <= 0)
            throw new ArgumentException("Цена должна быть больше нуля.");

        if (request.Stock < 0)
            throw new ArgumentException("Количество на складе не может быть отрицательным.");

        if (request.DurationMinutes <= 0)
            throw new ArgumentException("Длительность должна быть больше нуля.");

        var validConditions = new[] { "Отличное", "Хорошее", "Среднее", "Плохое" };
        if (!validConditions.Contains(request.Condition))
            throw new ArgumentException("Состояние кассеты должно быть одно из: Отличное, Хорошее, Среднее, Плохое.");
    }
}

