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
using Microsoft.EntityFrameworkCore;
using Foundry.Analytics.Data;
using Foundry.Analytics.Data.Repositories;
using Foundry.Analytics.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Analytics.Services
{
    /// <summary>
    /// client event service
    /// </summary>
    public class ClientEventService : Service<IClientEventRepository, ClientEvent>
    {
        public ClientEventService(IStackIdentityResolver userResolver, IMapper mapper, IClientEventRepository pageViewRepository)
            : base(userResolver, mapper, pageViewRepository) { }

        /// <summary>
        /// add content event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ClientEventSummary> Add(ClientEventCreate model)
        {
            if (string.IsNullOrWhiteSpace(model.Url))
                throw new InvalidModelException("Url is null");

            var entity = Map<ClientEvent>(model);

            var saved = await Repository.Add(entity);

            return Map<ClientEventSummary>(saved);
        }

        /// <summary>
        /// get all content events by url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<PagedResult<ClientEvent, ClientEventSummary>> GetAll(ClientEventDataFilter filter)
        {
            var query = Repository.GetAll()
                .Where(e => e.CreatedBy == Identity.Id.ToLower() && e.ClientId.ToLower() == Identity.ClientId.ToLower());

            return await PagedResultFactory.Execute<ClientEvent, ClientEventSummary>(query, filter, Identity);
        }

        /// <summary>
        /// get page view metric by url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<PageViewMetric> GetPageViewMetric(string url)
        {
            var metric = new PageViewMetric { Url = url };

            var total = await Repository.GetAllByUrl(url).CountAsync();
            var totalUnique = await Repository.GetAllByUrl(url).GroupBy(pv => pv.CreatedBy).CountAsync();
            var lastUrls = Repository.GetAllByUrl(url)
                .GroupBy(x => x.LastUrl)
                .Select(x => new { LastUrl = x.Key, Total = x.Count() })
                .OrderByDescending(x => x.Total)
                .Take(20);

            metric.Total = total;
            metric.TotalUnique = totalUnique;

            metric.History = new List<PageViewMetricHistory>();

            foreach (var lastUrl in lastUrls)
            {
                metric.History.Add(new PageViewMetricHistory()
                {
                    Url = lastUrl.LastUrl,
                    Total = lastUrl.Total
                });
            }

            return metric;
        }
    }
}
