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
using Microsoft.AspNetCore.Mvc;
using Foundry.Orders.Data.Entities;
using Foundry.Orders.Data.Repositories;
using Foundry.Orders.ViewModels;
using Foundry.Orders.Options;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Stack.Http.Identity.Attributes;

namespace Foundry.Orders.Controllers
{
    /// <summary>
    /// audience item
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class AudienceItemController : BaseController
    {
        IAudienceItemRepository _audienceItemRepository;

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="mapper"></param>
        /// <param name="audienceItemRepository"></param>
        public AudienceItemController(IStackIdentityResolver identityResolver, IMapper mapper, IAudienceItemRepository audienceItemRepository)
            : base(identityResolver, mapper)
        {
            _audienceItemRepository = audienceItemRepository ?? throw new ArgumentNullException("audienceItemRepository");
        }

        /// <summary>
        /// get all
        /// </summary>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/audienceitems")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        [ProducesResponseType(typeof(PagedResult<AudienceItem, AudienceItemSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]AudienceItemDataFilter dataFilter = null)
        {
            return Ok(await PagedResult<AudienceItem, AudienceItemSummary>(_audienceItemRepository.GetAll(), dataFilter));
        }

        /// <summary>
        /// get all by audience id
        /// </summary>
        /// <param name="audienceId"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/audience/{audienceId}/items")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        [ProducesResponseType(typeof(PagedResult<AudienceItem, AudienceItemSummary>), 200)]
        public async Task<IActionResult> GetAllById([FromRoute] int audienceId, [FromQuery]AudienceItemDataFilter dataFilter = null)
        {
            var query = _audienceItemRepository.GetAll().Where(x => x.AudienceId == audienceId);
            return Ok(await PagedResult<AudienceItem, AudienceItemSummary>(query, dataFilter));
        }
    }
}

