using VhsShop.dto.request;
using VhsShop.dto.response;
using VhsShop.model;

namespace VhsShop.dto;

/// <summary>
/// Контракт маппера, который преобразует доменные модели в DTO и наоборот
/// </summary>
public interface IMapper
{
    CassetteResponse Map(Cassette cassette);
    CustomerResponse Map(Customer customer);
    CustomerLoyaltyResponse MapLoyalty(Customer customer);
    PurchaseResponse Map(Purchase purchase);
}

