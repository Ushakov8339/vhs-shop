using VhsShop.dto.request;
using VhsShop.dto.response;
using VhsShop.model;

namespace VhsShop.dto;

/// <summary>
/// Реализация маппера для преобразования доменных моделей в DTO
/// </summary>
public class Mapper : IMapper
{
    public CassetteResponse Map(Cassette cassette)
    {
        return new CassetteResponse
        {
            Id = cassette.Id,
            MovieTitle = cassette.MovieTitle,
            ReleaseYear = cassette.ReleaseYear,
            Director = cassette.Director,
            Genre = cassette.Genre,
            DurationMinutes = cassette.DurationMinutes,
            Condition = cassette.Condition,
            CassetteYear = cassette.CassetteYear,
            Language = cassette.Language,
            HasSubtitles = cassette.HasSubtitles,
            Price = cassette.Price,
            Stock = cassette.Stock
        };
    }

    public CustomerResponse Map(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = customer.Email
        };
    }

    public CustomerLoyaltyResponse MapLoyalty(Customer customer)
    {
        return new CustomerLoyaltyResponse
        {
            Id = customer.Id,
            FullName = customer.FullName,
            LoyaltyPoints = customer.LoyaltyPoints
        };
    }

    public PurchaseResponse Map(Purchase purchase)
    {
        return new PurchaseResponse
        {
            Id = purchase.Id,
            CustomerId = purchase.CustomerId,
            CustomerName = purchase.CustomerName,
            CreatedAtUtc = purchase.CreatedAtUtc,
            Total = purchase.Total,
            CassetteIds = purchase.CassetteIds,
            EarnedPoints = purchase.EarnedPoints
        };
    }
}

