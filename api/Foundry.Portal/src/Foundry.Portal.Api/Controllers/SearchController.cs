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
    /// search controller
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class SearchController : ApiController
    {
        ContentService ContentService { get; }
        PlaylistService PlaylistService { get; }

        /// <summary>
        /// creates and instance of the search controller
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mill"></param>
        /// <param name="contentService"></param>
        /// <param name="playlistService"></param>
        public SearchController(CoreOptions options, ILoggerFactory mill, ContentService contentService, PlaylistService playlistService)
            : base(options, mill)
        {
            ContentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
            PlaylistService = playlistService ?? throw new ArgumentNullException(nameof(playlistService));
        }

        /// <summary>
        /// global search
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/search")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(SearchResult), 200)]
        public async Task<IActionResult> Search([FromQuery]SearchDataFilter search)
        {
            var result = new SearchResult
            {
                Contents = await ContentService.GetAll(new ContentDataFilter() { Term = search.Term, Skip = search.Skip, Take = search.Take }),
                Playlists = await PlaylistService.GetAll(new PlaylistDataFilter() { Term = search.Term, Skip = search.Skip, Take = search.Take })
            };

            return Ok(result);
        }
    }
}

