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
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using Foundry.Orders.Data.Entities;
using Foundry.Orders.Data.Repositories;
using Foundry.Orders.Options;
using Foundry.Orders.ViewModels;
using Stack.Patterns.Service.Models;
using System;
using System.Threading.Tasks;

namespace Foundry.Orders.Controllers
{
    /// <summary>
    /// audience controller
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class AudienceController : BaseController
    {
        IAudienceRepository _audienceRepository;

        /// <summary>
        /// create an instance of audience controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="mapper"></param>
        /// <param name="audienceRepository"></param>
        public AudienceController(IStackIdentityResolver identityResolver, IMapper mapper, IAudienceRepository audienceRepository)
            : base(identityResolver, mapper)
        {
            _audienceRepository = audienceRepository ?? throw new ArgumentNullException("audienceRepository");
        }

        /// <summary>
        /// get all audiences
        /// </summary>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/audiences")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        [ProducesResponseType(typeof(PagedResult<Audience, AudienceSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]AudienceDataFilter dataFilter = null)
        {
            return Ok(await PagedResult<Audience, AudienceSummary>(_audienceRepository.GetAll(), dataFilter));}
    }
}

