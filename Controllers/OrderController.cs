using Microsoft.AspNetCore.Mvc;
using Steeltoe.Messaging.RabbitMQ.Config;
using Steeltoe.Messaging.RabbitMQ.Core;
using System.Text.Json;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly RabbitAdmin _rabbitAdmin;
        private readonly RabbitTemplate _rabbitTemplate;

        public OrderController(RabbitAdmin rabbitAdmin, RabbitTemplate rabbitTemplate)
        {
            _rabbitAdmin = rabbitAdmin;
            _rabbitTemplate = rabbitTemplate;
        }

        [HttpPost]
        [Route("place")]
        public IActionResult PlaceOrder([FromBody] Order order)
        {
            string qName = "zzzzQ";
            // Check if the queue exists
            var queueExists = _rabbitAdmin.GetQueueProperties(qName) != null;

            if (!queueExists)
            {
                //throw new Exception("Queue 'order2Queue' does not exist in RabbitMQ.");


                // Dynamically create the queue
                _rabbitAdmin.DeclareQueue(new Queue(qName));

                // Optionally bind the queue to an exchange
                _rabbitAdmin.DeclareBinding(new Binding(
                    "order2QueueBinding",
                    qName,                              // Queue name
                    Binding.DestinationType.QUEUE,             // Destination type
                    "orderExchange",                            // Exchange name
                    qName,                              // Routing key
                    null                                        // Arguments (null means no additional arguments)
                ));
            }

            // Serialize Order object to JSON string
            var serializedOrder = JsonSerializer.Serialize(order);

            // Send message to RabbitMQ
            _rabbitTemplate.ConvertAndSend("orderExchange", qName, serializedOrder);

            return Ok("Order placed!");
        }
    }

    public record Order(int Id, string Item, double Amount);
}
