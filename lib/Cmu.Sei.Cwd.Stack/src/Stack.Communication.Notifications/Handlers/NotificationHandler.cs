/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Stack.DomainEvents;
using Stack.Http.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stack.Communication.Notifications
{
    /// <summary>
    /// the notification handler processes the domain event calls to the communication api
    /// </summary>
    public class NotificationHandler : INotificationHandler
    {
        CommunicationOptions CommunicationOptions { get; }
        public CommunicationClient CommunicationClient { get; }

        public NotificationHandler(AuthorizationOptions authorizationOptions, CommunicationOptions communicationOptions)
        {
            CommunicationOptions = communicationOptions;
            CommunicationClient = new CommunicationClient(authorizationOptions, communicationOptions);
        }

        /// <summary>
        /// process a domain event
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainEventResult>> Process(IDomainEvent e)
        {
            var results = new List<DomainEventResult>();

            var result = new DomainEventResult
            {
                Start = DateTime.UtcNow,
                Event = e
            };

            try
            {
                var factory = new NotificationStrategyFactory(CommunicationOptions);

                var strategy = factory.GetNotificationStrategy(e);

                if (strategy is INotificationCreateStrategy create)
                {
                    var model = await create.GetModel();
                    await CommunicationClient.PostAsync(model);
                }

                if (strategy is INotificationDeleteStrategy delete)
                {
                    var id = await delete.GetModel();
                    await CommunicationClient.DeleteAsync(id);
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            result.Finish = DateTime.UtcNow;
            results.Add(result);

            return results;
        }
    }
}
