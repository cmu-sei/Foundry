/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Api
{
    public static class IWebHostExtensions
    {
        /// <summary>
        /// seed database using json files
        /// </summary>
        /// <param name="webHost"></param>
        /// <returns></returns>
        public static async Task Seed(this IWebHost webHost)
        {
            try
            {
                using (var scope = webHost.Services.CreateScope())
                {
                    var hostingEnvironment = scope.ServiceProvider.GetRequiredService<IHostingEnvironment>();

                    var db = scope.ServiceProvider.GetRequiredService<SketchDbContext>();
                    var seedOptions = scope.ServiceProvider.GetRequiredService<SeedOptions>();
                    var factory = new SeedDataFactory(hostingEnvironment, db, seedOptions);

                    SeedTags(db, factory);

                    SeedProfiles(db, factory);
                }
            }
            catch (Exception ex)
            {
                Console.Error.Write(ex);
            }

            return;
        }

        static void SeedProfiles(SketchDbContext db, SeedDataFactory factory)
        {
            var profilesResult = factory.Seed<Profile>("profiles.json", (x) =>
            {
                var globalId = x.GlobalId.ToLower();

                return db.Profiles.SingleOrDefault(p => p.GlobalId.ToLower() == globalId);
            });

            Console.WriteLine(profilesResult.Message);
            if (profilesResult.Exception != null) Console.WriteLine(profilesResult.Exception);
        }

        static void SeedTags(SketchDbContext db, SeedDataFactory factory)
        {
            var tagsResult = factory.Seed<Tag>("tags.json", (x) =>
            {
                var name = x.Name.ToLower();
                var type = x.TagType.ToLower();

                return db.Tags.SingleOrDefault(t => t.Name.ToLower() == name && t.TagType.ToLower() == type);
            });

            Console.WriteLine(tagsResult.Message);
            if (tagsResult.Exception != null) Console.WriteLine(tagsResult.Exception);
        }
    }
}

