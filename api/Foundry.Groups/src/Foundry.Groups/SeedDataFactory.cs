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
using Newtonsoft.Json;
using Foundry.Groups.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Foundry.Groups
{
    public class SeedDataFactory
    {
        IHostingEnvironment _hostingEnvironment;
        GroupsDbContext _db;
        public SeedDataFactory(IHostingEnvironment hostingEnvironment, GroupsDbContext db)
        {
            _hostingEnvironment = hostingEnvironment;
            _db = db;
        }

        TEntity GetDataFromFile<TEntity>(string fileName)
            where TEntity : class, new()
        {
            var path = Path.Combine(_hostingEnvironment.ContentRootPath, fileName);

            if (File.Exists(path))
            {
                return JsonConvert.DeserializeObject<TEntity>(File.ReadAllText(path));
            }

            return new TEntity();
        }

        bool DeleteFile(string fileName)
        {
            var path = Path.Combine(_hostingEnvironment.ContentRootPath, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }

        public bool SeedAccounts()
        {
            try
            {
                var fileName = "accounts.json";
                var accounts = GetDataFromFile<List<Account>>(fileName);

                if (accounts.Any())
                {
                    foreach (var account in accounts)
                    {
                        var add = false;
                        var entity = _db.Accounts.SingleOrDefault(a => a.Id == account.Id);
                        if (entity == null)
                        {
                            add = true;
                            entity = new Account { Id = account.Id };
                        }

                        entity.Name = account.Name;
                        entity.IsAdministrator = account.IsAdministrator;

                        if (add) _db.Accounts.Add(entity);

                        _db.SaveChanges();
                    }

                    DeleteFile(fileName);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Accounts seeding failed.");
                Console.Error.Write(ex);
            }

            return false;
        }

        public bool SeedGroups()
        {
            try
            {
                var fileName = "groups.json";
                var groups = GetDataFromFile<List<Group>>(fileName);

                if (groups.Any())
                {
                    foreach (var group in groups)
                    {
                        _db.Groups.Add(group);
                        _db.SaveChanges();
                    }

                    DeleteFile(fileName);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Groups seeding failed.");
                Console.Error.Write(ex);
            }

            return false;
        }
    }
}

