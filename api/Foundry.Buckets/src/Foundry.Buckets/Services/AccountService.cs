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
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.Data.Repositories;
using Foundry.Buckets.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System.Threading.Tasks;

namespace Foundry.Buckets.Services
{
    /// <summary>
    /// account service
    /// </summary>
    public class AccountService : Service<IAccountRepository, Account>
    {
        /// <summary>
        /// create an instance of account service
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="accountRepository"></param>
        /// <param name="mapper"></param>
        public AccountService(IStackIdentityResolver identityResolver, IAccountRepository accountRepository, IMapper mapper)
            : base(identityResolver, accountRepository, mapper) { }

        /// <summary>
        /// get all accounts
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Account, AccountSummary>> GetAll(AccountDataFilter search = null)
        {
            if (IsAdministrator)
                return await PagedResult<Account, AccountSummary>(Repository.GetAll(), search);

            throw new EntityPermissionException("Action requires elevated permissions.");
        }

        /// <summary>
        /// get by global id
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public async Task<AccountDetail> GetByGlobalId(string globalId)
        {
            if (IsAdministrator || globalId.ToLower() == Identity.Id.ToLower())
                return Map<AccountDetail>(await Repository.GetByGlobalId(globalId));

            throw new EntityPermissionException("Action requires elevated permissions.");
        }

        /// <summary>
        /// update account and related account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<AccountDetail> Update(AccountUpdate model)
        {
            if (IsAdministrator)
            {
                var account = await Repository.GetByGlobalId(model.GlobalId);

                if (account.GlobalId != Identity.Id)
                {
                    account.IsAdministrator = model.IsAdministrator;
                }

                account.IsApplication = model.IsApplication;
                account.IsUploadOwner = model.IsUploadOwner;
                account.Name = model.Name;

                account.BucketAccounts.Clear();

                int defaultBucketId = 0;

                foreach (var ba in model.Buckets)
                {
                    if (ba.IsDefault && defaultBucketId == 0)
                    {
                        defaultBucketId = ba.Id;
                    }

                    account.BucketAccounts.Add(new BucketAccount { AccountId = account.GlobalId, BucketId = ba.Id, BucketAccessType = ba.BucketAccessType, IsDefault = ba.IsDefault });
                }

                await Repository.Update(account);

                await BucketService.UpdateDefaultBucket(DbContext, account.GlobalId, defaultBucketId);

                return await GetByGlobalId(account.GlobalId);
            }

            throw new EntityPermissionException("Action requires elevated permissions.");

        }

        /// <summary>
        /// create an account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<AccountDetail> Add(AccountCreate model)
        {
            if (IsAdministrator)
            {
                var account = await Repository.GetByGlobalId(model.GlobalId);

                if (account != null)
                    throw new EntityDuplicateException("An account with global id '" + model.GlobalId + "' already exists.");

                if (string.IsNullOrWhiteSpace(model.GlobalId))
                    throw new InvalidModelException("Global id is required.");

                account = new Account
                {
                    GlobalId = model.GlobalId,
                    IsAdministrator = model.IsAdministrator,
                    IsApplication = model.IsApplication,
                    IsUploadOwner = model.IsUploadOwner,
                    Name = model.Name
                };

                await Repository.Add(account);

                return await GetByGlobalId(account.GlobalId);
            }

            throw new EntityPermissionException("Action requires elevated permissions.");

        }
    }
}

