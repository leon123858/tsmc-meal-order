using mail.Model;
using mail.Repository;
using mail.Service;
using mail.utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<DatabaseSetting>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<CloudSetting>(builder.Configuration.GetSection("Cloud"));
builder.Services.Configure<SMTPSetting>(builder.Configuration.GetSection("SMTP"));
builder.Services.AddSingleton<MailRepository>();
builder.Services.AddSingleton<Pubsub>();
builder.Services.AddSingleton<MailService>();

// add json custom converter
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new ByteArrayJsonConverter());
});

builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseAuthorization();

app.MapControllers();

app.Run();