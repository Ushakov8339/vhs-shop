using VhsShop.dto.request;
using VhsShop.model;

namespace VhsShop.interfaces;

/// <summary>
/// Контракт сервиса работы с VHS кассетами
/// </summary>
public interface ICassetteService
{
    /// <summary>
    /// Возвращает все кассеты
    /// </summary>
    Task<IReadOnlyList<Cassette>> GetAllAsync();

    /// <summary>
    /// Возвращает кассету по идентификатору
    /// </summary>
    Task<Cassette?> GetByIdAsync(Guid id);

    /// <summary>
    /// Фильтрует кассеты по жанру
    /// </summary>
    Task<IReadOnlyList<Cassette>> GetByGenreAsync(string genre);

    /// <summary>
    /// Фильтрует кассеты по режиссёру
    /// </summary>
    Task<IReadOnlyList<Cassette>> GetByDirectorAsync(string director);

    /// <summary>
    /// Создаёт кассету
    /// </summary>
    Task<Cassette> AddAsync(CreateCassetteRequest request);

    /// <summary>
    /// Обновляет кассету
    /// Возвращает null если кассета не найдена
    /// </summary>
    Task<Cassette?> UpdateAsync(Guid id, UpdateCassetteRequest request);

    /// <summary>
    /// Удаляет кассету. Возвращает false, если кассета не найдена
    /// </summary>
    Task<bool> DeleteAsync(Guid id);
}

