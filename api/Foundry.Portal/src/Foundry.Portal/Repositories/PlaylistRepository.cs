/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Security;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Foundry.Portal.Repositories
{
    public class PlaylistRepository : Repository<Playlist>, IPlaylistRepository
    {
        PlaylistPermissionMediator _playlistPermissionMediator;

        public PlaylistRepository(SketchDbContext dbContext, PlaylistPermissionMediator playlistPermissionMediator)
            : base(dbContext)
        {
            _playlistPermissionMediator = playlistPermissionMediator;
        }

        IQueryable<Playlist> GetAllQuery(Expression<Func<Playlist, bool>> expression)
        {
            return GetAllQuery().Where(expression);
        }

        IQueryable<Playlist> GetAllQuery()
        {
            var query = DbContext.Playlists
                .Include(p => p.Sections)
                .Include("Sections.SectionContents")
                .Include("Sections.SectionContents.Content")
                .Include(c => c.PlaylistTags)
                .Include("PlaylistTags.Tag")
                .Include(p => p.ProfileFollowers)
                .Include(p => p.PlaylistGroups);

            return _playlistPermissionMediator.Process(query);
        }

        async Task<Playlist> GetQuery(Expression<Func<Playlist, bool>> expression)
        {
            var query = DbContext.Playlists
                .Include(p => p.Sections)
                .Include("Sections.SectionContents")
                .Include("Sections.SectionContents.Content")
                .Include(c => c.PlaylistTags)
                .Include("PlaylistTags.Tag")
                .Include(p => p.PlaylistGroups)
                .Include(p => p.ProfileFollowers)
                    .ThenInclude(pf => pf.Profile)
                .Where(expression);

            query = _playlistPermissionMediator.Process(query);

            return await query.SingleOrDefaultAsync();
        }

        public override IQueryable<Playlist> GetAll()
        {
            return GetAllQuery();
        }

        public IQueryable<Playlist> GetAllByProfileId(int profileId)
        {
            return GetAllQuery(o => o.ProfileFollowers.Any(f => f.ProfileId == profileId));
        }

        public IQueryable<Playlist> GetAllByGroupId(string groupId)
        {
            return GetAllQuery(o => o.PlaylistGroups.Any(f => f.GroupId == groupId));
        }

        public async override Task<Playlist> GetById(int id)
        {
            return await GetQuery(c => c.Id == id);
        }

        public IQueryable<Section> GetPlaylistSections(int id)
        {
            var query = DbContext.Sections
                .Include(pc => pc.SectionContents)
                .Include("SectionContents.Content")
                .Where(pc => pc.PlaylistId == id);

            query = _playlistPermissionMediator.Process(query);

            return query;
        }

        public async Task<Playlist> GetDefaultPlaylist(int profileId)
        {
            var playlists = GetAllByProfileId(profileId);

            var playlist = playlists.FirstOrDefault(b => b.IsDefault);
            if (playlist == null)
            {
                if (playlists.Any())
                {
                    playlist = playlists.First();
                    playlist.IsDefault = true;
                }
                else
                {
                    playlist = new Playlist { Name = "My Playlist", IsDefault = true, ProfileId = profileId };
                    DbContext.Playlists.Add(playlist);
                }

                await DbContext.SaveChangesAsync();
            }

            return playlist;
        }

        public async Task<Playlist> GetByGlobalId(string globalId)
        {
            return await GetQuery(p => p.GlobalId.ToLower() == globalId.ToLower());
        }

        public async Task<Playlist> GetByName(string name)
        {
            return await GetQuery(p => p.Name.ToLower() == name.ToLower());
        }
    }
}

