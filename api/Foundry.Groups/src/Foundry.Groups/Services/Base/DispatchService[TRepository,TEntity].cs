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
using Stack.DomainEvents;
using Foundry.Groups.Data;
using Stack.Http.Identity;
using Stack.Patterns.Repository;
using Stack.Validation.Handlers;
using System;
using System.Threading.Tasks;

namespace Foundry.Groups.Services
{
    public abstract class DispatchService<TRepository, TEntity> : Service<TRepository, TEntity>
        where TRepository : class, IRepository<GroupsDbContext, TEntity>
        where TEntity : class, new()
    {
        public IDomainEventDispatcher DomainEventDispatcher { get; }
        ILogger<DispatchService<TRepository, TEntity>> Logger { get; }

        public DispatchService(
            IDomainEventDispatcher domainEventDispatcher,
            IStackIdentityResolver identityResolver,
            TRepository repository,
            IMapper mapper,
            IValidationHandler validationHandler,
            ILogger<DispatchService<TRepository, TEntity>> logger)
                : base(identityResolver, repository, mapper, validationHandler)
        {
            DomainEventDispatcher = domainEventDispatcher ?? throw new ArgumentNullException(nameof(domainEventDispatcher));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected async Task DispatchAsync(DomainEvent @event)
        {
            var results = await DomainEventDispatcher.DispatchAsync(@event);

            foreach (var result in results)
            {
                if (result.Exception == null)
                {
                    Logger.LogInformation("DomainEventDispatcher", result);
                }
                else
                {
                    Logger.LogError(result.Exception, "DomainEventDispatcher", result);
                }
            }
        }
    }
}

