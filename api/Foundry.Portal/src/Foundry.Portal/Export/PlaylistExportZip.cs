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
using Foundry.Portal.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Export
{
    public class PlaylistExportZip : ExportZip, IExport
    {
        SketchDbContext _db;

        public PlaylistExportZip(SketchDbContext db, ContentOptions contentOptions, AutoMapper.IMapper mapper)
            : base(contentOptions, mapper)
        {
            _db = db;
        }

        public override async Task<IEnumerable<ContentExport>> GetAllByIds(int[] ids)
        {
            var collection = new List<ContentExport>();

            var playlists = await _db.Playlists
                .Include(p => p.Sections)
                .Include("Sections.SectionContents")
                .Include("Sections.SectionContents.Content")
                .Include("Sections.SectionContents.Content.ContentTags")
                .Include("Sections.SectionContents.Content.ContentTags.Tag")
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            foreach (var playlist in playlists)
            {
                foreach (var section in playlist.Sections)
                {
                    var contents = section.SectionContents
                        .Select(sc => Mapper.Map<ContentExport>(sc.Content))
                        .ToList();

                    contents.ForEach(ce => {
                        ce.PlaylistName = playlist.Name;
                        ce.PlaylistDescription = playlist.Description;
                        ce.PlaylistGlobalId = playlist.GlobalId;
                        ce.PlaylistLogoUrl = playlist.LogoUrl;
                        ce.PlaylistSummary = playlist.Summary;
                        ce.PlaylistTrailerUrl = playlist.TrailerUrl;
                        ce.SectionName = section.Name;
                        ce.SectionOrder = section.Order;
                        var sectionContent = section.SectionContents.FirstOrDefault(sc => ce.GlobalId == sc.Content.GlobalId);
                        ce.SectionContentOrder = sectionContent == null ? 0 : sectionContent.Order;
                    });

                    collection.AddRange(contents);
                }
            }

            return collection;
        }
    }
}

