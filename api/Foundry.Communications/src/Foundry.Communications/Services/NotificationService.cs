/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Foundry.Communications.Data;
using Foundry.Communications.Data.Entities;
using Foundry.Communications.Data.Repositories;
using Foundry.Communications.Hubs;
using Foundry.Communications.Identity;
using Foundry.Communications.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Communications.Services
{
    /// <summary>
    /// notification service
    /// </summary>
    public class NotificationService : StackIdentityRepositoryService<CommunicationDbContext, INotificationRepository, Notification>
    {
        IHubContext<NotificationHub> NotificationHubContext { get; }

        /// <summary>
        /// create notification service
        /// </summary>
        /// <param name="notificationHubContext"></param>
        /// <param name="stackIdentityResolver"></param>
        /// <param name="notificationRepository"></param>
        /// <param name="mapper"></param>
        public NotificationService(IHubContext<NotificationHub> notificationHubContext, IStackIdentityResolver stackIdentityResolver, INotificationRepository notificationRepository, IMapper mapper)
            : base(stackIdentityResolver, notificationRepository, mapper)
        {
            NotificationHubContext = notificationHubContext ?? throw new ArgumentNullException(nameof(notificationHubContext));
        }

        /// <summary>
        /// get all notifications for identity
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<PagedResult<Notification, NotificationSummary>> GetAll(NotificationDataFilter filter = null)
        {
            var query = Repository.GetAll(Identity.Id);

            return await PagedResultFactory.Execute<Notification, NotificationSummary>(query, filter, Identity);
        }

        /// <summary>
        /// get notification by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Notification> GetById(int id)
        {
            return await Repository.GetAll(Identity.Id).SingleOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// delete notification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteById(int id)
        {
            var notification = await GetById(id);

            if (notification == null)
                throw new EntityNotFoundException("Notification was not found.");

            var recipients = Repository.GetRecipientsForNotification(id, Identity.Id);

            foreach (var recipient in recipients)
            {
                recipient.Deleted = DateTime.UtcNow;
            }

            await Repository.DbContext.SaveChangesAsync();

            await NotificationHubContext.Clients.All.SendAsync("CurrentStateChanged");

            return true;
        }

        /// <summary>
        /// delete all notifications for Identity by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAllById(int[] ids)
        {
            if (ids.Any())
            {
                foreach (var id in ids)
                {
                    var recipients = Repository.GetRecipientsForNotification(id, Identity.Id);

                    foreach (var recipient in recipients)
                    {
                        recipient.Deleted = DateTime.UtcNow;
                    }
                }

                await Repository.DbContext.SaveChangesAsync();

                await NotificationHubContext.Clients.All.SendAsync("CurrentStateChanged");

                return true;
            }

            return false;
        }

        /// <summary>
        /// mark notification as read
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> MarkAsRead(int id)
        {
            var notification = await GetById(id);

            if (notification == null)
                throw new EntityNotFoundException("Notification was not found.");

            var recipients = Repository.GetRecipientsForNotification(id, Identity.Id);

            foreach (var recipient in recipients)
            {
                recipient.Read = DateTime.UtcNow;
            }

            await Repository.DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// mark notification as unread
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> MarkAsUnread(int id)
        {
            var notification = await GetById(id);

            if (notification == null)
                throw new EntityNotFoundException("Notification was not found.");

            var recipients = Repository.GetRecipientsForNotification(id, Identity.Id);

            foreach (var recipient in recipients)
            {
                recipient.Read = null;
            }

            await Repository.DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// get source from identity
        /// </summary>
        /// <returns></returns>
        async Task<Source> GetSource()
        {
            var client = Identity as ClientIdentity;
            if (client == null)
                return null;

            var source = await Repository.DbContext.Sources.SingleOrDefaultAsync(s => s.Id == client.Id);

            if (source == null)
            {
                source = new Source()
                {
                    Id = client.Id,
                    Name = client.Name
                };

                await Repository.DbContext.Sources.AddAsync(source);
            }
            else
            {
                source.Name = client.Name;
            }

            await Repository.DbContext.SaveChangesAsync();

            return source;
        }

        /// <summary>
        /// get source by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        async Task<Source> GetSource(string id)
        {
            var source = await Repository.DbContext.Sources.SingleOrDefaultAsync(s => s.Id == id);

            if (source == null)
            {
                source = new Source()
                {
                    Id = id,
                    Name = id
                };

                await Repository.DbContext.Sources.AddAsync(source);
                await Repository.DbContext.SaveChangesAsync();
            }

            return source;
        }

        async Task<Target> GetOrAddTargetForProfileIdentity()
        {
            var profile = Identity as ProfileIdentity;
            if (profile == null)
                return null;

            return await GetOrAddTarget(profile.Id, profile.Name);
        }

        async Task<Target> GetOrAddTarget(string id, string name)
        {
            var target = await Repository.DbContext.Targets.SingleOrDefaultAsync(s => s.Id.ToLower() == id.ToLower());

            if (target == null)
            {
                target = new Target
                {
                    Id = id.ToLower(),
                    Name = name
                };

                await Repository.DbContext.Targets.AddAsync(target);
            }
            else
            {
                target.Name = name;
            }

            await Repository.DbContext.SaveChangesAsync();

            return target;
        }

        async Task<List<Target>> GetOrAddTargets(params string[] ids)
        {
            if (ids == null || !ids.Any())
                return new List<Target>();

            var cleanedIds = ids.Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.ToLower())
                .OrderBy(r => r)
                .Distinct().ToArray();

            var targets = await Repository.DbContext.Targets.Where(s => cleanedIds.Contains(s.Id.ToLower())).ToListAsync();

            var foundIds = targets.Select(t => t.Id.ToLower()).ToArray();

            var missing = cleanedIds.Where(id => !foundIds.Contains(id)).ToArray();

            foreach(var id in missing)
            {
                var target = new Target { Id = id, Name = id };

                await Repository.DbContext.Targets.AddAsync(target);
                await Repository.DbContext.SaveChangesAsync();

                targets.Add(target);
            }

            return targets;
        }

        /// <summary>
        /// add notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> Add(NotificationCreate model)
        {
            var notification = new Notification
            {
                Body = model.Body,
                Priority = model.Priority,
                Subject = model.Subject,
                Url = model.Url,
                GlobalId = model.GlobalId,
                Created = DateTime.UtcNow
            };

            foreach (var kv in model.Values)
            {
                notification.Values.Add(new NotificationValue { Key = kv.Key, Value = kv.Value });
            }

            var source = await GetSourceFromIdentity();

            notification.SourceId = source.Id;

            var targets = await GetOrAddTargets(model.Recipients);

            if (!targets.Any())
                throw new EntityNotFoundException("Notification must have at least one recipient");

            foreach (var target in targets)
            {
                notification.Recipients.Add(new Recipient { TargetId = target.Id });
            }

            Repository.DbContext.Notifications.Add(notification);

            await Repository.DbContext.SaveChangesAsync();

            return true;
        }

        async Task<Source> GetSourceFromIdentity()
        {
            Source source = null;
            if (Identity is ClientIdentity)
            {
                source = await GetSource();
            }

            if (Identity is ProfileIdentity)
            {
                var profileIdentity = Identity as ProfileIdentity;
                source = await GetSource(profileIdentity.ClientId);
            }

            return source;
        }

        /// <summary>
        /// delete old notifications for profile
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAllReadByIdentity()
        {
            return await DeleteAllReadByTargetIds(Identity.Id);
        }

        /// <summary>
        /// get unread count for authenticated user
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetUnreadCount()
        {
            return await new NotificationDataFilter { Filter = "unread" }
                .FilterQuery(Repository.GetAll(Identity.Id), Identity).CountAsync();
        }

        /// <summary>
        /// delete old notifications by profile id
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAllReadByTargetIds(params string[] ids)
        {
            var v = ids.Select(i => i.ToLower().Trim()).ToArray();

            var recipients = await Repository.DbContext.Recipients
                .Where(r =>
                    r.Read.HasValue &&
                    v.Contains(r.TargetId.ToLower()) &&
                    r.NotificationId.HasValue)
                .OrderByDescending(r => r.Notification.Created)
                .ToListAsync();

            if (recipients != null && recipients.Any())
            {
                recipients.ForEach(r => r.Deleted = DateTime.UtcNow);
                await Repository.DbContext.SaveChangesAsync();
            }

            return true;
        }

        /// <summary>
        /// delete old notifications by profile id
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAllRead()
        {
            var ids = await Repository.DbContext.Recipients.Select(p => p.TargetId).ToArrayAsync();

            await DeleteAllReadByTargetIds(ids);

            return true;
        }
    }
}
