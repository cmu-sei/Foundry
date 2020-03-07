/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Portal.Cache;
using Foundry.Portal.Calculators;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Events;
using Foundry.Portal.Notifications;
using Foundry.Portal.ViewModels;
using Foundry.Portal.WebHooks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Portal.TestBed
{
    public class TestWebHookHandler : IWebHookHandler
    {
        public async Task<IEnumerable<DomainEventResult>> Process(IDomainEvent e)
        {
            return await Task.FromResult(new List<DomainEventResult>());
        }
    }

    public class TestProfileCache : IProfileCache
    {
        public Profile Get(string globalId)
        {
            return null;
        }

        public void Remove(string globalId)
        {
            return;
        }

        public Profile Set(Profile profile)
        {
            return profile;
        }

        public Profile Set(string key, Profile entity)
        {
            return entity;
        }
    }

    public class TestNotificationHandler : INotificationHandler
    {
        public async Task<bool> PostNotificationAsync(NotificationCreate notification)
        {
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<DomainEventResult>> Process(IDomainEvent e)
        {
            return await Task.FromResult(new List<DomainEventResult>());
        }
    }

    public class TestContentRatingCalculator : IContentRatingCalculator
    {
        public async Task<IEnumerable<DomainEventResult>> Process(IDomainEvent e)
        {
            return await Task.FromResult(new List<DomainEventResult>());
        }
    }

    public class TestContentDifficultyCalculator : IContentDifficultyCalculator
    {
        public async Task<IEnumerable<DomainEventResult>> Process(IDomainEvent e)
        {
            return await Task.FromResult(new List<DomainEventResult>());
        }
    }

    public class TestPlaylistRatingCalculator : IPlaylistRatingCalculator
    {
        public async Task<IEnumerable<DomainEventResult>> Process(IDomainEvent e)
        {
            return await Task.FromResult(new List<DomainEventResult>());
        }
    }
}
