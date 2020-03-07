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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stack.Communication.Notifications
{
    /// <summary>
    /// base strategy to create notifications
    /// </summary>
    public abstract class NotificationCreateStrategy : INotificationCreateStrategy
    {
        public CommunicationOptions CommunicationOptions { get; }

        public IDomainEvent DomainEvent { get; }

        public NotificationCreateStrategy(CommunicationOptions communicationOptions, IDomainEvent domainEvent)
        {
            CommunicationOptions = communicationOptions;
            DomainEvent = domainEvent;
        }

        public abstract string Label { get; }
        public abstract string Type { get; }

        public abstract Task<NotificationCreate> GetModel();

        public virtual List<NotificationCreateValue> ToNotificationCreateValues()
        {
            return new List<NotificationCreateValue>() {
                new NotificationCreateValue { Key = "type", Value = DomainEvent.Type },
                new NotificationCreateValue { Key = "label", Value = Label }
            };
        }
    }
}

