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
using Stack.Http.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Calculators
{
    /// <summary>
    /// calculates rating values for content
    /// </summary>
    public class ContentDifficultyCalculator : IContentDifficultyCalculator
    {
        readonly DbContextOptions _options;

        public ContentDifficultyCalculator(IConfiguration configuration)
        {
            _options = new DbContextOptionsBuilder().UseConfiguredDatabase(configuration).Options;
        }

        async Task CalculateContentDifficulty(string globalId)
        {
            using (var db = new SketchDbContext(_options))
            {
                var content = await db.Contents.SingleOrDefaultAsync(c => c.GlobalId == globalId);

                if (content == null)
                    throw new EntityNotFoundException(string.Format("Content '{0} was not found.", globalId));

                var query = db.ProfileContents
                    .Where(pc => pc.ContentId == content.Id && pc.Difficulty != Difficulty.Unrated)
                    .OrderBy(pc => pc.Difficulty);

                int total = query.Count();
                double average = 0;
                Difficulty median = Difficulty.Unrated;

                if (total > 0)
                {
                    average = query.Average(pc => (long?)pc.Difficulty) ?? 0;
                    median = query.Skip(query.Count() / 2).FirstOrDefault().Difficulty;
                }

                if (content.DifficultyAverage != average ||
                    content.DifficultyMedian != median ||
                    content.DifficultyTotal != total)
                {
                    content.DifficultyAverage = average;
                    content.DifficultyMedian = median;
                    content.DifficultyTotal = total;

                    await db.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// process the domain event to determine difficulty
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DomainEventResult>> Process(IDomainEvent e)
        {
            List<DomainEventResult> results = new List<DomainEventResult>();

            if (e.Type == DomainEventType.ContentLevel)
            {
                var result = new DomainEventResult
                {
                    Start = DateTime.UtcNow,
                    Event = e
                };

                try
                {
                    await CalculateContentDifficulty(e.Id.ToString().ToLower());

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
