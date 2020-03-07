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
using Microsoft.Extensions.Configuration;
using Foundry.Portal.Data;
using Foundry.Portal.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Calculators
{
    /// <summary>
    /// calculates rating values for playlists
    /// </summary>
    public class PlaylistRatingCalculator : IPlaylistRatingCalculator
    {
        DbContextOptions _options;

        SketchDbContext DbContext
        {
            get { return new SketchDbContext(_options); }
        }

        public PlaylistRatingCalculator(IConfiguration configuration)
        {
            _options = new DbContextOptionsBuilder().UseConfiguredDatabase(configuration).Options;
        }

        async Task CalculatePlaylistRating(string globalId)
        {
            using (var db = DbContext)
            {
                var ratings = await db.ProfileFollowers
                    .Where(pc => pc.Playlist.GlobalId == globalId && pc.Rating != Rating.Unrated)
                    .OrderBy(pc => pc.Rating)
                    .Select(pc => (int)pc.Rating).ToListAsync();

                var playlist = await db.Playlists.SingleOrDefaultAsync(c => c.GlobalId == globalId);

                var total = ratings.Count();
                var average = ratings.Average();
                var median = (Rating)ratings.Skip(ratings.Count() / 2).First();                

                if (playlist.RatingAverage != average ||
                    playlist.RatingMedian != median ||
                    playlist.RatingTotal != total)
                {
                    playlist.RatingAverage = average;
                    playlist.RatingMedian = median;
                    playlist.RatingTotal = total;

                    await db.SaveChangesAsync();
                }
            }
        }
        
        public async Task<IEnumerable<DomainEventResult>> Process(IDomainEvent e)
        {
            List<DomainEventResult> results = new List<DomainEventResult>();

            if (e.Type == DomainEventType.ContentRate)
            {
                var result = new DomainEventResult
                {
                    Start = DateTime.UtcNow,
                    Event = e
                };

                try
                {
                    await CalculatePlaylistRating(e.Id.ToString().ToLower());

                    result.Finish = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    result.Exception = ex;
                }

                results.Add(result);
            }

            return results;
        }        
    }
}
