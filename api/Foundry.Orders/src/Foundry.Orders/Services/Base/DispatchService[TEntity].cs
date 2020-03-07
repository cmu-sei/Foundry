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
using Stack.DomainEvents;
using Stack.Http.Identity;
using System;
using System.Threading.Tasks;

namespace Foundry.Orders.Services
{
    public abstract class DispatchService<TEntity> : Service<TEntity>
        where TEntity : class, new()
    {
        public IDomainEventDispatcher DomainEventDispatcher { get; }

        public DispatchService(
            IDomainEventDispatcher domainEventDispatcher,
            IStackIdentityResolver identityResolver,
            IMapper mapper)
                : base(identityResolver, mapper)
        {
            DomainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
        }

        protected void Dispatch(DomainEvent @event)
        {
            var _ = Task.Run(() => {
                DomainEventDispatcher.Dispatch(@event);
            });
        }

        protected async Task DispatchAsync(DomainEvent @event)
        {
            await DomainEventDispatcher.DispatchAsync(@event);
        }
    }
}

