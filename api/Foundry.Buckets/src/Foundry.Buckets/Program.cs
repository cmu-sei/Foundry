/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Foundry.Buckets.Data;
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.Extensions;
using Stack.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Buckets
{
    /// <summary>
    /// program main
    /// </summary>
    public class Program
    {
        /// <summary>
        /// main entry point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();

            webHost.InitializeDatabase<BucketsDbContext>(Seed)
                .ProcessFolders()
                .Run();
        }

        /// <summary>
        /// create web host builder interface
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();

        /// <summary>
        /// seed buckets database with browser source which owns uploads and administrator account
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        static async Task Seed(BucketsDbContext db)
        {
            if (!db.Accounts.Any(a => a.IsApplication))
            {
                var browser = new Account
                {
                    GlobalId = "sketch-browser",
                    Name = "sketch-browser",
                    IsUploadOwner = true,
                    IsApplication = true
                };

                await db.Accounts.AddAsync(browser);
                await db.SaveChangesAsync();
            }

            var globalId = "9fd3c38e-58b0-4af1-80d1-1895af91f1f9";

            var admin = await db.Accounts
                .Include(a => a.BucketAccounts)
                .SingleOrDefaultAsync(t => t.GlobalId.ToLower() == globalId);

            if (admin == null)
            {
                admin = new Account
                {
                    Name = "Administrator",
                    GlobalId = globalId,
                    IsAdministrator = true
                };

                await db.Accounts.AddAsync(admin);
                await db.SaveChangesAsync();
            }

            if (!admin.IsAdministrator)
            {
                admin.IsAdministrator = true;
                await db.SaveChangesAsync();
            }

            if (!admin.BucketAccounts.Any())
            {
                var pri = new Bucket { Name = admin.Name + " (Private)", BucketSharingType = BucketSharingType.Private, CreatedById = admin.GlobalId };

                pri.BucketAccounts.Add(new BucketAccount { AccountId = admin.GlobalId, BucketAccessType = BucketAccessType.Owner });

                var pub = new Bucket { Name = admin.Name, CreatedById = admin.GlobalId };

                pub.BucketAccounts.Add(new BucketAccount { IsDefault = true, AccountId = admin.GlobalId, BucketAccessType = BucketAccessType.Owner });

                await db.Buckets.AddRangeAsync(pri, pub);
                await db.SaveChangesAsync();
            }
        }
    }
}
