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
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Security;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Foundry.Portal.Repositories
{
    public class ProfileRepository : Repository<Profile>, IProfileRepository
    {
        ProfilePermissionMediator _profilePermissionMediator;

        public ProfileRepository(SketchDbContext dbContext, ProfilePermissionMediator profilePermissionMediator)
            : base(dbContext)
        {
            _profilePermissionMediator = profilePermissionMediator ?? throw new ArgumentNullException(nameof(profilePermissionMediator));
        }

        async Task<Profile> GetQuery(Expression<Func<Profile, bool>> expression)
        {
            var query = DbContext.Profiles
                .Include(p => p.KeyValues)
                .Include(p => p.Playlists)
                .Include(p => p.ProfileContents)
                .Include("ProfileContents.Content");

            query = _profilePermissionMediator.Process(query);

            return await query.SingleOrDefaultAsync(expression);
        }

        public async override Task<Profile> GetById(int id)
        {
            return await GetQuery(p => p.Id == id);
        }

        public async Task<Profile> GetByName(string name)
        {
            return await GetQuery(p => p.Name == name);
        }

        public async Task<Profile> GetByGlobalId(string globalId)
        {
            return await GetQuery(p => p.GlobalId == globalId);
        }

        public override IQueryable<Profile> GetAll()
        {
            var query = DbContext.Profiles
                .Include(p => p.KeyValues)
                .Include(p => p.Playlists)
                .Include(p => p.ProfileContents);

            return _profilePermissionMediator.Process(query);
        }

        public async Task SetKeyValue(int id, string key, string value)
        {
            var profile = await GetById(id);

            var keyValue = profile.KeyValues.SingleOrDefault(kv => kv.Key == key);

            if (keyValue == null)
            {
                keyValue = new ProfileKeyValue();
                profile.KeyValues.Add(keyValue);
            }

            keyValue.Key = key.ToLower();
            keyValue.Value = value;

            await DbContext.SaveChangesAsync();
        }
    }
}
