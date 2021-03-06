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
    public class PlaylistFollowNotification : NotificationStrategy
    {
        public override DomainEventType DomainEventType => DomainEventType.PlaylistFollow;

        public override async Task<NotificationCreate> Build()
        {
            var entity = await DbContext.Playlists.SingleAsync(x => x.GlobalId.ToLower() == GlobalId);
            var ids = DbContext.Playlists
                .Where(p => p.GlobalId.ToLower() == GlobalId && p.ProfileId.HasValue)
                .Select(p => p.Profile.GlobalId).ToList().Distinct();

            var notification = new NotificationCreate
            {
                GlobalId = GlobalId,
                Subject = "Playlist Follower Added",
                Body = string.Format("A new follower for playlist '{0}'.", entity.Name),
                Values = ToValues(),
                Url = ToLocalUrl("playlist", entity.Id, entity.Slug),
                Recipients = ids.ToArray()
            };

            return notification;
        }
    }
}
