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
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Foundry.Portal.Calculators;
using Foundry.Portal.Events;
using Foundry.Portal.Notifications;
using Foundry.Portal.WebHooks;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Portal.Messages.RabbitMQ
{
    /// <summary>
    /// wires up message consumer to process incoming DomainEvents
    /// </summary>
    public class RabbitMQMessageConsumer : DomainEventDelegator, IMessageConsumer, IDisposable
    {
        bool _disposed = false;
        IConnection Connection { get; set; }
        IModel RabbitMQChannel { get; set; }
        ILogger<RabbitMQMessageConsumer> Logger { get; }
        RabbitMQOptions RabbitMQOptions { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <param name="rabbitMQOptions"></param>
        /// <param name="webHookHandler"></param>
        /// <param name="notificationHandler"></param>
        /// <param name="contentRatingCalculator"></param>
        /// <param name="playlistRatingCalculator"></param>
        /// <param name="contentDifficultyCalculator"></param>
        public RabbitMQMessageConsumer(
            ILogger<RabbitMQMessageConsumer> logger,
            DomainEventDispatcherOptions options,
            RabbitMQOptions rabbitMQOptions,
            IWebHookHandler webHookHandler,
            INotificationHandler notificationHandler,
            IContentRatingCalculator contentRatingCalculator,
            IPlaylistRatingCalculator playlistRatingCalculator,
            IContentDifficultyCalculator contentDifficultyCalculator)
            : base(options, webHookHandler, notificationHandler, contentRatingCalculator, playlistRatingCalculator, contentDifficultyCalculator)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            RabbitMQOptions = rabbitMQOptions ?? throw new ArgumentNullException(nameof(rabbitMQOptions));
        }

        const string LoggerPrefix = "[RabbitMQ Message Consumer] ";

        void LogError(string message, params object[] args)
        {
            Logger.LogError(LoggerPrefix + message, args);
        }

        void LogWarning(string message, params object[] args)
        {
            Logger.LogWarning(LoggerPrefix + message, args);
        }

        void LogInformation(string message, params object[] args)
        {
            Logger.LogInformation(LoggerPrefix + message, args);
        }

        /// <summary>
        /// starts listening for DomainEvents in the RabbitMQ queue specified in Options:DomainEventDispatcher:Key
        /// </summary>
        public void Start()
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
                }
                catch (BrokerUnreachableException brokerUnreachableException)
                {
                    attempt++;
                    LogError("Connection attempt {0} failed. {1}", attempt, brokerUnreachableException.Message);
                    LogWarning("Trying again in {0} seconds.", delay / 1000);
                    Task.Delay(delay).Wait();
                }
                catch (Exception ex)
                {
                    attempt++;
                    LogError("Unhandled exception. Attempt {0} failed. {1}", attempt, ex.Message);
                    LogWarning("Trying again in {0} seconds.", delay / 1000);
                    Task.Delay(delay).Wait();
                }
            }

            if (Connection != null)
            {
                try
                {
                    RabbitMQChannel = Connection.CreateModel();
                    RabbitMQChannel.QueueDeclare(queue: RabbitMQOptions.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    RabbitMQChannel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    var consumer = new EventingBasicConsumer(RabbitMQChannel);
                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            var value = Encoding.UTF8.GetString(ea.Body);

                            LogInformation("Received {0}", value);
                            var domainEvent = JsonConvert.DeserializeObject<DomainEvent>(value);

                            if (domainEvent == null)
                            {
                                LogWarning("Could not handle {0}", value);
                            }
                            else
                            {
                                var result = Delegate(domainEvent).Result;
                                LogInformation("Handled {0}", domainEvent.Key);
                                RabbitMQChannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogError("Unhandled exception. Handling received message failed. {0}", ex.Message);
                            throw;
                        }
                    };

                    RabbitMQChannel.BasicConsume(queue: RabbitMQOptions.QueueName, autoAck: false, consumer: consumer);
                }
                catch (Exception ex)
                {
                    LogError("Unhandled exception. Failed to establish listener. {0}", ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// stops the listener
        /// </summary>
        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                RabbitMQChannel.Dispose();
                Connection.Dispose();
            }
        }
    }
}
