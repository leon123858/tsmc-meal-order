using order.Config;
using order.Repository;
using order.Repository.SqlImplement;
using order.Repository.TestImplement;
using order.Repository.WebImplement;
using order.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<WebConfig>(builder.Configuration.GetSection("WebApi"));

// For Test
//builder.Services.AddSingleton<IUserRepository, MemoryUserRepository>();
//builder.Services.AddSingleton<IOrderRepository, MemoryOrderRepository>();
//builder.Services.AddSingleton<IFoodItemRepository, MemoryFoodItemRepository>();

builder.Services.AddSingleton<IUserRepository, WebUserRepository>();
builder.Services.AddSingleton<IOrderRepository, SqlOrderRepository>(); 
builder.Services.AddSingleton<IFoodItemRepository, WebFoodItemRepository>();
builder.Services.AddSingleton<MailService>();
builder.Services.AddSingleton<OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseCors();

// do not need to use https
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();