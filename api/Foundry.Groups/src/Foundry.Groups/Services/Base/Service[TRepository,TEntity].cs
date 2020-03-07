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
using Foundry.Groups.Data;
using Foundry.Groups.Identity;
using Stack.Http.Identity;
using Stack.Patterns.Repository;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Groups.Services
{
    /// <summary>
    /// base service class to expose common methods
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class Service<TRepository, TEntity> : StackIdentityRepositoryService<GroupsDbContext, TRepository, TEntity>
        where TRepository : class, IRepository<GroupsDbContext, TEntity>
        where TEntity : class, new()
    {
        /// <summary>
        /// injected Mapper configuration
        /// </summary>
        protected IMapper Mapper { get; }
        protected IValidationHandler ValidationHandler { get; }

        /// <summary>
        /// creates an instance of the base service class
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="repository"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        public Service(IStackIdentityResolver identityResolver, TRepository repository, IMapper mapper, IValidationHandler validationHandler)
            : base(identityResolver, repository, mapper)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            ValidationHandler = validationHandler ?? throw new ArgumentNullException(nameof(validationHandler));
        }

        IStackIdentity _identity;
        public new IStackIdentity Identity
        {
            get
            {
                if (_identity == null)
                {
                    try { _identity = IdentityResolver.GetIdentityAsync().Result; }
                    catch { }
                }

                return _identity;
            }
        }

        public bool IsAdministrator
        {
            get
            {
                if (Identity == null)
                    return false;

                return Identity.Permissions.Contains("administrator");
            }
        }

        /// <summary>
        /// map object to type
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public TType Map<TType>(object source)
        {
            return Map<TType>(source, opts => {
                if (Identity is ProfileIdentity)
                {
                    opts.Items["Profile"] = Identity;
                    opts.Items["ProfileId"] = Identity.Id;
                    opts.Items["ProfileGlobalId"] = Identity.Id;
                }

                if (Identity is ClientIdentity)
                {
                    opts.Items["Client"] = Identity;
                    opts.Items["ClientId"] = Identity.Id;
                    opts.Items["ClientGlobalId"] = Identity.Id;
                }
            });
        }

        /// <summary>
        /// map object to type with operation options
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="source"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        TType Map<TType>(object source, Action<IMappingOperationOptions> opts)
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
        /// <returns></returns>
        public async Task<PagedResult<TEntityModel, TViewModel>> PagedResult<TEntityModel, TViewModel>(
            IQueryable<TEntityModel> query,
            IDataFilter<TEntityModel> dataFilter)
            where TEntityModel : class
            where TViewModel : class
        {
            return await PagedResultFactory.Execute<TEntityModel, TViewModel>(query, dataFilter, Identity);
        }
    }
}
