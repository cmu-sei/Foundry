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
using Microsoft.AspNetCore.Http;
using Foundry.Buckets.Data;
using Foundry.Buckets.Options;
using Foundry.Buckets.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Foundry.Buckets.Storage
{
    /// <summary>
    /// implementation to store files locally
    /// </summary>
    public class StorageLocalProvider : IStorageProvider
    {
        BucketsDbContext DbContext { get; }
        public StorageLocalOptions StorageLocalOptions { get; }
        IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// create an instance of storage local provider
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="dbContext"></param>
        /// <param name="storageLocalOptions"></param>
        public StorageLocalProvider(IHostingEnvironment hostingEnvironment, BucketsDbContext dbContext, StorageLocalOptions storageLocalOptions)
        {
            HostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            StorageLocalOptions = storageLocalOptions ?? throw new ArgumentNullException(nameof(storageLocalOptions));
        }

        internal string GetPath(string globalId, string fileName, BucketDetail bucket)
        {
            var rootPath = HostingEnvironment.ContentRootPath;
            var path = Path.Combine(rootPath, StorageLocalOptions.Path, bucket.GlobalId);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return Path.Combine(path, globalId) + Path.GetExtension(fileName);
        }

        /// <summary>
        /// save form file to bucket
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="bucket"></param>
        /// <returns></returns>
        public async Task<FileStorageResult> Save(IFormFile formFile, BucketDetail bucket)
        {
            var result = new FileStorageResult
            {
                FileName = formFile.FileName,
                GlobalId = Guid.NewGuid().ToString().ToLower()
            };

            try
            {
                result.Path = GetPath(result.GlobalId, result.FileName, bucket);

                using (var fileStream = new FileStream(result.Path, FileMode.Create))
                {
                    await formFile.CopyToAsync(fileStream);
                }

                result.Type = FileStorageResultType.UploadComplete;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.Type = FileStorageResultType.UploadFailed;
            }

            return result;
        }

        /// <summary>
        /// delete file from disk including all versions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<FileStorageResult> Delete(FileDetail model)
        {
            var result = new FileStorageResult
            {
                File = model
            };

            try
            {
                var file = await DbContext.Files.Include(f => f.FileVersions).SingleOrDefaultAsync(f => f.Id == model.Id);

                foreach (var version in file.FileVersions)
                {
                    File.Delete(version.Path);
                }

                result.Type = FileStorageResultType.DeleteComplete;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.Type = FileStorageResultType.DeleteFailed;
            }

            return result;
        }
    }
}

