using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AppMailClient
{
    public class Mailer: IAppMailClient
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private readonly Options _options;

        public Mailer (
            ILogger<Mailer> logger,
            Options options,
            HttpClient httpClient
        ){
            _client = httpClient;
            _logger = logger;
            _options = options;
        }

        public async Task<MailMessageStatus> Send(MailMessage message)
        {

            if (String.IsNullOrEmpty(message.From))
            {
                message.From = _options.From;
            }

            message.Cc += $";{_options.CcRecipients}";

            message.Bcc += $";{_options.BccRecipients}";

            var response = await _client.PostAsync("", JsonContent(message));

            string content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MailMessageStatus>(content);
        }

        public async Task<MailMessageStatus> Status(string referenceId)
        {
            var content = await _client.GetStringAsync(referenceId);
            return JsonConvert.DeserializeObject<MailMessageStatus>(content);
        }

        private HttpContent JsonContent(object obj)
        {
            return new StringContent(
                JsonConvert.SerializeObject(obj),
                Encoding.UTF8,
                "application/json"
            );
        }
    }
}
