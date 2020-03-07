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
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Portal.Api.Controllers
{
    /// <summary>
    /// creates an instance of the PlaylistController
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class PlaylistController : ApiController
    {
        PlaylistService _playlistService;
        ContentService _contentService;

        /// <summary>
        /// creates an instance of the PlaylistController
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mill"></param>
        /// <param name="playlistService"></param>
        /// <param name="contentService"></param>
        public PlaylistController(CoreOptions options, ILoggerFactory mill, PlaylistService playlistService, ContentService contentService)
            :base(options, mill)
        {
            _playlistService = playlistService ?? throw new ArgumentNullException("playlistService");
            _contentService = contentService ?? throw new ArgumentNullException("contentService");
        }

        /// <summary>
        /// get all playlists by profile id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/profile/{id}/playlists")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Playlist, PlaylistSummary>), 200)]
        public async Task<IActionResult> GetPlaylistsByProfileId([FromRoute] int id, [FromQuery] PlaylistDataFilter search = null)
        {
            return Ok(await _playlistService.GetAllByProfileId(id, search));
        }

        /// <summary>
        /// get all public playlists
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/playlists")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Playlist, PlaylistSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] PlaylistDataFilter search = null)
        {
            return Ok(await _playlistService.GetAll(search));
        }

        /// <summary>
        /// follow a playlist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}/follow")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PlaylistSummary), 200)]
        public async Task<IActionResult> FollowPlaylist(int id)
        {
            return Ok(await _playlistService.Follow(id));
        }

        /// <summary>
        /// unfollow a playlist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}/unfollow")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PlaylistSummary), 200)]
        public async Task<IActionResult> UnfollowPlaylist(int id)
        {
            return Ok(await _playlistService.Unfollow(id));
        }

        /// <summary>
        /// follow a playlist
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}/group/{groupId}/follow")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PlaylistSummary), 200)]
        public async Task<IActionResult> GroupFollowPlaylist(int id, string groupId)
        {
            return Ok(await _playlistService.Follow(id, groupId));
        }

        /// <summary>
        /// unfollow a playlist
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}/group/{groupId}/unfollow")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PlaylistSummary), 200)]
        public async Task<IActionResult> GroupUnfollowPlaylist(int id, string groupId)
        {
            return Ok(await _playlistService.Unfollow(id, groupId));
        }

        /// <summary>
        /// get all content by playlist
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}/contents")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Content, ContentSummary>), 200)]
        public async Task<IActionResult> GetContentsByPlaylistId([FromRoute] int id, [FromQuery] ContentDataFilter search = null)
        {
            return Ok(await _playlistService.GetPlaylistContents(id, search));
        }

        /// <summary>
        /// get playlist for current user
        /// </summary>
        /// <returns></returns>
        [Route("api/profile/me/playlists")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Playlist, PlaylistSummary>), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Playlists([FromQuery] PlaylistDataFilter search = null)
        {
            return Ok(await _playlistService.GetAllByProfileId(_playlistService.Identity.GetId(), search));
        }

        /// <summary>
        /// add playlist to profile
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        [Route("api/playlists")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PlaylistDetail), 201)]
        public async Task<IActionResult> Add([FromBody] PlaylistCreate playlist)
        {
            var model = await _playlistService.Add(playlist);
            return Created("api/playlist/" + model.Id, model);
        }

        /// <summary>
        /// get playlist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PlaylistDetail), 200)]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            return Ok(await _playlistService.GetById(id));
        }

        /// <summary>
        /// get all playlists by group id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/group/{id}/playlists")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Playlist, PlaylistSummary>), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetGroupPlaylists([FromRoute]string id, [FromQuery] PlaylistDataFilter search = null)
        {
            return Ok(await _playlistService.GetAllGroupPlaylists(id, search));
        }

        /// <summary>
        /// update playlist
        /// </summary>
        /// <param name="id"></param>
        /// <param name="playlist"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}")]
        [HttpPut]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PlaylistDetail), 200)]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody] PlaylistUpdate playlist)
        {
            return Ok(await _playlistService.Update(playlist));
        }

        /// <summary>
        /// delete playlist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return Ok(await _playlistService.Delete(id));
        }

        /// <summary>
        /// set playlist as default playlist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}/default")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> SetAsDefault([FromRoute] int id)
        {
            return Ok(await _playlistService.SetAsDefault(id));
        }

        /// <summary>
        /// add content to playlist by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}/content/{contentId}")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> AddContentToPlaylist([FromRoute]int id, [FromRoute] int contentId)
        {
            return Ok(await _playlistService.AddContent(id, contentId));
        }

        /// <summary>
        /// remove content from playlist by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}/content/{contentId}")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> RemoveContentFromPlaylist([FromRoute]int id, [FromRoute] int contentId)
        {
            return Ok(await _playlistService.RemoveContent(id, contentId));
        }

        /// <summary>
        /// add content to playlist
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [Route("api/playlist/content/{contentId}")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> AddToDefaultPlaylist([FromRoute] int contentId)
        {
            return Ok(await _playlistService.AddContentToDefaultPlaylist(contentId));
        }

        /// <summary>
        /// organize playlist content by the order of the sections passed in
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sections"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}/organize")]
        [HttpPut]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Organize([FromRoute] int id, [FromBody] List<PlaylistSectionUpdate> sections)
        {
            return Ok(await _playlistService.Organize(id, sections));
        }

        /// <summary>
        /// get all playlists by rating
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/playlists/top")]
        [HttpGet]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(PagedResult<Playlist, PlaylistSummary>), 200)]
        public async Task<IActionResult> GetAllByRating([FromQuery]PlaylistDataFilter search)
        {
            return Ok(await _playlistService.GetAllByRating(Rating.Good, 10, search));
        }

        /// <summary>
        /// rate playlist
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rating"></param>
        /// <returns></returns>
        [Route("api/playlist/{id}/rate/{rating}")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(RatingMetricDetail), 200)]
        public async Task<IActionResult> SetRating([FromRoute] int id, [FromRoute] Rating rating)
        {
            var model = await _playlistService.SetRating(id, rating);
            return Ok(model.Rating);
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

