
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using TodoApi;

namespace TodoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ToDoDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), new MySqlServerVersion(new Version(8, 0, 36))));

            builder.Services.AddCors(); // הוספת השירות של CORS

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Todo API", Version = "v1" });
            });

            var app = builder.Build();
            
             // השתמש ב CORS
             app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
                });
            }

            app.MapGet("/tasks", async (ToDoDbContext context) =>
            {
                var tasks = await context.Items.ToListAsync();
                return Results.Ok(tasks);
            });

            app.MapPost("/tasks", async (ToDoDbContext context, Item item) =>
            {
                context.Items.Add(item);
                await context.SaveChangesAsync();
                return Results.Created($"/tasks/{item.Id}", item);
            });

            app.MapPut("/tasks/{id}", async (ToDoDbContext context, int id, Item updatedItem) =>
            {
                var existingItem = await context.Items.FindAsync(id);
                if (existingItem == null)
                    return Results.NotFound();

                existingItem.Name = updatedItem.Name;
                existingItem.IsComplete = updatedItem.IsComplete;

                await context.SaveChangesAsync();
                return Results.Ok(existingItem);
            });

            app.MapDelete("/tasks/{id}", async (ToDoDbContext context, int id) =>
            {
                var existingItem = await context.Items.FindAsync(id);
                if (existingItem == null)
                    return Results.NotFound();

                context.Items.Remove(existingItem);
                await context.SaveChangesAsync();
                return Results.NoContent();
            });

            app.Run();
        }
    }
}
