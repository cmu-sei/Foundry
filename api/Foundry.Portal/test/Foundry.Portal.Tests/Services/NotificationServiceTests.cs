/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Portal.Data;
using Foundry.Portal.Events;
using Foundry.Portal.Notifications;
using System;
using System.Linq;
using Xunit;

namespace Foundry.Portal.Tests.Services
{
    [Collection("AutoMapper")]
    public class NotificationServiceTests : ServiceTests
    {
        /// <summary>
        /// loop through each Domain Event Type and make sure that there is a notification
        /// strategy to handle that event type
        /// </summary>
        [Fact]
        public void EnsureAllDomainEventTypesHaveANotificationStrategy()
        {
            using (var init = CreateTestContext())
            {
                var notificationFactory = new NotificationFactory(init.DbContext, new CommunicationOptions() { SourceUrl = "http://localhost", Url = "http://localhost" });

                var domainEventTypes = Enum.GetValues(typeof(DomainEventType)).Cast<DomainEventType>().Where(d => d != DomainEventType.NotSet);

                foreach (var det in domainEventTypes)
                {
                    var strategy = notificationFactory.GetNotificationStrategy(new DomainEvent(Guid.NewGuid().ToString(), det.ToString(), det));
                    Assert.True(strategy != null, det + " strategy was not found.");
                }
            }
        }
    }
}
