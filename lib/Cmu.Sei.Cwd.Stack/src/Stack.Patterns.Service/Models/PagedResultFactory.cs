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
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stack.Patterns.Service.Models
{
    public class PagedResultFactory
    {
        public IMapper Mapper { get; }
        public PagedResultFactory(IMapper mapper)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedResult<TEntityModel, TViewModel>> Execute<TEntityModel, TViewModel>(
            IQueryable<TEntityModel> query,
            IDataFilter<TEntityModel> dataFilter,
            IStackIdentity identity)
            where TEntityModel : class
            where TViewModel : class
        {
            var result = new PagedResult<TEntityModel, TViewModel>
            {
                DataFilter = dataFilter
            };

            if (dataFilter != null)
            {
                query = dataFilter.SearchQuery(query);
                query = dataFilter.FilterQuery(query, identity);
            }

            result.Total = query.Count();

            if (dataFilter != null)
            {
                query = dataFilter.SortQuery(query);

                if (dataFilter.Skip > 0)
                {
                    query = query.Skip(dataFilter.Skip);
                }

                if (dataFilter.Take > 0)
                {
                    query = query.Take(dataFilter.Take);
                }
            }

            var entities = await query.ToListAsync();

            var mappingOperationOptions = identity.GetMappingOperations();

            if (mappingOperationOptions == null)
            {
                result.Results = entities.Select(Mapper.Map<TEntityModel, TViewModel>).ToArray();
            }
            else
            {
                result.Results = entities.Select(e => Mapper.Map<TEntityModel, TViewModel>(e, mappingOperationOptions)).ToArray();
            }

            return result;
        }
    }
}
