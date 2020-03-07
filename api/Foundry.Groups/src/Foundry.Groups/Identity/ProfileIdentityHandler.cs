/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stack.Groups.Cache;
using Stack.Groups.Data;
using Stack.Http.Identity;
using System.Threading.Tasks;

namespace Stack.Groups.IdentityHandlers
{
    public class ProfileIdentityHandler : IProfileIdentityHandler
    {
        GroupsDbContext _db;
        IAccountCache _cache;
        ILogger<ProfileIdentityHandler> _logger;

        public ProfileIdentityHandler(GroupsDbContext db, IAccountCache cache, ILogger<ProfileIdentityHandler> logger)
        {
            _db = db;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IStackIdentity> Add(string globalId, string name)
        {
            var account = new Account
            {
                Id = globalId.ToLower(),
                Name = name ?? "Anonymous"
            };

            _db.Accounts.Add(account);
            _db.SaveChanges();

            _cache.Set(globalId, account);

            return ConvertToIdentity(account);
        }

        public async Task<IStackIdentity> Get(string globalId)
        {
            var account = _cache.Get(globalId);

            if (account == null)
            {
                account = await _db.Accounts.SingleOrDefaultAsync(p => p.Id.ToLower() == globalId);

                if (account == null)
                    return null;
            }

            return ConvertToIdentity(account);
        }

        public async Task<IStackIdentity> Update(string globalId, string name)
        {
            var account = await _db.Accounts.SingleOrDefaultAsync(p => p.Id.ToLower() == globalId);

            if (account.Name != name)
            {
                account.Name = name;
                _db.SaveChanges();
                _cache.Remove(account.Id);
            }

            return ConvertToIdentity(account);
        }

        IStackIdentity ConvertToIdentity(Account account)
        {
            if (account == null)
                return null;

            var identity = new ProfileIdentity() { GlobalId = account.Id, Name = account.Name };

            if (account.IsAdministrator)
            {
                identity.Permissions = new string[] { "administrator" };
            }

            return identity;
        }
    }
}

