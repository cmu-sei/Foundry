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
using Microsoft.Extensions.Logging;
using Foundry.Portal.Events;
using Stack.Http.Identity;
using System;
using System.Threading.Tasks;

namespace Foundry.Portal.Services
{
    /// <summary>
    /// Dispatch Service provides functionality to queue/process domain events
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class DispatchService<TEntity> : Service<TEntity>
        where TEntity : class, new()
    {
        public IDomainEventDispatcher DomainEventDispatcher { get; }

        public DispatchService(IDomainEventDispatcher domainEventDispatcher, CoreOptions options, IStackIdentityResolver identityResolver, ILoggerFactory loggerFactory, IMapper mapper)
            : base(options, identityResolver, loggerFactory, mapper)
        {
            DomainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException("domainEventDispatcher");
        }

        protected void Dispatch(DomainEvent @event)
        {
            var _ = Task.Run(() => DomainEventDispatcher.Dispatch(@event));
        }
    }
}
