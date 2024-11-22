using Steeltoe.Messaging.RabbitMQ.Config;
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
            builder.Services.AddRabbitAdmin(); // Add this line to register RabbitAdmin
            // Learn more about configuring Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add RabbitMQ services
            builder.Services.AddRabbitServices(); // Adds RabbitMQ connection
            builder.Services.AddRabbitTemplate(); // Registers RabbitTemplate
            builder.Services.AddRabbitQueue("order2Queue"); // Adds a queue for message routing
            builder.Services.AddRabbitExchange(new DirectExchange("orderExchange")); // Add exchange

            var app = builder.Build();

            // Configure RabbitMQ at runtime
            var rabbitAdmin = app.Services.GetRequiredService<RabbitAdmin>();

            // Declare queue and exchange
            rabbitAdmin.DeclareQueue(new Queue("order2Queue"));
            rabbitAdmin.DeclareExchange(new DirectExchange("orderExchange"));

            // Bind the queue to the exchange
            var binding = new Binding(
                "order2QueueBinding",                       // Binding name
                "order2Queue",                              // Queue name (destination)
                Binding.DestinationType.QUEUE,             // Destination type
                "orderExchange",                            // Exchange name
                "order2Queue",                              // Routing key
                null                                        // Arguments (null means no additional arguments)
            );
            rabbitAdmin.DeclareBinding(binding);

            // Configure a health check for RabbitMQ
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
