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
using Stack.Http.Attributes;
using Stack.Patterns.Service.Models;
using System;
using System.Threading.Tasks;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;

namespace Foundry.Orders.Controllers
{
    /// <summary>
    /// assessment type controller
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class AssessmentTypeController : BaseController
    {
        IAssessmentTypeRepository _assessmentTypeRepository;

        /// <summary>
        /// create an instance of assessment type controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="mapper"></param>
        /// <param name="assessmentTypeRepository"></param>
        public AssessmentTypeController(IStackIdentityResolver identityResolver, IMapper mapper, IAssessmentTypeRepository assessmentTypeRepository)
            : base(identityResolver, mapper)
        {
            _assessmentTypeRepository = assessmentTypeRepository ?? throw new ArgumentNullException("assessmentTypeRepository");
        }

        /// <summary>
        /// get all assessment types
        /// </summary>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/assessmenttypes")]
        [ProducesResponseType(typeof(PagedResult<AssessmentType, AssessmentTypeSummary>), 200)]
        [JsonExceptionFilter]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 1800)]
        public async Task<IActionResult> GetAll([FromQuery]AssessmentTypeDataFilter dataFilter = null)
        {
            return Ok(await PagedResult<AssessmentType, AssessmentTypeSummary>(_assessmentTypeRepository.GetAll(), dataFilter));
        }
    }
}

