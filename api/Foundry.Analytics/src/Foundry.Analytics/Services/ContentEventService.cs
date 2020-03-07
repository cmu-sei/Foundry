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
    /// content event service
    /// </summary>
    public class ContentEventService : Service<IContentEventRepository, ContentEvent>
    {
        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="userResolver"></param>
        /// <param name="mapper"></param>
        /// <param name="ContentLaunchRepository"></param>
        public ContentEventService(IStackIdentityResolver userResolver, IMapper mapper, IContentEventRepository ContentLaunchRepository)
            : base(userResolver, mapper, ContentLaunchRepository) { }

        /// <summary>
        /// add content event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ContentEventSummary> Add(ContentEventCreate model)
        {
            var contentId = model.ContentId.ToLower().Trim();
            var type = model.Type.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(model.ContentId))
                throw new InvalidModelException("Content Id is required");

            if (string.IsNullOrWhiteSpace(model.ContentName))
                throw new InvalidModelException("Content Name is required");

            if (string.IsNullOrWhiteSpace(model.ContentSlug))
                throw new InvalidModelException("Content Slug is required");

            if (string.IsNullOrWhiteSpace(model.Type))
                throw new InvalidModelException("Type is required");

            return Map<ContentEventSummary>(await Repository.Add(Map<ContentEvent>(model)));
        }

        /// <summary>
        /// get all content events
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<PagedResult<ContentEvent, ContentEventSummary>> GetAll(ContentEventDataFilter filter)
        {
            var query = Repository.GetAll()
                .Where(e => e.CreatedBy == Identity.Id.ToLower() && e.ClientId.ToLower() == Identity.ClientId.ToLower());

            return await PagedResultFactory.Execute<ContentEvent, ContentEventSummary>(query, filter, Identity);
        }
    }
}
