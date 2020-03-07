/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using Foundry.Portal.Data;
using Foundry.Portal.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Notifications.Strategies
{
    public class ContentAddNotification : NotificationStrategy
    {
        public override DomainEventType DomainEventType => DomainEventType.ContentAdd;

        public override async Task<NotificationCreate> Build()
        {
            var entity = await DbContext.Contents.SingleAsync(x => x.GlobalId.ToLower() == GlobalId);

            var notification = new NotificationCreate
            {
                GlobalId = GlobalId,
                Subject = "Content Added",
                Body = string.Format("Content '{0}' was created.", entity.Name),
                Values = ToValues(),
                Url = ToLocalUrl("content", entity.Id, entity.Slug),
                Recipients = new string[] { GlobalId }
            };

            return notification;
        }
    }
}
