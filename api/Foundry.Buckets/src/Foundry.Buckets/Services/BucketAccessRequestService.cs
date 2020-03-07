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
using Foundry.Buckets.Data;
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.Data.Repositories;
using Foundry.Buckets.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Buckets.Services
{
    /// <summary>
    /// bucket access request service
    /// </summary>
    public class BucketAccessRequestService : Service<IBucketAccessRequestRepository, BucketAccessRequest>
    {
        /// <summary>
        /// creates an instance of bucket access request service
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="bucketAccessRequestRepository"></param>
        /// <param name="mapper"></param>
        public BucketAccessRequestService(IStackIdentityResolver identityResolver, IBucketAccessRequestRepository bucketAccessRequestRepository, IMapper mapper)
            : base(identityResolver, bucketAccessRequestRepository, mapper) { }

        /// <summary>
        /// base query for files
        /// </summary>
        /// <returns></returns>
        IQueryable<BucketAccessRequest> QueryRequests()
        {
            if (Identity == null)
                return new List<BucketAccessRequest>().AsQueryable();

            var ids = GetBucketIdsByAccessType(BucketAccessType.Owner).Result;

            return DbContext.BucketAccessRequests
                .Include(b => b.Account)
                .Include(b => b.Bucket)
                    .ThenInclude(b => b.BucketAccounts)
                .Where(bar => ids.Contains(bar.Id));
        }

        IQueryable<BucketAccessRequest> QueryInvites()
        {
            if (Identity == null)
                return new List<BucketAccessRequest>().AsQueryable();

            var query = DbContext.BucketAccessRequests
                    .Include(b => b.Account)
                    .Include(bar => bar.Bucket)
                    .ThenInclude(b => b.BucketAccounts);

            return query.Where(bar => bar.AccountId.ToLower() == Identity.Id.ToLower());
        }

        /// <summary>
        /// get all access requests
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<BucketAccessRequest, BucketAccessRequestDetail>> GetAllRequests(BucketAccessRequestDataFilter search = null)
        {
            return await PagedResult<BucketAccessRequest, BucketAccessRequestDetail>(QueryRequests(), search);
        }

        /// <summary>
        /// get all access request invites
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<BucketAccessRequest, BucketAccessRequestDetail>> GetAllInvites(BucketAccessRequestDataFilter search = null)
        {
            return await PagedResult<BucketAccessRequest, BucketAccessRequestDetail>(QueryInvites(), search);
        }

        /// <summary>
        /// get by global id
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public async Task<BucketAccessRequestDetail> GetByGlobalId(string globalId)
        {
            return Map<BucketAccessRequestDetail>(await QueryRequests().SingleOrDefaultAsync(b => b.GlobalId.ToLower() == globalId.ToLower()));
        }

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BucketAccessRequestDetail> GetById(int id)
        {
            return Map<BucketAccessRequestDetail>(await QueryRequests().SingleOrDefaultAsync(b => b.Id == id));
        }

        /// <summary>
        /// add a bucket access request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BucketAccessRequestDetail> Request(BucketAccessRequestCreate model)
        {
            if (Identity == null)
                throw new EntityPermissionException("Not authenticated.");

            if (string.IsNullOrWhiteSpace(model.AccountId))
                throw new InvalidModelException("No account specified.");

            if (await HasBucketAccount(model.BucketId, model.AccountId))
                throw new EntityPermissionException("Account already has access.");

            var request = await DbContext.BucketAccessRequests.SingleOrDefaultAsync(bar =>
                bar.BucketId == model.BucketId &&
                bar.AccountId.ToLower() == model.AccountId.ToLower() &&
                bar.Type == BucketAccessRequestType.Pending);

            if (request == null)
            {
                request = new BucketAccessRequest
                {
                    AccountId = model.AccountId.ToLower(),
                    BucketId = model.BucketId,
                    CreatedById = Identity.Id.ToLower()
                };
            }

            await DbContext.BucketAccessRequests.AddAsync(request);
            await DbContext.SaveChangesAsync();

            return Map<BucketAccessRequestDetail>(request);
        }

        /// <summary>
        /// add a bucket access invite
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BucketAccessRequestDetail> Invite(BucketAccessRequestCreate model)
        {
            if (Identity == null)
                throw new EntityPermissionException("Not authenticated.");


            if (string.IsNullOrWhiteSpace(model.AccountId))
                throw new InvalidModelException("No account specified.");

            BucketAccessRequest request = null;

            if (await HasBucketAccount(model.BucketId, model.AccountId))
                throw new EntityPermissionException("Account already has access.");

            request = await DbContext.BucketAccessRequests.SingleOrDefaultAsync(bar =>
                bar.BucketId == model.BucketId &&
                bar.AccountId == model.AccountId &&
                bar.Type == BucketAccessRequestType.Pending);

            if (request == null)
            {
                request = new BucketAccessRequest
                {
                    AccountId = model.AccountId.ToLower(),
                    BucketId = model.BucketId,
                    CreatedById = Identity.Id.ToLower()
                };
            }

            await DbContext.BucketAccessRequests.AddAsync(request);
            await DbContext.SaveChangesAsync();

            return Map<BucketAccessRequestDetail>(request);
        }

        /// <summary>
        /// checks if invite is not null, for the identity, and bucket not already accessible
        /// </summary>
        /// <param name="invite"></param>
        /// <returns></returns>
        async Task<bool> IsValidInvite(BucketAccessRequest invite)
        {
            if (invite == null)
                return false;

            if (invite.AccountId.ToLower() != Identity.Id.ToLower())
                return false;

            if (invite.Type != BucketAccessRequestType.Pending)
                return false;

            if (await HasBucketAccount(invite.BucketId, invite.AccountId))
                return false;

            return true;
        }

        /// <summary>
        /// checks if request is not null and is pending
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        async Task<bool> IsValidRequest(BucketAccessRequest request)
        {
            if (request == null)
                return false;

            if (request.Type != BucketAccessRequestType.Pending)
                return false;

            if (await HasBucketAccount(request.BucketId, request.AccountId))
                return false;

            return true;
        }

        /// <summary>
        /// accept a bucket invitation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Accept(int id)
        {
            var invite = await QueryInvites()
                .SingleOrDefaultAsync(bar => bar.Id == id);

            if (!(await IsValidInvite(invite)))
                throw new EntityPermissionException("Invalid invitation.");

            invite.Type = BucketAccessRequestType.Accepted;

            await DbContext.BucketAccounts.AddAsync(new BucketAccount { BucketId = invite.BucketId, AccountId = Identity.Id.ToLower() });
            await DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// decline a bucket invitation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Decline(int id)
        {
            var invite = await QueryInvites()
                .SingleOrDefaultAsync(bar => bar.Id == id);

            if (!(await IsValidInvite(invite)))
                throw new EntityPermissionException("Invalid invitation.");

            invite.Type = BucketAccessRequestType.Declined;

            await DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// approve bucket access
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Approve(int id)
        {
            var request = await QueryRequests()
                .SingleOrDefaultAsync(bar => bar.Id == id);

            if (!(await IsValidRequest(request)))
                throw new EntityPermissionException("Invalid request.");

            request.Type = BucketAccessRequestType.Approved;

            await DbContext.BucketAccounts.AddAsync(new BucketAccount { BucketId = request.BucketId, AccountId = request.AccountId.ToLower() });
            await DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// deny bucket access
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Deny(int id)
        {
            var request = await QueryRequests()
                .SingleOrDefaultAsync(bar => bar.Id == id);

            if (!(await IsValidRequest(request)))
                throw new EntityPermissionException("Invalid request.");

            request.Type = BucketAccessRequestType.Denied;

            await DbContext.SaveChangesAsync();

            return true;
        }
    }
}
