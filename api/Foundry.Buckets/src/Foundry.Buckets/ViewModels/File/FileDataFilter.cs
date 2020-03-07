/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Buckets.Data.Entities;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using System;
using System.Linq;

namespace Foundry.Buckets.ViewModels
{
    /// <summary>
    /// file data filter
    /// </summary>
    public class FileDataFilter : DataFilter<File>
    {
        /// <summary>
        /// filter query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public override IQueryable<File> FilterQuery(IQueryable<File> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var intValues = filter.ToIntValues();
                var stringValues = filter.StringValues;
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                var globalIds = stringValues.Select(s => s.ToLower().Trim()).ToArray();

                switch (key)
                {
                    case "tag":
                        var tags = filter.StringValues;
                        query = not
                            ? query.Where(f => !f.FileTags.Any(ft => tags.Contains(ft.Tag.Name.ToLower())))
                            : query.Where(f => f.FileTags.Any(ft => tags.Contains(ft.Tag.Name.ToLower())));
                        break;
                    case "createdby":
                        if (filter.Value == "me")
                        {
                            query = not
                                ? query.Where(f => !f.FileVersions.Any(fv => fv.CreatedById.ToLower() == identity.Id.ToLower()))
                                : query.Where(f => f.FileVersions.Any(fv => fv.CreatedById.ToLower() == identity.Id.ToLower()));
                        }
                        else
                        {
                            query = not
                                ? query.Where(f => !f.FileVersions.Any(fv => !globalIds.Contains(fv.CreatedById.ToLower())))
                                : query.Where(f => f.FileVersions.Any(fv => globalIds.Contains(fv.CreatedById.ToLower())));
                        }
                        break;
                    case "extension":
                        var extensions = filter.StringValues.Select(t => t.Replace(".", "")).ToArray();
                        query = not
                            ? query.Where(f => !f.FileVersions.Any(fv => extensions.Contains(fv.Extension.ToLower().Replace(".", ""))))
                            : query.Where(f => f.FileVersions.Any(fv => extensions.Contains(fv.Extension.ToLower().Replace(".", ""))));
                        break;
                    case "bucket":
                        if (intValues.Any())
                        {
                            // support for bucket int ids
                            query = not
                                ? query.Where(x => !intValues.Contains(x.BucketId))
                                : query.Where(x => intValues.Contains(x.BucketId));
                        }
                        else if (stringValues.Any())
                        {
                            // support for bucket global ids
                            query = not
                                ? query.Where(x => !globalIds.Contains(x.GlobalId.ToLower()))
                                : query.Where(x => globalIds.Contains(x.GlobalId.ToLower()));
                        }

                        break;
                }
            }

            return query;
        }

        /// <summary>
        /// search query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IQueryable<File> SearchQuery(IQueryable<File> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query.Where(f => f.Name.ToLower().Contains(term) || f.FileVersions.Any(fv => fv.Name.ToLower().Contains(term)));
        }

        /// <summary>
        /// sort query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IOrderedQueryable<File> SortQuery(IQueryable<File> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "name";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "createdby":
                    return desc
                        ? query.OrderByDescending(c => c.FileVersions.Single(fv => fv.IsCurrent).CreatedBy.Name)
                        : query.OrderBy(c => c.FileVersions.Single(fv => fv.IsCurrent).CreatedBy.Name);
                case "contenttype":
                    return desc
                        ? query.OrderByDescending(c => c.FileVersions.Single(fv => fv.IsCurrent).ContentType)
                        : query.OrderBy(c => c.FileVersions.Single(fv => fv.IsCurrent).ContentType);
                case "size":
                    return desc
                        ? query.OrderByDescending(c => c.FileVersions.Single(fv => fv.IsCurrent).Length)
                        : query.OrderBy(c => c.FileVersions.Single(fv => fv.IsCurrent).Length);
                case "recent":
                case "created":
                    return desc
                        ? query.OrderByDescending(c => c.Created)
                        : query.OrderBy(c => c.Created);
                case "bucket":
                    return desc
                        ? query.OrderByDescending(c => c.Bucket.Name)
                        : query.OrderBy(c => c.Bucket.Name);
                case "alphabetic":
                case "name":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
            }
        }
    }
}

