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
    /// buckets file endpoints
    /// </summary>
    [SecurityHeaders]
    public class FileController : StackController
    {
        IHostingEnvironment HostingEnvironment { get; }
        IFileUploadMonitor FileUploadMonitor { get; }
        FileService FileService { get; }
        BucketService BucketService { get; }

        /// <summary>
        /// create a new instance of file controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="monitor"></param>
        /// <param name="fileService"></param>
        /// <param name="bucketService"></param>
        public FileController(IStackIdentityResolver identityResolver, IFileUploadMonitor monitor, FileService fileService, BucketService bucketService)
            : base(identityResolver)
        {
            FileUploadMonitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            FileService = fileService ?? throw new ArgumentNullException(nameof(FileService));
            BucketService = bucketService ?? throw new ArgumentNullException(nameof(bucketService));
        }

        /// <summary>
        /// upload files to identity default bucket
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [DisableRequestSizeLimit]
        [Route("api/upload")]
        [StackAuthorize]
        public async Task<IActionResult> Upload(ICollection<IFormFile> files)
        {
            var bucket = await BucketService.GetBucketForRequest();
            var fileStorageResults = await FileService.Upload(bucket, files);
            return Ok(fileStorageResults);
        }

        /// <summary>
        /// download file by global id
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 31536000)]
        [HttpGet("f/{globalId}")]
        public async Task<IActionResult> Download([FromRoute]string globalId)
        {
            if (!(await FileService.Exists(globalId)))
                return NotFound(string.Format("File '{0}' was not found", globalId));

            return await FileService.GetFileResult(globalId);
        }

        /// <summary>
        /// download file by global id with extension
        /// </summary>
        /// <param name="globalId"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 31536000)]
        [HttpGet("f/{globalId}.{extension}")]
        public async Task<IActionResult> DownloadWithExtension([FromRoute]string globalId, string extension)
        {
            if (!(await FileService.Exists(globalId)))
                return NotFound(string.Format("File '{0}.{1}' was not found", globalId, extension));

            return await FileService.GetFileResult(globalId);
        }

        /// <summary>
        /// get all files for identity
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/files")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(PagedResult<File, FileSummary>), 200)]
        [StackAuthorize]
        public async Task<IActionResult> GetAll([FromQuery]FileDataFilter search = null)
        {
            return Ok(await FileService.GetAll(search ?? new FileDataFilter()));
        }

        /// <summary>
        /// get unmapped physical files
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/import/files")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(PagedResult<ImportFileSummary, ImportFileSummary>), 200)]
        [StackAuthorize]
        public async Task<IActionResult> GetAllForImport([FromQuery]ImportFileDataFilter search = null)
        {
            return Ok(await FileService.GetAllForImport(search ?? new ImportFileDataFilter()));
        }

        /// <summary>
        /// import files
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("api/import/files")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(List<ImportFileResult>), 200)]
        [StackAuthorize]
        public async Task<IActionResult> ImportFiles([FromBody] List<ImportFileUpdate> files)
        {
            var results = await FileService.Import(files);

            return Ok(results);
        }

        /// <summary>
        /// get random file by data filter
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/file/random")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(FileDetail), 200)]
        [StackAuthorize]
        public async Task<IActionResult> GetRandomFile([FromQuery]FileDataFilter search = null)
        {
            return Ok(await FileService.GetRandomFile(search));
        }


        /// <summary>
        /// get file by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/file/{id:int}")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(FileDetail), 200)]
        [StackAuthorize]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            return Ok(await FileService.GetById(id));
        }

        /// <summary>
        /// delete a file by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("api/file/{id}")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(FileStorageResult), 200)]
        [StackAuthorize]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            return Ok(await FileService.Delete(id));
        }

        /// <summary>
        /// update file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/file/{id}")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(FileDetail), 200)]
        [StackAuthorize]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody]FileUpdate model)
        {
            return Ok(await FileService.Update(model));
        }

        /// <summary>
        /// set tags for file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        [HttpPost("api/file/{id}/tags")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(string[]), 200)]
        [StackAuthorize]
        public async Task<IActionResult> SetTags([FromRoute]int id, [FromBody]string[] tags)
        {
            return Ok(await FileService.SetTags(id, tags));
        }
    }
}

