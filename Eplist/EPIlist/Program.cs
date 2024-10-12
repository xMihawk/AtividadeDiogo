using Microsoft.EntityFrameworkCore;
using Namespace.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDataContext>(
    options => options.UseSqlite("Data Source=ecommerce.db;Cache=shared")
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors
(
    c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
);

app.MapControllers();

app.Run();
