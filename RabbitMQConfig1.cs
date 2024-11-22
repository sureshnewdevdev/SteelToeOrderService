using Steeltoe.Messaging.RabbitMQ.Config;
using Steeltoe.Messaging.RabbitMQ.Core;

public class RabbitMQConfig
{
    public static void ConfigureRabbitMQ(RabbitAdmin rabbitAdmin)
    {
        // Declare exchange
        var exchange = new DirectExchange("orderExchange");
        rabbitAdmin.DeclareExchange(exchange);

        // Declare queue
        var queue = new Queue("order2Queue");
        rabbitAdmin.DeclareQueue(queue);

        // Bind queue to exchange with a routing key
        //var binding = new Binding("order2Queue", Binding.DestinationType.QUEUE, "orderExchange", "order2Queue", null);
        //rabbitAdmin.DeclareBinding(binding);
    }
}

