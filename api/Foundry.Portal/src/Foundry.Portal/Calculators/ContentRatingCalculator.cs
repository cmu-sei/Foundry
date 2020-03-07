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
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Foundry.Portal.Calculators
{
    /// <summary>
    /// calculates rating values for content
    /// </summary>
    public class ContentRatingCalculator : IContentRatingCalculator
    {
        public DbContextOptions DbContextOptions { get; set; }

        public ContentRatingCalculator(IConfiguration configuration)
        {
            if (configuration != null)
            {
                DbContextOptions = new DbContextOptionsBuilder().UseConfiguredDatabase(configuration).Options;
            }
        }

        public async Task CalculateContentRating(string globalId)
        {
            using (var db = new SketchDbContext(DbContextOptions))
            {
                var content = await db.Contents.SingleOrDefaultAsync(c => c.GlobalId == globalId);

                var ratings = db.ProfileContents
                    .Where(pc => pc.ContentId == content.Id && pc.Rating != Rating.Unrated)
                    .OrderBy(pc => pc.Rating)
                    .Select(pc => pc.Rating);

                int total = ratings.Count();
                double average = 0;
                Rating median = Rating.Unrated;

                if (total > 0)
                {
                    average = ratings.Average(r => (long?)r) ?? 0;
                    median = ratings.Skip(ratings.Count() / 2).FirstOrDefault();
                }

                if (content.RatingAverage != average ||
                    content.RatingMedian != median ||
                    content.RatingTotal != total)
                {
                    content.RatingAverage = average;
                    content.RatingMedian = median;
                    content.RatingTotal = total;

                    await db.SaveChangesAsync();

                    if (content.AuthorId > 0)
                    {
                        await CalculateFor<Profile>(db, content.AuthorId.Value, pc => pc.Content.AuthorId == content.AuthorId);
                    }
                }
            }
        }

        public async Task CalculateFor<TEntity>(SketchDbContext db, int id, Expression<Func<ProfileContent, bool>> predicate)
            where TEntity : class, IRated, IEntityPrimary
        {
            var ratings = await db.ProfileContents
                .Where(predicate)
                .OrderBy(pc => pc.Rating)
                .Select(pc => (int)pc.Rating).ToListAsync();

            var rated = await db.Set<TEntity>().SingleOrDefaultAsync(g => g.Id == id);

            var total = ratings.Count();
            var average = ratings.Average();
            var median = (Rating)ratings.Skip(ratings.Count() / 2).First();

            if (rated.RatingAverage != average ||
                rated.RatingMedian != median ||
                rated.RatingTotal != total)
            {
                rated.RatingTotal = total;
                rated.RatingAverage = average;
                rated.RatingMedian = median;

                await db.SaveChangesAsync();
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
                    await CalculateContentRating(e.Id.ToString().ToLower());

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

