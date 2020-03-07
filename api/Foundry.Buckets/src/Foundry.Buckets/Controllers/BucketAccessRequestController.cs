/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Foundry.Buckets.Attributes;
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.Monitors;
using Foundry.Buckets.Services;
using Foundry.Buckets.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System;
using System.Threading.Tasks;

namespace Foundry.Buckets.Controllers
{
    /// <summary>
    /// bucket access request api endpoint
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class BucketAccessRequestController : StackController
    {
        IHostingEnvironment HostingEnvironment { get; }
        IFileUploadMonitor FileUploadMonitor { get; }
        BucketAccessRequestService BucketAccessRequestService { get; }
        BucketService BucketService { get; }

        /// <summary>
        /// create an instance of bucket controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="monitor"></param>
        /// <param name="host"></param>
        /// <param name="bucketAccessRequestService"></param>
        /// <param name="bucketService"></param>
        public BucketAccessRequestController(IStackIdentityResolver identityResolver, IFileUploadMonitor monitor, IHostingEnvironment host, BucketAccessRequestService bucketAccessRequestService, BucketService bucketService)
            : base(identityResolver)
        {
            HostingEnvironment = host ?? throw new ArgumentNullException(nameof(host));
            FileUploadMonitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            BucketAccessRequestService = bucketAccessRequestService ?? throw new ArgumentNullException(nameof(bucketAccessRequestService));
            BucketService = bucketService ?? throw new ArgumentNullException(nameof(bucketService));
        }

        /// <summary>
        /// get all bucket access requests
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/requests")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(PagedResult<BucketAccessRequest, BucketAccessRequestSummary>), 200)]
        public async Task<IActionResult> GetAllRequests([FromQuery]BucketAccessRequestDataFilter search = null)
        {
            return Ok(await BucketAccessRequestService.GetAllRequests(search));
        }

        /// <summary>
        /// get all bucket invites
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/invites")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(PagedResult<BucketAccessRequest, BucketAccessRequestSummary>), 200)]
        public async Task<IActionResult> GetAllInvites([FromQuery]BucketAccessRequestDataFilter search = null)
        {
            return Ok(await BucketAccessRequestService.GetAllInvites(search));
        }

        /// <summary>
        /// accept bucket invite
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/invite/{id}")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(BucketAccessRequestDetail), 200)]
        public async Task<IActionResult> Accept([FromRoute]int id)
        {
            return Ok(await BucketAccessRequestService.Accept(id));
        }

        /// <summary>
        /// decline bucket invite
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("api/invite/{id}")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(BucketAccessRequestDetail), 200)]
        public async Task<IActionResult> Decline([FromRoute]int id)
        {
            return Ok(await BucketAccessRequestService.Decline(id));
        }

        /// <summary>
        /// approve access request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/request/{id}")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(BucketAccessRequestDetail), 200)]
        public async Task<IActionResult> Approve([FromRoute]int id)
        {
            return Ok(await BucketAccessRequestService.Approve(id));
        }

        /// <summary>
        /// deny request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("api/request/{id}")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(BucketAccessRequestDetail), 200)]
        public async Task<IActionResult> Deny([FromRoute]int id)
        {
            return Ok(await BucketAccessRequestService.Deny(id));
        }

        /// <summary>
        /// create a bucket access request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/requests")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(BucketAccessRequestDetail), 200)]
        public async Task<IActionResult> Add([FromBody]BucketAccessRequestCreate model)
        {
            return Ok(await BucketAccessRequestService.Request(model));
        }

        /// <summary>
        /// create a bucket access request to bucket for identity
        /// </summary>
        /// <param name="bucketId"></param>
        /// <returns></returns>
        [HttpPost("api/bucket/{bucketId}/requests")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(BucketAccessRequestDetail), 200)]
        public async Task<IActionResult> AddBucketRequestByIdentity(int bucketId)
        {
            var model = new BucketAccessRequestCreate
            {
                BucketId = bucketId,
                AccountId = Identity.Id.ToLower()
            };

            return Ok(await BucketAccessRequestService.Request(model));
        }
    }
}

