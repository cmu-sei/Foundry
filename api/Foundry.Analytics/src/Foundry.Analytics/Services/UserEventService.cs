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
using Foundry.Analytics.Data;
using Foundry.Analytics.Data.Repositories;
using Foundry.Analytics.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Analytics.Services
{
    /// <summary>
    /// user event service
    /// </summary>
    public class UserEventService : Service<IUserEventRepository, UserEvent>
    {
        public UserEventService(IStackIdentityResolver userResolver, IMapper mapper, IUserEventRepository LoginRepository)
            : base(userResolver, mapper, LoginRepository) { }

        /// <summary>
        /// add user event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<UserEventSummary> Add(UserEventCreate model)
        {
            if (string.IsNullOrWhiteSpace(model.Type))
                throw new InvalidModelException("Type is required");

            return Map<UserEventSummary>(await Repository.Add(Map<UserEvent>(model)));
        }

        /// <summary>
        /// get all
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PagedResult<UserEvent, UserEventSummary>> GetAll(UserEventDataFilter filter)
        {
            var query = Repository.GetAll()
                .Where(e => e.CreatedBy == Identity.Id.ToLower() && e.ClientId.ToLower() == Identity.ClientId.ToLower());

            return await PagedResultFactory.Execute<UserEvent, UserEventSummary>(query, filter, Identity);
        }
    }
}
