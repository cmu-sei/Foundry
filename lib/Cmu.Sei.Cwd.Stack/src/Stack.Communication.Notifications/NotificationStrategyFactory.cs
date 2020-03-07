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
using System;
using System.Linq;

namespace Stack.Communication.Notifications
{
    public class NotificationStrategyFactory
    {
        CommunicationOptions CommunicationOptions { get; }

        public NotificationStrategyFactory(CommunicationOptions options)
        {
            CommunicationOptions = options;
        }

        public INotificationStrategy GetNotificationStrategy(IDomainEvent domainEvent)
        {
            INotificationStrategy strategy = null;

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes());

            var notificationStrategyTypes = types.Where(p => typeof(INotificationStrategy).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface);

            var strategies = notificationStrategyTypes.Select(t => (INotificationStrategy)Activator.CreateInstance(t, new object[] { CommunicationOptions, domainEvent }));

            strategy = strategies.SingleOrDefault(s => s.Type == domainEvent.Type);

            if (strategy == null)
                throw new InvalidOperationException(string.Format("'{0}' domain event type is invalid or strategy was not found.", domainEvent.Type));

            return strategy;
        }
    }
}
