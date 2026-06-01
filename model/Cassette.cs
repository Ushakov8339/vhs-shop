namespace VhsShop.model;

/// <summary>
/// VHS кассета с фильмом в магазине
/// </summary>
public class Cassette
{
    /// <summary>
    /// Идентификатор кассеты
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название фильма
    /// </summary>
    public string MovieTitle { get; set; } = string.Empty;

    /// <summary>
    /// Год выпуска фильма
    /// </summary>
    public int ReleaseYear { get; set; }

    /// <summary>
    /// Режиссер фильма
    /// </summary>
    public string Director { get; set; } = string.Empty;

    /// <summary>
    /// Жанр 
    /// </summary>
    public string Genre { get; set; } = string.Empty;

    /// <summary>
    /// Длительность в минутах
    /// </summary>
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Состояние кассеты: "Отличное", "Хорошее", "Среднее", "Плохое"
    /// </summary>
    public string Condition { get; set; } = "Хорошее";

    /// <summary>
    /// Год выпуска кассеты (может отличаться от года фильма)
    /// </summary>
    public int CassetteYear { get; set; }

    /// <summary>
    /// Язык оригинала.
    /// </summary>
    public string Language { get; set; } = "Русский";

    /// <summary>
    /// Наличие субтитров.
    /// </summary>
    public bool HasSubtitles { get; set; }

    /// <summary>
    /// Рейтинг: G, PG, PG-13, R и т.д.
    /// </summary>
    public string Rating { get; set; } = string.Empty;

    /// <summary>
    /// Цена кассеты
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Количество на складе.
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// Краткое описание фильма
    /// </summary>
    public string Description { get; set; } = string.Empty;

    public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
}

