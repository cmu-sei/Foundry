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
using Foundry.Buckets.Data;
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.Data.Repositories;
using Stack.Patterns.Repository;
using System.Threading.Tasks;
using System.Linq;

namespace Foundry.Buckets.Repositories
{
    /// <summary>
    /// file repository
    /// </summary>
    public class FileRepository : Repository<BucketsDbContext, File>, IFileRepository
    {
        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="db"></param>
        public FileRepository(BucketsDbContext db)
            : base(db) { }

        /// <summary>
        /// get file entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<File> GetById(int id)
        {
            return await DbContext.Files
                .Include(f => f.CurrentVersionNumber)
                .Include(f => f.Bucket)
                    .ThenInclude(b => b.BucketAccounts)
                .SingleOrDefaultAsync(f => f.Id == id);
        }

        /// <summary>
        /// get file entity by global id
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public async Task<File> GetByGlobalId(string globalId)
        {
            return await DbContext.Files
                .Include(f => f.CurrentVersionNumber)
                .Include(f => f.Bucket)
                    .ThenInclude(b => b.BucketAccounts)
                .SingleOrDefaultAsync(f => f.GlobalId.ToLower() == globalId.ToLower());
        }
    }
}

