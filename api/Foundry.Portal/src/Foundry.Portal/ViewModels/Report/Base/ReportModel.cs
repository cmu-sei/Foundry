/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using Foundry.Portal.Data;
using Foundry.Portal.Reports;
using Stack.Http.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Portal.ViewModels
{
    public abstract class ReportModel<TModel> : IReportModel
        where TModel: class
    {
        protected SketchDbContext DbContext { get; }
        protected AuthorizationOptions AuthorizationOptions { get; }
        protected AnalyticsOptions AnalyticsOptions { get; }

        public virtual bool IsEnabled { get { return true; } }

        public ReportModel(SketchDbContext dbContext, AuthorizationOptions authorizationOptions, AnalyticsOptions analyticsOptions)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            AuthorizationOptions = authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions));
            AnalyticsOptions = analyticsOptions ?? throw new ArgumentNullException(nameof(analyticsOptions));
        }

        public DataSet DataSet { get; set; }

        public string Slug { get { return Name.ToUrlString(); } }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public string FileName
        {
            get
            {
                string timestamp = string.Format("{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow);
                return string.Format("{0}_{1}", Slug, timestamp);
            }
        }

        public string DefaultSort { get; set; }
        public Filter[] Filters { get; set; }
        public int Total { get; set; }
        public abstract void Run(ReportDataFilter dataFilter);

        public abstract IQueryable<TModel> Filter(ReportDataFilter dataFilter, IQueryable<TModel> query);
        public abstract IOrderedQueryable<TModel> Sort(ReportDataFilter dataFilter, IQueryable<TModel> query);
        public virtual IQueryable<TModel> ApplyDataFilter(ReportDataFilter dataFilter, IQueryable<TModel> query)
        {
            Total = query.Count();

            if (dataFilter != null)
            {
                query = Filter(dataFilter, query);
                query = Sort(dataFilter, query);

                if (dataFilter.Skip > 0)
                {
                    query = query.Skip(dataFilter.Skip);
                }

                if (dataFilter.Take > 0)
                {
                    query = query.Take(dataFilter.Take);
                }
            }

            return query;
        }

        TokenResponse TokenResponse { get; set; }
        DateTime TokenExpires { get; set; }

        async Task<TokenResponse> GetTokenAsync(bool always = false)
        {
            if (TokenResponse == null || TokenExpires > DateTime.UtcNow || always)
            {
                var discoveryClient = await DiscoveryClient.GetAsync(AuthorizationOptions.Authority);

                if (discoveryClient.IsError)
                    throw new SecurityTokenException(discoveryClient.Error);

                var client = new TokenClient(discoveryClient.TokenEndpoint, AuthorizationOptions.ClientId, AuthorizationOptions.ClientSecret);
                var response = await client.RequestClientCredentialsAsync(AuthorizationOptions.AuthorizationScope);

                if (response.IsError)
                    throw new SecurityTokenException(response.Error);

                TokenResponse = response;
                TokenExpires = DateTime.UtcNow.AddSeconds(response.ExpiresIn);
            }

            return TokenResponse;
        }

        public async Task<HttpContent> GetAnalyticsAsync(string endpoint)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(AnalyticsOptions.Url) })
            {
                client.SetBearerToken((await GetTokenAsync(true)).AccessToken);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(endpoint);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return response.Content;
                    default:
                        return null;
                }
            }
        }
    }
}

