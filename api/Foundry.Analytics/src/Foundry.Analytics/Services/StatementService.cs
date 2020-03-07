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
using Microsoft.AspNetCore.Http;
using Foundry.Analytics.xApi;
using Foundry.Analytics.xApi.Statements;
using Stack.Http.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinCan;

namespace Foundry.Analytics.Services
{
    public class StatementService : Service
    {
        LearningRecordStoreOptions LearningRecordStoreOptions { get; }
        IHttpContextAccessor HttpContextAccessor { get; }

        public StatementService(IStackIdentityResolver identityResolver, IMapper mapper, LearningRecordStoreOptions learningRecordStoreOptions, IHttpContextAccessor httpContextAccessor)
            : base(identityResolver, mapper)
        {
            LearningRecordStoreOptions = learningRecordStoreOptions ?? throw new ArgumentNullException(nameof(learningRecordStoreOptions));
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        LearningRecordStoreRequest CreateLearningRecordStoreRequest()
        {
            return new LearningRecordStoreRequest(LearningRecordStoreOptions, HttpContextAccessor.HttpContext, IdentityResolver);
        }

        public async Task<bool> AddContentLaunch(LaunchStatementCreate model)
        {
            var request = CreateLearningRecordStoreRequest();
            var response = await request.Save(new LaunchStatement(model.Name, model.GlobalId));

            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage, response.Exception);

            return true;
        }

        public async Task<bool> AddContentView(ViewStatementCreate model)
        {
            var request = CreateLearningRecordStoreRequest();
            var response = await request.Save(new ViewStatement(model.Name, model.GlobalId));

            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage, response.Exception);

            return true;
        }

        public async Task<bool> AddUserLogin()
        {
            var request = CreateLearningRecordStoreRequest();
            var response = await request.Save(new LogInStatement(Identity.Id, Identity.Id));

            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage, response.Exception);

            return true;
        }

        public async Task<bool> AddSearch(SearchStatementCreate model)
        {
            var request = CreateLearningRecordStoreRequest();
            var response = await request.Save(new SearchStatement(model.Term));

            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage, response.Exception);

            return true;
        }

        public async Task<List<IStatement>> GetAllByAgent(string name, int? limit = 0)
        {
            var request = CreateLearningRecordStoreRequest();
            var response = await request.QueryByAgent(name);

            Validate(response);

            var result = new List<IStatement>();
            var statements = response.Response.Content.Statements;
            foreach (var statement in statements)
            {
                IStatement model = Map(statement);

                // TODO: filter by name here until name query issue resolved
                if (model != null && model.AgentName == name)
                {
                    result.Add(model);
                }
            }

            if (limit.HasValue && limit.Value > 0)
                return result.OrderByDescending(x => x.Count).Take(limit.Value).ToList();

            return result.OrderByDescending(x => x.Count).ToList();
        }

        public async Task<List<IStatement>> GetAllByVerb(string verb, int? limit = 0)
        {
            return await GetAllByAgentAndVerb(null, verb, limit);
        }

        void Validate(LearningRecordStoreStatementsResponse response)
        {
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage, response.Exception);

            if (response.Response == null)
                throw new Exception("Response is null");

            if (response.Response.Content == null)
                throw new Exception("Response content is null");

            if (response.Response.Content.Statements == null)
                throw new Exception("Response content statements is null");
        }

        public async Task<List<IStatement>> GetAllByAgentAndVerb(string name, string verb, int? limit = 0)
        {
            var request = CreateLearningRecordStoreRequest();
            var response = string.IsNullOrWhiteSpace(name)
                ? await request.QueryByVerb(verb)
                : await request.QueryByAgentAndVerb(name, verb);

            Validate(response);

            var statements = response.Response.Content.Statements;

            var result = new List<IStatement>();

            foreach (var statement in statements)
            {
                IStatement model = Map(statement);

                if (model != null)
                {
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        var existing = result.FirstOrDefault(x => x.Description == model.Description);

                        if (existing != null)
                            existing.Count += 1;
                        else
                            result.Add(model);
                    }
                    else
                    {
                        // TODO: filter by name here until name query issue resolved
                        if (model.AgentName == name)
                        {
                            result.Add(model);
                        }
                    }
                }
            }

            if (limit.HasValue && limit.Value > 0)
                return result.OrderByDescending(x => x.Count).Take(limit.Value).ToList();

            return result.OrderByDescending(x => x.Count).ToList();
        }

        static IStatement Map(TinCan.Statement statement)
        {
            IStatement model = null;

            if (statement == null)
                return model;

            var timestamp = statement.Timestamp.HasValue
                ? statement.Timestamp.Value.ToUniversalTime()
                : statement.Timestamp;

            var activityDescription = string.Empty;
            var activityName = string.Empty;

            if (statement.Target is Activity activity && activity.Definition != null)
            {
                if (activity.Definition.Description != null)
                    activityDescription = activity.Definition.Description.ToJObject().GetValue("en-US").ToString();

                if (activity.Definition.Name != null)
                    activityName = activity.Definition.Name.ToJObject().GetValue("en-US").ToString();
            }

            var verb = statement.Verb.Id.ToString();

            var agentName = statement.Actor.Name;

            switch (verb)
            {
                case "http://adlnet.gov/expapi/verbs/launched":
                    model = new LaunchStatement(activityName, activityDescription) { Timestamp = timestamp, AgentName = agentName, Slug = activityName.ToUrlString() };
                    break;
                case "http://adlnet.gov/expapi/verbs/viewed":
                    model = new ViewStatement(activityName, activityDescription) { Timestamp = timestamp, AgentName = agentName, Slug = activityName.ToUrlString() };
                    break;
                case "http://adlnet.gov/expapi/verbs/logged-in":
                    model = new LogInStatement(activityName, activityDescription) { Timestamp = timestamp, AgentName = agentName, Slug = activityName.ToUrlString() };
                    break;
                case "http://adlnet.gov/expapi/verbs/searched":
                    model = new SearchStatement(activityName) { Timestamp = timestamp, AgentName = agentName, Slug = activityName.ToUrlString() };
                    break;
                default:
                    break;
            }

            return model;
        }
    }
}
