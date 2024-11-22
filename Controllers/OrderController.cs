using Microsoft.AspNetCore.Mvc;
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
            // Check if the queue exists
            var queueExists = _rabbitAdmin.GetQueueProperties("TestQ") != null;

            if (!queueExists)
            {
                throw new Exception("Queue 'order2Queue' does not exist in RabbitMQ.");
            }

            // Serialize Order object to JSON string
            var serializedOrder = JsonSerializer.Serialize(order);

            // Send message to RabbitMQ
            _rabbitTemplate.ConvertAndSend("orderExchange", "order2Queue", serializedOrder);

            return Ok("Order placed!");
        }
    }

    public record Order(int Id, string Item, double Amount);
}
