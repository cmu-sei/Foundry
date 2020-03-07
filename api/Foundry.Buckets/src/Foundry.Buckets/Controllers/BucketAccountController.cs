/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Mvc;
using Foundry.Buckets.Attributes;
using Foundry.Buckets.Data.Entities;
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
    /// bucket account api endpoints
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class BucketAccountController : StackController
    {
        BucketAccountService BucketAccountService { get; }

        /// <summary>
        /// create an instance of bucket account controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="bucketAccountService"></param>
        public BucketAccountController(IStackIdentityResolver identityResolver, BucketAccountService bucketAccountService)
            : base(identityResolver)
        {
            bucketAccountService = bucketAccountService ?? throw new ArgumentNullException(nameof(bucketAccountService));
        }

        /// <summary>
        /// get all sources by bucket id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/bucket/{id}/accounts")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(PagedResult<BucketAccount, BucketAccountSummary>), 200)]
        public async Task<IActionResult> GetAllByBucketId([FromRoute]int id, [FromQuery]BucketAccountDataFilter search = null)
        {
            return Ok(await BucketAccountService.GetAll(id, search));
        }

        /// <summary>
        /// remove account from bucket
        /// </summary>
        /// <param name="bucketId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpDelete("api/bucket/{bucketId}/account/{accountId}")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Delete([FromRoute]int bucketId, [FromRoute]string accountId)
        {
            return Ok(await BucketAccountService.Delete(bucketId, accountId));
        }
    }
}

