using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace RecebeRequisicaoWebhook.Controllers
{
    [ApiController]
    [Route("github/webhook")]
    public class RecebeWebhookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> RecebeWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq", //nome do conteiner porta 5672
                //HostName = "IpDaVps",
                //Port = 5672,
                UserName = "admin",
                Password = "admin123"
            };

            await using var connection = await factory.CreateConnectionAsync();

            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "github-events",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var message = Encoding.UTF8.GetBytes(body);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "github-events",
                mandatory: false,
                basicProperties: new BasicProperties(),
                body: message
            );

            return Ok(new { status = "Webhook recebido e enviado para RabbitMQ" });
        }
    }
}