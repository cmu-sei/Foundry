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
using Microsoft.Extensions.Logging;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity.Attributes;
using System;
using System.Threading.Tasks;

namespace Foundry.Portal.Api.Controllers
{
    /// <summary>
    /// export api endpoints
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class ExportController : ApiController
    {
        ContentService _contentService;
        PlaylistService _playlistService;

        /// <summary>
        /// creates an instance of the ExportController
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mill"></param>
        /// <param name="contentService"></param>
        /// <param name="playlistService"></param>
        public ExportController(CoreOptions options, ILoggerFactory mill, ContentService contentService, PlaylistService playlistService)
            : base(options, mill)
        {
            _contentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
            _playlistService = playlistService ?? throw new ArgumentNullException(nameof(playlistService));
        }

        /// <summary>
        /// export content
        /// </summary>
        /// <param name="export"></param>
        /// <returns></returns>
        [Route("api/contents/export")]
        [HttpPost]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> ExportContent([FromBody]ExportSettings export)
        {
            var result = await _contentService.Export(export);

            byte[] bytes = result.Bytes;

            var fileName = string.IsNullOrWhiteSpace(export.FileName)
                ? string.Format("export-{0}", DateTime.UtcNow.ToString("YYYY-MM-DD"))
                : export.FileName;

            var contentType = export.Type == ExportType.Csv
                ? "application/octet-stream"
                : "application/zip";

            return File(bytes, contentType, fileName + "." + export.Extension);
        }


        /// <summary>
        /// export playlists
        /// </summary>
        /// <param name="export"></param>
        /// <returns></returns>
        [Route("api/playlists/export")]
        [HttpPost]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> ExportPlaylists([FromBody]ExportSettings export)
        {
            var result = await _playlistService.Export(export);

            byte[] bytes = result.Bytes;

            var fileName = string.IsNullOrWhiteSpace(export.FileName)
                ? string.Format("export-{0}", DateTime.UtcNow.ToString("YYYY-MM-DD"))
                : export.FileName;

            var contentType = export.Type == ExportType.Csv
                ? "application/octet-stream"
                : "application/zip";

            return File(bytes, contentType, fileName + "." + export.Extension);
        }

        /// <summary>
        /// error
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

