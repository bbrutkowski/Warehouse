using Warehouse.BussinessLogic;
using Warehouse.BussinessLogic.Interfaces;
using Warehouse.Context;
using Warehouse.Repository;
using Warehouse.Repository.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DataContext>();

builder.Services.AddScoped<IDownloadService, DownloadService>();
builder.Services.AddScoped<IDownloadRepository, DownloadRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

builder.Services.AddScoped<IPriceService, PriceService>();
builder.Services.AddScoped<IPriceRepository, PriceRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
