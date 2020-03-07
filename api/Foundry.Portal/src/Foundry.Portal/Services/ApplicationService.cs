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
using Microsoft.Extensions.Logging;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.ViewModels;
using Foundry.Portal.WebHooks;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Http.Options;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Services
{
    /// <summary>
    /// application service
    /// </summary>
    public class ApplicationService : Service<Application>
    {
        IApplicationRepository ApplicationRepository { get; }
        AuthorizationOptions AuthorizationOptions { get; }
        IExtensionResolver ExtensionResolver { get; }
        SketchDbContext DbContext { get; }

        /// <summary>
        /// create an instance of the application service
        /// </summary>
        /// <param name="options"></param>
        /// <param name="userResolver"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="mapper"></param>
        /// <param name="dbContext"></param>
        /// <param name="applicationRepository"></param>
        /// <param name="extensionResolver"></param>
        /// <param name="authorizationOptions"></param>
        public ApplicationService(
            CoreOptions options,
            IStackIdentityResolver userResolver,
            ILoggerFactory loggerFactory,
            IMapper mapper,
            SketchDbContext dbContext,
            IApplicationRepository applicationRepository,
            IExtensionResolver extensionResolver,
            AuthorizationOptions authorizationOptions)
            : base(options, userResolver, loggerFactory, mapper)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            ApplicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
            AuthorizationOptions = authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions));
            ExtensionResolver = extensionResolver ?? throw new ArgumentNullException(nameof(extensionResolver));
        }

        /// <summary>
        /// get all applications
        /// </summary>
        /// <param name="dataFilter"></param>
        /// <param name="sync"></param>
        /// <returns></returns>
        public async Task<PagedResult<Application, ApplicationSummary>> GetAll(ApplicationDataFilter dataFilter)
        {
            return await PagedResult<Application, ApplicationSummary>(ApplicationRepository.GetAll(), dataFilter);
        }

        /// <summary>
        /// get by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ApplicationSummary> GetByName(string name)
        {
            return Map<ApplicationSummary>(await ApplicationRepository.GetByName(name));
        }

        /// <summary>
        /// synchronize applicatiosn with Identity Server clients
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Synchronize()
        {
            if (!IsPowerUser)
                throw new EntityPermissionException("Action requires elevated permissions.");

            await FindAndRemoveDuplicates();

            var identityExtensions = await ExtensionResolver.GetPublishedClients();

            foreach (var extension in identityExtensions)
            {
                var app = (await ApplicationRepository.GetById(extension.Id))
                    ?? (await ApplicationRepository.GetByName(extension.Name))
                    ?? (await ApplicationRepository.GetBySlug(extension.Slug));

                if (app == null)
                {
                    // create new application if not found
                    app = new Application
                    {
                        Id = extension.Id,
                        ClientUri = extension.ClientUri,
                        Description = extension.Description,
                        DisplayName = extension.DisplayName,
                        Enabled = extension.Enabled,
                        EventReferenceUri = extension.EventReferenceUri,
                        LogoUri = extension.LogoUri,
                        Name = extension.Name,
                        Slug = extension.Slug
                    };

                    await DbContext.Applications.AddAsync(app);
                }
                else
                {
                    // update application from identity client
                    app.ClientUri = extension.ClientUri;
                    app.Description = extension.Description;
                    app.DisplayName = extension.DisplayName;
                    app.Enabled = extension.Enabled;
                    app.EventReferenceUri = extension.EventReferenceUri;
                    app.LogoUri = extension.LogoUri;
                    app.Name = extension.Name;
                    app.Slug = extension.Slug;
                }

                await DbContext.SaveChangesAsync();
            }

            // hide applications not returned by identity server
            var ids = identityExtensions.Select(e => e.Id).ToArray();

            var missing = await ApplicationRepository.GetAll()
                .Where(a => !ids.Contains(a.Id))
                .ToListAsync();

            foreach (var remove in missing)
            {
                remove.IsHidden = true;
                remove.Enabled = false;
            }

            await DbContext.SaveChangesAsync();

            return true;
        }

        async Task FindAndRemoveDuplicates()
        {
            var allApplications = await ApplicationRepository.GetAll().OrderBy(a => a.Name).ToListAsync();

            var removeApplications = new List<Application>();

            foreach (var app in allApplications)
            {
                if (!removeApplications.Contains(app))
                {
                    var allMatches = allApplications.Where(a => a.Slug == app.Slug);

                    if (allMatches.Count() > 1)
                    {
                        removeApplications.AddRange(allMatches.Skip(1));
                    }
                }
            }

            foreach (var remove in removeApplications)
            {
                await ApplicationRepository.Delete(remove);
            }
        }

        /// <summary>
        /// update application
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ApplicationSummary> Update(ApplicationUpdate model)
        {
            if (!IsPowerUser)
                throw new EntityPermissionException("Action requires elevated permissions.");

            var app = await ApplicationRepository.GetById(model.Id);

            if (app == null)
                throw new EntityNotFoundException("Application '" + model.Id + "' was not found.");

            app.IsHidden = model.IsHidden;
            app.IsPinned = model.IsPinned;

            await DbContext.SaveChangesAsync();

            return Map<ApplicationSummary>(app);
        }

        /// <summary>
        /// add application to the current identity
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public async Task<bool> AddToIdentity(string slug)
        {
            var application = await DbContext.Applications.SingleOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());

            if (application == null)
                throw new EntityNotFoundException("Application '" + slug + "' was not found.");

            return await AddToProfile(application, Identity.GetId());
        }

        /// <summary>
        /// associate application with profiles
        /// </summary>
        /// <param name="application"></param>
        /// <param name="profileIds"></param>
        /// <returns></returns>
        async Task<bool> AddToProfile(Application application, params int[] profileIds)
        {
            if (application == null)
                throw new EntityNotFoundException("Application was not found.");

            var existingProfileIds = await DbContext.ProfileApplications
                .Where(pa => pa.ApplicationId == application.Id && profileIds.Contains(pa.ProfileId))
                .Select(pa => pa.ProfileId).ToArrayAsync();

            var addToProfileIds = profileIds.Except(existingProfileIds);

            foreach (var id in addToProfileIds)
            {
                await DbContext.ProfileApplications.AddAsync(new ProfileApplication
                {
                    ProfileId = id,
                    ApplicationId = application.Id
                });
            }

            await DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// remove application from identity
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFromIdentity(string slug)
        {
            var application = await DbContext.Applications.SingleOrDefaultAsync(a => a.Slug.ToLower() == slug.ToLower());

            if (application == null)
                throw new EntityNotFoundException("Application '" + slug + "' was not found.");

            var id = Identity.GetId();
            var profileApp = await DbContext.ProfileApplications.SingleOrDefaultAsync(pa => pa.ApplicationId == application.Id && pa.ProfileId == id);

            if (profileApp == null)
                throw new InvalidModelException("Identity does not have the application '" + slug + "'");

            DbContext.ProfileApplications.Remove(profileApp);
            await DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// add applications to all profiles
        /// </summary>
        /// <param name="applicationIds"></param>
        /// <returns></returns>
        public async Task<bool> AddToAllProfiles(params int[] applicationIds)
        {
            if (!Identity.Permissions.Contains("poweruser"))
                throw new EntityPermissionException("Action requires elevated permissions.");

            var apps = DbContext.Applications.Where(a => applicationIds.Contains(a.Id));

            var profileIds = await DbContext.Profiles.Select(p => p.Id).ToArrayAsync();

            foreach (var app in apps)
            {
                await AddToProfile(app, profileIds);
            }

            return true;
        }
    }
}
