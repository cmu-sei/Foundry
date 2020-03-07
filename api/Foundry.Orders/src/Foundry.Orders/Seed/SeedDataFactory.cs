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
using Foundry.Orders.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Foundry.Orders.Api
{
    public class SeedDataFactory
    {
        IHostingEnvironment HostingEnvironment { get; }
        OrdersDbContext DbContext { get; }
        SeedOptions SeedOptions { get; }

        public SeedDataFactory(IHostingEnvironment hostingEnvironment, OrdersDbContext sketchDbContext, SeedOptions seedOptions)
        {
            HostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            DbContext = sketchDbContext ?? throw new ArgumentNullException(nameof(sketchDbContext));
            SeedOptions = seedOptions ?? throw new ArgumentNullException(nameof(seedOptions));
        }

        bool DeleteFile(string fileName)
        {
            var path = GetFilePath(fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }

        string GetFilePath(string fileName)
        {
            return Path.Combine(HostingEnvironment.ContentRootPath, SeedOptions.Path, fileName);
        }

        T ConvertFileDataTo<T>(string fileName)
            where T : class, new()
        {
            var path = GetFilePath(fileName);

            if (File.Exists(path))
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }

            return new T();
        }

        /// <summary>
        /// seeds the database with a collection of the specified type
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="lookup"></param>
        /// <returns></returns>
        public SeedDataResult<TEntity> Seed<TEntity>(string fileName, Func<TEntity, TEntity> lookup)
            where TEntity : class, new()
        {
            var result = new SeedDataResult<TEntity>();

            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentNullException(nameof(fileName));

                if (lookup == null)
                    throw new ArgumentNullException(nameof(lookup));

                result.Entities = ConvertFileDataTo<List<TEntity>>(fileName);

                if (result.Entities.Any())
                {
                    foreach (var entity in result.Entities)
                    {
                        var item = new SeedDataResultItem<TEntity>
                        {
                            Entity = lookup(entity)
                        };

                        if (item.Entity == null)
                        {
                            item.Entity = entity;
                            DbContext.Set<TEntity>().Add(entity);
                            DbContext.SaveChanges();
                            item.Status = SeedDataResultItemStatusType.Success;
                        }
                        else
                        {
                            item.Status = SeedDataResultItemStatusType.Exists;
                        }
                    }

                    if (SeedOptions.Delete)
                    {
                        if (DeleteFile(fileName))
                        {
                            result.Message = string.Format("Seeding '{0}' collection from '{1}' succeeded. File delete succeeded.", typeof(TEntity).Name, fileName);
                        }
                        else
                        {
                            result.Message = string.Format("Seeding '{0}' collection from '{1}' succeeded. File delete failed.", typeof(TEntity).Name, fileName);
                        }
                    }
                    else
                    {
                        result.Message = string.Format("Seeding '{0}' collection from '{1}' succeeded.", typeof(TEntity).Name, fileName);
                    }
                }
                else
                {
                    result.Message = string.Format("Seeding '{0}' collection skipped. No items found.", typeof(TEntity).Name, fileName);
                }
            }
            catch (Exception ex)
            {
                result.Message = string.Format("Seeding '{0}' collection from '{1}' failed.", typeof(TEntity).Name, fileName);
                result.Exception = ex;
            }

            return result;
        }
    }
}

