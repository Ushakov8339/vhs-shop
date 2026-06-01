using Microsoft.EntityFrameworkCore;
using VhsShop.api;
using VhsShop.database;
using VhsShop.dto;
using VhsShop.interfaces;
using VhsShop.services;
var builder = WebApplication.CreateBuilder(args);

// SWAGGER / OPENAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "VHS Shop API",
        Version = "v1",
        Description = "Backend магазина VHS кассет: каталог, клиенты, покупки, лояльность, чеки.",
        Contact = new()
        {
            Name = "VHS Shop Support",
            Email = "support@vhs-shop.local"
        }
    });
});
// DATABASE
var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException(
        "Не найдена строка подключения 'ConnectionStrings:Postgres' в appsettings.json. " +
        "Убедитесь, что PostgreSQL запущен и строка подключения указана правильно.");

builder.Services.AddDbContext<VhsShopDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
// DEPENDENCY INJECTION (Services)
builder.Services.AddScoped<ICassetteService, CassetteService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IMapper, Mapper>();
var app = builder.Build();
// DATABASE INITIALIZATION
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<VhsShopDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
}
// SWAGGER UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "VHS Shop API v1");
        options.RoutePrefix = "swagger";
    });
}


// ROUTING & ENDPOINTS
// Корневой эндпоинт с информацией об API
app.MapGet("/", () =>
    {
        return Results.Ok(new
        {
            title = "VHS Shop API",
            version = "v1",
            description = "Backend магазина VHS кассет",
            endpoints = new
            {
                cassettes = new { get_all = "/api/cassettes", get_one = "/api/cassettes/{id}" },
                customers = new { get_all = "/api/customers", get_one = "/api/customers/{id}" },
                purchases = new { get_all = "/api/purchases", get_one = "/api/purchases/{id}", create = "POST /api/purchases" },
                swagger = "/swagger"
            }
        });
    })
    .WithName("Root")
    .WithSummary("Информация об API")
    .WithDescription("Возвращает общую информацию и ссылки на основные endpoint-ы.");

// Регистрация групп эндпоинтов
var apiGroup = app.MapGroup("/api");
apiGroup.MapCassettesEndpoints();
apiGroup.MapCustomersEndpoints();
apiGroup.MapPurchasesEndpoints();

app.Run();
