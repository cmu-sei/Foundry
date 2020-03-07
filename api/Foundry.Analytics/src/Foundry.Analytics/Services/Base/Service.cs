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
using Foundry.Analytics.Identity;
using Foundry.Analytics.ViewModels;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Analytics.Services
{
    public abstract class Service
    {
        internal IMapper Mapper { get; }
        internal IStackIdentityResolver IdentityResolver { get; }
        internal AnalyticsPagedResultFactory PagedResultFactory { get; }

        public Service(IStackIdentityResolver identityResolver, IMapper mapper) {
            IdentityResolver = identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            PagedResultFactory = new AnalyticsPagedResultFactory(Mapper);
        }

        IAnalyticsIdentity _identity;
        /// <summary>
        /// get current identity
        /// </summary>
        public IAnalyticsIdentity Identity
        {
            get
            {
                if (_identity == null)
                {
                    try { _identity = IdentityResolver.GetIdentityAsync().Result as IAnalyticsIdentity; }
                    catch { }
                }

                return _identity;
            }
        }

        /// <summary>
        /// default mapping operations with Identity
        /// </summary>
        /// <returns></returns>
        public virtual Action<IMappingOperationOptions> GetMappingOperationOptions()
        {
            return ToMappingOperations(new Dictionary<string, object>() { { ResolutionContextExtensions.ResolutionContextKey, Identity } });
        }

        /// <summary>
        /// map with default mapping operations
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public TType Map<TType>(object source)
        {
            return Map<TType>(source, GetMappingOperationOptions());
        }

        /// <summary>
        /// convert dictionary to mapping operation options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Action<IMappingOperationOptions> ToMappingOperations(IDictionary<string, object> options)
        {
            return mappingOperationOptions =>
            {
                if (options == null)
                    return;

                foreach (var o in options)
                    mappingOperationOptions.Items[o.Key] = o.Value;
            };
        }

        /// <summary>
        /// map object to type with operation options
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="source"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public TType Map<TType>(object source, Action<IMappingOperationOptions> opts)
        {
            try
            {
                if (opts == null)
                    return Mapper.Map<TType>(source);

                return Mapper.Map<TType>(source, opts);
            }
            catch (AutoMapperMappingException ex)
            {
                string message = string.Format("Could not map '{0}' to '{1}'",
                    source.GetType().Name, typeof(TType).Name);

                throw new AutoMapperMappingException(message, ex);
            }
        }

        /// <summary>
        /// get a paged result by generic type and always add Identity for automapper options
        /// </summary>
        /// <typeparam name="TEntityModel"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="dataFilter"></param>
        /// <param name="mappingOperationOptions"></param>
        /// <returns></returns>
        public async Task<PagedResult<TEntityModel, TViewModel>> PagedResult<TEntityModel, TViewModel>(
            IQueryable<TEntityModel> query,
            IDataFilter<TEntityModel> dataFilter,
            Action<IMappingOperationOptions> mappingOperationOptions = null)
            where TEntityModel : class
            where TViewModel : class
        {
            return await PagedResultFactory.Execute<TEntityModel, TViewModel>(query, dataFilter, Identity,
                mappingOperationOptions ?? GetMappingOperationOptions());
        }
    }
}

