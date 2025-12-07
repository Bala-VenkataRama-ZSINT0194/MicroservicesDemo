using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Data.Entities;
using UserService.Messaging;
using UserService.Repositories.Implementations;
using UserService.Repositories.Interfaces;
using UserService.Services.Interfaces;
using UserService.Services.Implementations;

namespace UserService
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

            // Repositories
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();

            // Services
            builder.Services.AddScoped<IUserService, UserServiceImpl>();

            // Messaging
            builder.Services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();

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
