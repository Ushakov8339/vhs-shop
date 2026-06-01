using Microsoft.EntityFrameworkCore;
using VhsShop.database;
using VhsShop.dto.request;
using VhsShop.interfaces;
using VhsShop.model;

namespace VhsShop.services;

/// <summary>
/// Сервис доступа к покупателям через EF Core.
/// </summary>
public class CustomerService(VhsShopDbContext db) : ICustomerService
{
    public async Task<IReadOnlyList<Customer>> GetAllAsync()
        => await db.Customers
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .ToListAsync();

    public async Task<Customer?> GetByIdAsync(Guid id)
        => await db.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Customer> AddAsync(CreateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new ArgumentException("Имя покупателя не может быть пустым.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email не может быть пустым.");

        var id = request.Id ?? Guid.NewGuid();
        if (await db.Customers.AnyAsync(x => x.Id == id))
        {
            throw new InvalidOperationException($"Покупатель с идентификатором {id} уже существует.");
        }

        var entity = new Customer
        {
            Id = id,
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            LoyaltyPoints = 0
        };

        db.Customers.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<Customer?> UpdateAsync(Guid id, UpdateCustomerRequest request)
    {
        var entity = await db.Customers.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
            entity.FullName = request.FullName.Trim();

        if (!string.IsNullOrWhiteSpace(request.Email))
            entity.Email = request.Email.Trim();

        await db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await db.Customers.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return false;
        }

        db.Customers.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}

