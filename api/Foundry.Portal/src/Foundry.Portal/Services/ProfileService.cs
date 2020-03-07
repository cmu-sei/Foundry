/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.Extensions.Logging;
using Mos.xApi;
using Newtonsoft.Json.Linq;
using Foundry.Portal.Cache;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Events;
using Foundry.Portal.Extensions;
using Foundry.Portal.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Portal.Services
{
    /// <summary>
    /// profile service
    /// </summary>
    public class ProfileService : DispatchService<Profile>
    {
        IProfileRepository _profileRepository;
        //readonly IGroupRepository _groupRepository;
        IProfileCache _profileCache;

        /// <summary>
        /// create an instance of profile service
        /// </summary>
        /// <param name="profileRepository"></param>
        /// <param name="groupRepository"></param>
        /// <param name="domainEventDispatcher"></param>
        /// <param name="options"></param>
        /// <param name="userResolver"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="mapper"></param>
        /// <param name="profileCache"></param>
        public ProfileService(
            IProfileRepository profileRepository,
            //IGroupRepository groupRepository,
            IDomainEventDispatcher domainEventDispatcher,
            CoreOptions options,
            IStackIdentityResolver userResolver,
            ILoggerFactory loggerFactory,
            AutoMapper.IMapper mapper,
            IProfileCache profileCache)
            : base(domainEventDispatcher, options, userResolver, loggerFactory, mapper)
        {
            _profileRepository = profileRepository ?? throw new ArgumentNullException("profileRepository");
            //_groupRepository = groupRepository ?? throw new ArgumentNullException("groupRepository");
            _profileCache = profileCache ?? throw new ArgumentNullException(nameof(profileCache));
        }

        /// <summary>
        /// get profile by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ProfileDetail> GetByName(string name)
        {
            var profile = _profileRepository.DbContext.Profiles.FirstOrDefault(p => p.Name == name);

            if (profile == null)
                throw new EntityNotFoundException("Profile was not found.");

            return await GetById(profile.Id);
        }

        /// <summary>
        /// get profile by globalId and cache
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public async Task<ProfileDetail> GetByGlobalId(string globalId)
        {
            var profile = _profileCache.Get(globalId);

            if (profile == null)
            {
                profile = _profileRepository.DbContext.Profiles.FirstOrDefault(p => p.GlobalId.ToLower() == globalId.ToLower());

                if (profile == null)
                    throw new EntityNotFoundException("Profile was not found.");

                _profileCache.Set(profile.GlobalId, profile);
            }

            return Map<ProfileDetail>(await GetById(profile.Id));
        }

        /// <summary>
        /// get profile by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProfileDetail> GetById(int id)
        {
            if (id == 0)
                id = Identity.GetId();

            if (!await _profileRepository.Exists(id))
                throw new EntityNotFoundException("Profile was not found.");

            var profile = await _profileRepository.GetById(id);

            return Map<ProfileDetail>(profile);
        }

        /// <summary>
        /// add profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProfileDetail> Add(ProfileCreate model)
        {
            var profile = new Profile
            {
                Name = model.Name,
                Description = model.Description,
                Organization = model.Organization,
                LogoUrl = model.LogoUrl
            };

            if (!string.IsNullOrWhiteSpace(model.GlobalId))
            {
                profile.GlobalId = model.GlobalId;
            }

            var result = await _profileRepository.Add(profile);

            return Map<ProfileDetail>(result);
        }

        /// <summary>
        /// update profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ProfileDetail> Update(ProfileUpdate model)
        {
            if (model.Id != Identity.GetId())
                throw new EntityPermissionException("Profile edit requires elevated permissions.");

            var profile = await _profileRepository.GetById(model.Id);

            if (profile == null)
                throw new EntityNotFoundException("Profile was not found.");

            profile.Name = model.Name;
            profile.Description = model.Description;
            profile.Organization = model.Organization;
            profile.LogoUrl = model.LogoUrl;

            await _profileRepository.Update(profile);

            _profileCache.Remove(profile.GlobalId);

            return Map<ProfileDetail>(profile);
        }

        /// <summary>
        /// get all profiles
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Profile, ProfileSummary>> GetAll(ProfileDataFilter search)
        {
            return await PagedResult<Profile, ProfileSummary>(_profileRepository.GetAll(), search);
        }

        /// <summary>
        /// get all authors by rating
        /// </summary>
        /// <param name="minimumRatingAverage"></param>
        /// <param name="minimumRatingTotal"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Profile, ProfileSummary>> GetAllByRating(Rating minimumRatingAverage, int minimumRatingTotal, ProfileDataFilter search)
        {
            var rating = (double)minimumRatingAverage;

            var query = _profileRepository.GetAll()
                .Where(c => c.Contents.Where(cw =>
                    cw.RatingTotal > minimumRatingTotal).Sum(cs => cs.RatingAverage) /
                    c.Contents.Count(cw => cw.RatingTotal > minimumRatingTotal) >= rating);

            return await PagedResult<Profile, ProfileSummary>(query, search);
        }

        public async Task SetKeyValue(int id, string key, string value)
        {
            if (Identity.GetId() != id)
                throw new EntityPermissionException("Action requires elevated permission");

            await _profileRepository.SetKeyValue(id, key, value);
        }

        /// <summary>
        /// toggles a profile permission
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<ProfileSummary> TogglePermission(int id, SystemPermissions permission)
        {
            // TODO: This is where we can make a call to the ID Server

            // only administrators can update another user's permissions
            if (!IsAdministrator)
            {
                throw new EntityPermissionException("Only administrators can update profile permissions.");
            }

            // a user should not be able to change their own permissions
            if (id == Identity.GetId())
            {
                throw new EntityPermissionException("Users are not permitted to update their own permissions.");
            }

            var profile = await _profileRepository.GetById(id);

            if (profile.Permissions.HasFlag(permission))
            {
                profile.Permissions -= permission;
                await _profileRepository.Update(profile);
            }
            else
            {
                profile.Permissions |= permission;
                await _profileRepository.Update(profile);
                Dispatch(new DomainEvent(profile.GlobalId, profile.Name, DomainEventType.ProfilePermissionGranted));
            }

            // clear cache so next request will fetch profile
            _profileCache.Remove(profile.GlobalId);

            return Map<ProfileSummary>(profile);
        }


        public async Task<ProfileSummary> SetDisabled(int id, bool isDisabled)
        {
            // only administrators can update another user's permissions
            if (!IsAdministrator)
                throw new EntityPermissionException("Action requires elevated permissions.");

            // a user cannot disable their own account
            if (id == Identity.GetId())
                throw new EntityPermissionException("You cannot disable your own account.");

            var profile = await _profileRepository.GetById(id);

            profile.IsDisabled = isDisabled;

            await _profileRepository.Update(profile);

            // clear cache so next request will fetch profile
            _profileCache.Remove(profile.GlobalId);

            return Map<ProfileSummary>(profile);
        }
    }
}

