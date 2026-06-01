namespace VhsShop.model;

public class PurchaseItem
{
    public Guid PurchaseId { get; set; }

    public Guid CassetteId { get; set; }

    public decimal PriceAtPurchase { get; set; }

    public Purchase? Purchase { get; set; }

    public Cassette? Cassette { get; set; }
}

