/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stack.DomainEvents
{
    /// <summary>
    /// the core domain event delegator. any future delegators should inherit from this
    /// so that handlers are called in the same manner.
    /// </summary>
    public class DomainEventDelegator : IDomainEventDelegator
    {
        protected DomainEventDispatcherOptions _options;

        public DomainEventDelegator(DomainEventDispatcherOptions options, IEnumerable<IDomainEventHandler> handlers)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            Handlers = new List<IDomainEventHandler>(handlers);
        }

        /// <summary>
        /// the handlers that will be called by the delegator
        /// </summary>
        public IEnumerable<IDomainEventHandler> Handlers { get; private set; }

        /// <summary>
        /// makes a call to each domain event handlers process method
        /// </summary>
        /// <param name="e">the domain event</param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainEventResult>> Delegate(IDomainEvent e)
        {
            var results = new List<DomainEventResult>();

            if (e != null)
            {
                foreach (var handler in Handlers)
                {
                    var handlerResults = await handler.Process(e);
                    results.AddRange(handlerResults);
                }
            }

            return results;
        }
    }
}
