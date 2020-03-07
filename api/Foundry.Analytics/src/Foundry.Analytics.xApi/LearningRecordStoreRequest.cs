/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Http;
using Stack.Http.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinCan;
using TinCan.LrsResponses;

namespace Foundry.Analytics.xApi
{
    public class LearningRecordStoreRequest
    {
        LearningRecordStoreOptions _options;
        IStackIdentityResolver _identityResolver;
        HttpContext _httpContext;

        public LearningRecordStoreRequest(LearningRecordStoreOptions options, HttpContext httpContext, IStackIdentityResolver identityResolver)
        {
            _options = options;
            _identityResolver = identityResolver;
            _httpContext = httpContext;
        }

        public IStackIdentity Identity
        {
            get { return _identityResolver.GetIdentityAsync().Result; }
        }

        TinCan.Statement Map(IStatement statement)
        {
            var request = _httpContext.Request;

            var id = string.Format("{0}://{1}{2}", request.Scheme, request.Host, request.Path);
            var homePage = new Uri(id);

            var activity = new Activity { Id = id };

            if (statement.Name != null || statement.Description != null)
            {
                var activityDefinition = new ActivityDefinition();

                if (statement.Name != null)
                {
                    activityDefinition.Name = new LanguageMap();
                    activityDefinition.Name.Add("en-US", statement.Name);
                }

                if (statement.Description != null)
                {
                    activityDefinition.Description = new LanguageMap();
                    activityDefinition.Description.Add("en-US", statement.Description);
                }


                activity.Definition = activityDefinition;
            }

            var verb = new Verb
            {
                Id = new Uri(statement.Id),
                Display = new LanguageMap(new Dictionary<string, string>() { { "en-US", statement.Verb } })
            };

            /// openid isn't being saved on the learning locker side so we will push the GlobalId for name here
            var actor = new Agent
            {
                Name = Identity.Id,
                Mbox = string.Format("mailto:{0}@mbox.com", Identity.Id),
                Openid = Identity.Id
            };

            //actor.account = new AgentAccount
            //{
            //    name = _options.AccountName,
            //    homePage = homePage
            //};

            actor.Account = new AgentAccount
            {
                Name = Identity.Id,
                HomePage = homePage
            };

            return new TinCan.Statement
            {
                Actor = actor,
                Verb = verb,
                Target = activity
            };
        }

        public async Task<LearningRecordStoreStatementResponse> Save(IStatement statement)
        {
            try
            {
                var learningRecordStore = new RemoteLrs(_options.Uri, _options.Username, _options.Password);

                var converted = Map(statement);
                var response = await learningRecordStore.SaveStatementAsync(converted);

                return new LearningRecordStoreStatementResponse
                {
                    IsSuccessful = response.Success,
                    ErrorMessage = response.ErrMsg,
                    Exception = response.HttpException,
                    Statement = statement,
                    Response = response
                };
            }
            catch (Exception ex)
            {
                return new LearningRecordStoreStatementResponse
                {
                    IsSuccessful = false,
                    ErrorMessage = ex.Message,
                    Exception = ex,
                    Statement = statement,
                    Response = null
                };
            }
        }

        public async Task<LearningRecordStoreStatementsResponse> QueryByVerb(string verb)
        {
            bool isSuccessful = false;
            string errorMessage = "";
            Exception exception = null;
            StatementsResultLrsResponse response = null;

            try
            {

                var learningRecordStore = new RemoteLrs(_options.Uri, _options.Username, _options.Password);

                var query = new StatementsQuery();
                query.VerbId = new Uri("http://adlnet.gov/expapi/verbs/" + verb);

                response = await learningRecordStore.QueryStatementsAsync(query);

                if (response != null)
                {
                    isSuccessful = response.Success;
                    errorMessage = response.ErrMsg;
                    exception = response.HttpException;
                }
            }
            catch (Exception ex)
            {
                isSuccessful = false;
                errorMessage = ex.Message;
                exception = ex;
            }

            return new LearningRecordStoreStatementsResponse
            {
                IsSuccessful = isSuccessful,
                ErrorMessage = errorMessage,
                Exception = exception,
                Response = response
            };
        }

        public async Task<LearningRecordStoreStatementsResponse> QueryByAgent(string name)
        {
            bool isSuccessful = false;
            string errorMessage = "";
            Exception exception = null;
            StatementsResultLrsResponse response = null;

            try
            {
                var learningRecordStore = new RemoteLrs(_options.Uri, _options.Username, _options.Password);

                var query = new StatementsQuery();
                query.Agent = new Agent
                {
                    Name = name,
                    //mbox = string.Format("mailto:{0}@mbox.com", name)
                };

                response = await learningRecordStore.QueryStatementsAsync(query);

                if (response != null)
                {
                    isSuccessful = response.Success;
                    errorMessage = response.ErrMsg;
                    exception = response.HttpException;
                }
            }
            catch (Exception ex)
            {
                isSuccessful = false;
                errorMessage = ex.Message;
                exception = ex;
            }

            return new LearningRecordStoreStatementsResponse
            {
                IsSuccessful = isSuccessful,
                ErrorMessage = errorMessage,
                Exception = exception,
                Response = response
            };
        }

        public async Task<LearningRecordStoreStatementsResponse> QueryByAgentAndVerb(string name, string verb)
        {
            bool isSuccessful = false;
            string errorMessage = "";
            Exception exception = null;
            StatementsResultLrsResponse response = null;

            try
            {
                var learningRecordStore = new RemoteLrs(_options.Uri, _options.Username, _options.Password);

                var query = new StatementsQuery();
                query.VerbId = new Uri("http://adlnet.gov/expapi/verbs/" + verb);
                query.Agent = new Agent
                {
                    Name = name,
                    //mbox = string.Format("mailto:{0}@mbox.com", name)
                };

                response = await learningRecordStore.QueryStatementsAsync(query);

                if (response != null)
                {
                    isSuccessful = response.Success;
                    errorMessage = response.ErrMsg;
                    exception = response.HttpException;
                }
            }
            catch (Exception ex)
            {
                isSuccessful = false;
                errorMessage = ex.Message;
                exception = ex;
            }

            return new LearningRecordStoreStatementsResponse
            {
                IsSuccessful = isSuccessful,
                ErrorMessage = errorMessage,
                Exception = exception,
                Response = response
            };
        }
    }
}

