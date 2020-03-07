/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Buckets.Controllers
{
    /// <summary>
    /// bucket api endpoint
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class BucketController : StackController
    {
        IHostingEnvironment HostingEnvironment { get; }
        IFileUploadMonitor FileUploadMonitor { get; }
        FileService FileService { get; }
        BucketService BucketService { get; }

        /// <summary>
        /// create an instance of bucket controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="monitor"></param>
        /// <param name="host"></param>
        /// <param name="fileService"></param>
        /// <param name="bucketService"></param>
        public BucketController(IStackIdentityResolver identityResolver, IFileUploadMonitor monitor, IHostingEnvironment host, FileService fileService, BucketService bucketService)
            : base(identityResolver)
        {
            HostingEnvironment = host ?? throw new ArgumentNullException(nameof(host));
            FileUploadMonitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            FileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            BucketService = bucketService ?? throw new ArgumentNullException(nameof(bucketService));
        }

        /// <summary>
        /// upload files to the specified bucket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [DisableRequestSizeLimit]
        [Route("api/bucket/{id}/upload")]
        [Authorize]
        public async Task<IActionResult> Upload([FromRoute]int id, ICollection<IFormFile> files)
        {
            var bucket = await BucketService.GetById(id);

            return Ok(await FileService.Upload(bucket, files));
        }

        /// <summary>
        /// get buckets
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/buckets")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(PagedResult<Bucket, BucketSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]BucketDataFilter search = null)
        {
            return Ok(await BucketService.GetAll(search));
        }

        /// <summary>
        /// get bucket by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/bucket/{id}")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(BucketDetail), 200)]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            return Ok(await BucketService.GetById(id));
        }

        /// <summary>
        /// set default bucket
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/bucket/{id}/default")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(BucketDetail), 200)]
        public async Task<IActionResult> SetDefaultBucket([FromRoute]int id)
        {
            return Ok(await BucketService.SetDefaultBucket(id));
        }

        /// <summary>
        /// create a new bucket for identity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/buckets")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(BucketDetail), 200)]
        public async Task<IActionResult> Add([FromBody]BucketCreate model)
        {
            return Ok(await BucketService.Add(model));
        }

        /// <summary>
        /// update bucket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/bucket/{id}")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(BucketDetail), 200)]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody]BucketUpdate model)
        {
            return Ok(await BucketService.Update(model));
        }

        /// <summary>
        /// add account to bucket
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/bucket/{id}/accounts")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(BucketDetail), 200)]
        public async Task<IActionResult> AddAccountToBucket([FromRoute]int id, [FromBody]BucketAccountCreate model)
        {
            return Ok(await BucketService.AddBucketAccount(id, model));
        }
    }
}

