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
using Stack.Http.Identity;
using Stack.Patterns.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Buckets.Services
{
    /// <summary>
    /// tag service
    /// </summary>
    public class TagService : Service<ITagRepository, Tag>
    {
        /// <summary>
        /// creates an instance of tag service
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="tagRepository"></param>
        /// <param name="mapper"></param>
        public TagService(IStackIdentityResolver identityResolver, ITagRepository tagRepository, IMapper mapper)
            : base(identityResolver, tagRepository, mapper) { }

        /// <summary>
        /// converts a string array to a pipe delimited string
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static string ToTagWarehouse(params string[] tags)
        {
            if (tags == null || !tags.Any())
                return string.Empty;

            return string.Join("|", tags.OrderBy(t => t).Select(t => t.ToSlug()));
        }

        /// <summary>
        /// convert string array to FileTag collection using <paramref name="fileId"/>
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        IEnumerable<FileTag> ConvertToFileTagCollection(int fileId, string[] tags)
        {
            if (tags == null || !tags.Any())
                return new List<FileTag>();

            var entityTags = ConvertToTagCollection(tags);

            return entityTags.Select(t => new FileTag { Tag = t, FileId = fileId });
        }

        /// <summary>
        /// convert string array to Tag collection by looking up Tags or constructing new Tags
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        IEnumerable<Tag> ConvertToTagCollection(string[] tags)
        {
            if (tags == null || !tags.Any())
                return new List<Tag>();

            var found = GetAllByName(tags);

            var notFound = tags.Select(t => t.ToLower())
                .Except(found.Select(t => t.Name.ToLower()));

            var result = new List<Tag>();

            result.AddRange(found);

            foreach (var create in notFound)
            {
                result.Add(new Tag() { Name = create });
            }

            return result;
        }

        /// <summary>
        /// get all tags by name
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public IQueryable<Tag> GetAllByName(string[] tags)
        {
            var names = tags.Select(t => t.ToLower()).ToArray();

            return DbContext.Tags
                .Include(t => t.FileTags)
                .Where(t => names.Contains(t.Name.ToLower()));
        }

        /// <summary>
        /// update FileTags by <paramref name="fileId"/> using <paramref name="tags"/>
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FileTag>> UpdateFileTags(int fileId, params string[] tags)
        {
            IEnumerable<FileTag> result = new List<FileTag>();
            if (tags != null && tags.Any())
            {
                var fileTags = ConvertToFileTagCollection(fileId, tags);
                var fileIds = fileTags.Select(ft => ft.FileId).ToArray().Distinct();
                var remove = DbContext.FileTags.Where(ft => fileIds.Contains(ft.FileId));

                DbContext.FileTags.RemoveRange(remove);

                await DbContext.FileTags.AddRangeAsync(fileTags);
                await DbContext.SaveChangesAsync();

                return fileTags;
            }
            else
            {
                var fileTags = DbContext.FileTags.Where(ft => ft.FileId == fileId);
                DbContext.FileTags.RemoveRange(fileTags);
                await DbContext.SaveChangesAsync();

                return fileTags;
            }
        }
    }
}

