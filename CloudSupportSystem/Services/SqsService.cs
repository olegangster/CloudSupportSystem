
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;

namespace CloudSupportSystem.Services
{
    public class SqsService
    {
        private readonly IAmazonSQS _sqs;
        private readonly IConfiguration _configuration;

        public SqsService(
            IAmazonSQS sqs,
            IConfiguration configuration)
        {
            _sqs = sqs;
            _configuration = configuration;
        }

        public async Task SendTicketCreatedAsync(
            int id,
            string subject,
            string priority)
        {
            var queueUrl = _configuration["AWS:SQSQueueUrl"];

            if (string.IsNullOrWhiteSpace(queueUrl))
            {
                throw new InvalidOperationException(
                    "AWS SQS queue URL is not configured.");
            }

            var message = new
            {
                Id = id,
                Subject = subject,
                Priority = priority
            };

            var request = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = JsonSerializer.Serialize(message)
            };

            await _sqs.SendMessageAsync(request);
        }
    }
}
