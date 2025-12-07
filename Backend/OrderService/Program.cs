using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Data.Entities;
using OrderService.Messaging;
using OrderService.Repositories.Implementations;
using OrderService.Repositories.Interfaces;
using OrderService.Services.Implementations;
using OrderService.Services.Interfaces;

namespace OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(Program));

            // HttpClient for calling UserService
            builder.Services.AddHttpClient<IOrderService, OrderServiceImpl>();

            // Repositories
            builder.Services.AddScoped<IRepository<Order>, Repository<Order>>();

            // Services
            builder.Services.AddScoped<IOrderService, OrderServiceImpl>();

            // Messaging
            builder.Services.AddSingleton<IMessageConsumer, RabbitMQConsumer>();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Start RabbitMQ consumer
            var messageConsumer = app.Services.GetRequiredService<IMessageConsumer>();
            messageConsumer.StartConsumingAsync();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
