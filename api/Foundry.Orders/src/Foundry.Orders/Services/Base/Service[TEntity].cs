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
using Microsoft.EntityFrameworkCore;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Orders.Services
{
    public class Service<TEntity>
        where TEntity : class
    {
        IStackIdentityResolver IdentityResolver { get; }
        PagedResultFactory PagedResultFactory { get; }
        public IMapper Mapper { get; }

        IStackIdentity _identity;

        public Service(IStackIdentityResolver identityResolver, IMapper mapper)
        {
            IdentityResolver = identityResolver ?? throw new ArgumentNullException("identityResolver");
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            PagedResultFactory = new PagedResultFactory(mapper);
        }

        public IStackIdentity Identity
        {
            get
            {
                if (_identity == null)
                {
                    try { _identity = IdentityResolver.GetIdentityAsync().Result; } catch { }
                }

                return _identity;
            }
        }

        public bool IsAdministrator
        {
            get
            {
                if (Identity == null)
                    return false;

                return Identity.Permissions.Contains("administrator");
            }
        }

        public async Task<PagedResult<TEntityModel, TViewModel>> PagedResult<TEntityModel, TViewModel>(
            IQueryable<TEntityModel> query,
            IDataFilter<TEntityModel> dataFilter)
            where TEntityModel : class
            where TViewModel : class
        {
            var asNoTrackingQuery = query.AsNoTracking();

            return await PagedResultFactory.Execute<TEntityModel, TViewModel>(asNoTrackingQuery, dataFilter, Identity);
        }
    }
}

