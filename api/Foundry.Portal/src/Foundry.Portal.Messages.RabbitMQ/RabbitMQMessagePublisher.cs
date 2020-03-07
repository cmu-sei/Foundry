/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Foundry.Portal.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Portal.Messages.RabbitMQ
{
    /// <summary>
    /// the publisher is responsible for sending DomainEvents to RabbitMQ
    /// </summary>
    public class RabbitMQMessagePublisher : IDomainEventDelegator
    {
        const string prefix = "[RabbitMQ Message Publisher] ";

        RabbitMQOptions RabbitMQOptions { get; }
        DomainEventDispatcherOptions DomainEventDispatcherOptions { get; }
        IConnection Connection { get; set; }
        ILogger<RabbitMQMessagePublisher> Logger { get; }
        public IEnumerable<IDomainEventHandler> Handlers { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <param name="rabbitMQOptions"></param>
        public RabbitMQMessagePublisher(ILogger<RabbitMQMessagePublisher> logger, DomainEventDispatcherOptions options, RabbitMQOptions rabbitMQOptions)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            DomainEventDispatcherOptions = options ?? throw new ArgumentNullException(nameof(options));
            RabbitMQOptions = rabbitMQOptions ?? throw new ArgumentNullException(nameof(rabbitMQOptions));
        }

        void LogError(string message, params object[] args)
        {
            Logger.LogError(prefix + message, args);
        }

        void LogWarning(string message, params object[] args)
        {
            Logger.LogWarning(prefix + message, args);
        }

        void LogInformation(string message, params object[] args)
        {
            Logger.LogInformation(prefix + message, args);
        }

        public async Task<bool> IsConnectionOpen()
        {
            await StartConnection();

            return Connection != null && Connection.IsOpen;
        }

        async Task StartConnection()
        {
            var factory = new ConnectionFactory() { HostName = RabbitMQOptions.HostName };

            var delay = RabbitMQOptions.Delay;
            var maxAttempts = RabbitMQOptions.MaxAttempts;
            int attempt = 0;

            while (attempt < maxAttempts && Connection == null)
            {
                try
                {
                    Connection = factory.CreateConnection();
                    return;
                }
                catch (BrokerUnreachableException brokerUnreachableException)
                {
                    attempt++;
                    LogError("Connection attempt {0} failed. {1}", attempt, brokerUnreachableException.Message);
                    LogWarning("Trying again in {0} seconds.", delay / 1000);
                    await Task.Delay(delay);
                }
                catch (Exception ex)
                {
                    attempt++;
                    LogError("Unhandled exception. Attempt {0} failed. {1}", attempt, ex.Message);
                    LogWarning("Trying again in {0} seconds.", delay / 1000);
                    await Task.Delay(delay);
                }
            }
        }

        public async Task<IEnumerable<DomainEventResult>> Delegate(IDomainEvent e)
        {
            var results = new List<DomainEventResult>();

            await StartConnection();

            if (Connection != null)
            {
                try
                {
                    using (var channel = Connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: RabbitMQOptions.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                        var value = JsonConvert.SerializeObject(e);
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        channel.BasicPublish(exchange: "", routingKey: RabbitMQOptions.QueueName, mandatory: true, basicProperties: properties, body: Encoding.UTF8.GetBytes(value));
                        LogInformation("Sent {0}", value);
                    }
                }
                catch (Exception ex)
                {
                    LogError("Unhandled exception. Publishing message failed. {0}", ex.Message);
                    throw;
                }
            }

            return results;
        }
    }
}
