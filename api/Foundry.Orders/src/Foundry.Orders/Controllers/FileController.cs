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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stack.Http.Identity.Attributes;
using Foundry.Orders.Data.Entities;
using Foundry.Orders.Data.Repositories;
using Foundry.Orders.Services;
using Foundry.Orders.ViewModels;
using Foundry.Orders.Options;
using Stack.Http.Identity;
using Stack.Http.Options;
using Stack.Patterns.Service.Models;
using System;
using System.Threading.Tasks;

namespace Foundry.Orders.Controllers
{
    /// <summary>
    /// files
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class FileController : BaseController
    {
        FileOptions FileOptions { get; }
        BrandingOptions DisplayOptions { get; }
        IFileRepository FileRepository { get; }
        FileService FileService { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="mapper"></param>
        /// <param name="fileRepository"></param>
        /// <param name="fileService"></param>
        public FileController(
            IStackIdentityResolver identityResolver,
            IMapper mapper,
            IFileRepository fileRepository,
            FileService fileService)
            : base(identityResolver, mapper)
        {
            FileRepository = fileRepository ?? throw new ArgumentNullException("fileRepository");
            FileService = fileService;
        }

        /// <summary>
        /// get all
        /// </summary>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/files")]
        [ProducesResponseType(typeof(PagedResult<File, FileSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]FileDataFilter dataFilter = null)
        {
            var result = await PagedResult<File, FileSummary>(FileRepository.GetAll(), dataFilter);
            return Ok(result);
        }

        /// <summary>
        /// get all by order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/order/{orderId}/files")]
        [ProducesResponseType(typeof(PagedResult<File, FileSummary>), 200)]
        public async Task<IActionResult> GetAllByFileId([FromRoute]int orderId, [FromQuery]FileDataFilter dataFilter = null)
        {
            var query = FileRepository.GetAllByFileId(orderId);
            var result = await PagedResult<File, FileSummary>(query, dataFilter);
            return Ok(result);
        }

        /// <summary>
        /// dummy endpoint
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/file")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Dummy()
        {
            return Ok();
        }

        /// <summary>
        /// upload file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/file/upload/{id}")]
        [ApiExplorerSettings(IgnoreApi=true)]
        [ProducesResponseType(typeof(bool), 200)]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Upload([FromRoute]int id, [FromBody] IFormFile file)
        {
            FileCreate model = new FileCreate();

            if (file.Length > 0)
            {
                if (await FileService.Save(file, id))
                {
                    var entity = new File
                    {
                        OrderId = id,
                        Name = file.FileName
                    };

                    //await _fileRepository.Add(entity);

                    return Json(true);
                }
            }

            throw new Exception("File upload failed");
        }
    }
}

