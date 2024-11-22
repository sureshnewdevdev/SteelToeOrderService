using Steeltoe.Messaging.RabbitMQ.Core;
using Steeltoe.Messaging.RabbitMQ.Extensions;

namespace OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add RabbitMQ
            builder.Services.AddRabbitServices();
            builder.Services.AddRabbitTemplate(); // Add this line to register RabbitTemplate
            builder.Services.AddRabbitQueue("orderQueue2");

            var app = builder.Build();

            // Get RabbitTemplate from DI and configure Returns event
            var rabbitTemplate = app.Services.GetRequiredService<RabbitTemplate>();

            //rabbitTemplate.ReturnCallback += (sender, args) =>
            //{
            //    Console.WriteLine($"Message returned: {args.Message.Body}");
            //    Console.WriteLine($"Reason: {args.ReplyText}");
            //};

            app.MapGet("/", () => "Order Service is running");

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
        }
    }
}
