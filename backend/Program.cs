using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<TodoDb>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<AccountServices>();
builder.Services.AddScoped<TodoItemServices>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

//HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowFrontend");

if (args.Contains("--migrate"))
{
	await using var scope = app.Services.CreateAsyncScope();
	// perform the db migration
	var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
	await db.Database.MigrateAsync();
}

app.Run();