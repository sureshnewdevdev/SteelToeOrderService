using Microsoft.AspNetCore.Mvc;
using Steeltoe.Messaging.RabbitMQ.Core;
using System.Text.Json;
using System.Xml.Linq;


namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly RabbitTemplate _rabbitTemplate;

        public OrderController(RabbitTemplate rabbitTemplate)
        {
            _rabbitTemplate = rabbitTemplate;
        }

        [HttpPost]
        [Route("place")]
        public IActionResult PlaceOrder([FromBody] Order order)
        {
            // Serialize Order object to JSON string
            var serializedOrder = JsonSerializer.Serialize(order);
            _rabbitTemplate.Mandatory = true;
       
            _rabbitTemplate.ConvertAndSend(string.Empty,"order2Queue", serializedOrder); // Send as string
            return Ok("Order placed!");
        }

        //If no queue exists with the name order2Queue, RabbitMQ will discard the message without raising an error unless you explicitly configure a mandatory flag
    }

    public record Order(int Id, string Item, double Amount);
}
