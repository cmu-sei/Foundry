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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeDetective;
using MimeDetective.Extensions;
using Newtonsoft.Json;
using Foundry.Buckets.Data;
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.Data.Repositories;
using Foundry.Buckets.Repositories;
using Foundry.Buckets.Security;
using Foundry.Buckets.Storage;
using Foundry.Buckets.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO = System.IO;

namespace Foundry.Buckets.Services
{
    /// <summary>
    /// manage file storage and database interactions
    /// </summary>
    public class FileService : Service<IFileRepository, File>
    {
        IHostingEnvironment HostingEnvironment { get; }
        IStorageProvider StorageProvider { get; }
        BucketPermissionMediator BucketPermissionMediator { get; }

        /// <summary>
        /// creates an instance of file service
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="fileRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="host"></param>
        /// <param name="storageProvider"></param>
        /// <param name="bucketPermissionMediator"></param>
        public FileService(IStackIdentityResolver identityResolver, IFileRepository fileRepository, IMapper mapper, IHostingEnvironment host, IStorageProvider storageProvider, BucketPermissionMediator bucketPermissionMediator)
            : base(identityResolver, fileRepository, mapper)
        {
            StorageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
            HostingEnvironment = host ?? throw new ArgumentNullException(nameof(host));
            BucketPermissionMediator = bucketPermissionMediator ?? throw new ArgumentNullException(nameof(bucketPermissionMediator));
        }

        IQueryable<File> Query()
        {
            var query = DbContext.Buckets
                .Include(b => b.BucketAccounts)
                .Include("BucketAccounts.Account");

            var ids = BucketPermissionMediator.Process(query).Select(b => b.Id).ToArray();

            return DbContext.Files
                .Include(f => f.Bucket)
                .Include("Bucket.BucketAccounts")
                .Include(f => f.FileVersions)
                .Include("FileVersions.CreatedBy")
                .Include(f => f.FileTags)
                .Include("FileTags.Tag")
                .Where(f => ids.Contains(f.BucketId));
        }

        /// <summary>
        /// gets a file object by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FileDetail> GetById(int id)
        {
            var file = await Query().SingleOrDefaultAsync(f => f.Id == id);
            return Map<FileDetail>(file);
        }

        /// <summary>
        /// check if id exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Exists(int id)
        {
            return await DbContext.Files.AnyAsync(f => f.Id == id);
        }

        /// <summary>
        /// check if globalId exists
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public async Task<bool> Exists(string globalId)
        {
            return await DbContext.Files.AnyAsync(f => f.GlobalId.ToLower() == globalId.ToLower());
        }

        /// <summary>
        /// gets a file object by globalId
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public async Task<FileDetail> GetByGlobalId(string globalId)
        {
            var query = Query();
            var file = await query.SingleOrDefaultAsync(f => f.GlobalId.ToLower() == globalId.ToLower());
            return Map<FileDetail>(file);
        }

        /// <summary>
        /// get all files accessible by user
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<FileDetail> GetRandomFile(FileDataFilter search = null)
        {
            var pagedResult = await PagedResult<File, FileDetail>(Query(), search);

            if (pagedResult.Results.Length == 0)
                throw new EntityNotFoundException("No file was found matching this search.");

            var random = new Random();
            int randomNumber = random.Next(0, pagedResult.Results.Length);
            return pagedResult.Results[randomNumber];
        }

        /// <summary>
        /// get all files accessible by user
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<File, FileSummary>> GetAll(FileDataFilter search = null)
        {
            return await PagedResult<File, FileSummary>(Query(), search);
        }

        /// <summary>
        /// import files
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ImportFileResult>> Import(IEnumerable<ImportFileUpdate> files)
        {
            if (!Identity.Permissions.Contains("administrator"))
                throw new EntityPermissionException("Action requires elevated permissions.");

            var results = new List<ImportFileResult>();

            var account = await DbContext.Accounts.FirstOrDefaultAsync(a => a.GlobalId.ToLower() == Identity.Id.ToLower());

            foreach (var file in files)
            {
                try
                {
                    if (file.BucketId > 0)
                    {
                        var bucket = await DbContext.Buckets
                            .Include(b => b.BucketAccounts)
                            .Include("BucketAccounts.Account")
                            .SingleOrDefaultAsync(b => b.Id == file.BucketId);

                        var accountIsApplication = false;
                        var accountIsUploadOwner = false;

                        var bucketOwner = bucket.BucketAccounts.FirstOrDefault(ba => ba.BucketAccessType == BucketAccessType.Owner);

                        if (bucketOwner != null)
                        {
                            accountIsApplication = bucketOwner.Account.IsApplication;
                            accountIsUploadOwner = bucketOwner.Account.IsUploadOwner;
                        }

                        var result = await ImportFile(file.Path, file.Name, file.GlobalId, string.Empty, account, bucket);
                        results.Add(result);
                    }
                    else
                    {
                        results.Add(new ImportFileResult { Exception = new Exception("No bucket id.") });
                    }
                }
                catch (Exception ex)
                {
                    results.Add(new ImportFileResult { Exception = ex });
                }
            }

            return results;
        }

        /// <summary>
        /// get all files unmapped on disk
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<ImportFileSummary, ImportFileSummary>> GetAllForImport(ImportFileDataFilter search)
        {
            if (!Identity.Permissions.Contains("administrator"))
                throw new EntityPermissionException("Action requires elevated permissions.");

            var result = new PagedResult<ImportFileSummary, ImportFileSummary>
            {
                DataFilter = search
            };

            var buckets = await DbContext.Buckets.ToListAsync();
            var bucketGlobalIds = buckets.Select(b => b.GlobalId.ToLower());

            var versions = await DbContext.FileVersions.ToListAsync();
            var versionGlobalIds = versions.Select(b => b.GlobalId.ToLower());

            if (StorageProvider is StorageLocalProvider local)
            {
                var directories = IO.Directory.EnumerateDirectories(local.StorageLocalOptions.Path);

                var filePaths = new List<string>();

                foreach (var directory in directories)
                {
                    filePaths.AddRange(IO.Directory.EnumerateFiles(directory));
                }

                var files = filePaths;

                var results = new List<ImportFileSummary>();

                foreach (var file in files)
                {
                    var paths = file.Split(IO.Path.DirectorySeparatorChar);

                    var bucketGlobalId = paths[paths.Length - 2].ToLower();

                    var meta = GetFileMeta(file);
                    var globalId = "";
                    var extension = "";

                    if (meta.Name.Contains("."))
                    {
                        var chunks = meta.Name.Split(".");
                        globalId = chunks[0].ToLower();
                        extension = chunks[chunks.Length - 1];
                    }

                    if (!versions.Any(v => !string.IsNullOrWhiteSpace(v.GlobalId) && v.GlobalId.ToLower() == globalId))
                    {
                        var bucket = buckets.SingleOrDefault(b => b.GlobalId.ToLower() == bucketGlobalId);

                        var isBucketAvailable = false;
                        var bucketName = "";
                        var bucketId = 0;

                        if (bucket != null)
                        {
                            isBucketAvailable = true;
                            bucketName = bucket.Name;
                            bucketId = bucket.Id;
                        }

                        results.Add(new ImportFileSummary
                        {
                            Path = file,
                            GlobalId = globalId,
                            Name = meta.Name,
                            Extension = extension,
                            BucketGlobalId = bucketGlobalId,
                            IsBucketAvailable = isBucketAvailable,
                            BucketName = bucketName,
                            BucketId = bucketId
                        });
                    }
                }

                result.Total = results.Count();

                result.Results = results.Skip(search.Skip).Take(search.Take).ToArray();

            }

            return result;
        }

        /// <summary>
        /// update file
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<FileDetail> Update(FileUpdate model)
        {
            var file = await Query().SingleOrDefaultAsync(f => f.Id == model.Id);

            if (file == null)
                throw new EntityNotFoundException("File with id '" + model.Id + "' was not found.");

            if (!(await HasBucketAccessType(file.BucketId, BucketAccessType.Owner, BucketAccessType.Manager)))
                throw new EntityPermissionException("Action requires elevated permission.");

            // if bucket changed
            if (model.BucketId > 0 && file.BucketId != model.BucketId)
            {
                // if identity does not own both buckets throw
                if (!(await HasBucketAccessType(file.BucketId, BucketAccessType.Owner)) &&
                    !(await HasBucketAccessType(model.BucketId, BucketAccessType.Owner)))
                    throw new EntityPermissionException("Must own both buckets to move file.");

                file.BucketId = model.BucketId;
            }

            await SetTags(file.Id, model.Tags);

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                file.Name = model.Name;
            }

            file.Description = model.Description;
            file.Tags = TagService.ToTagWarehouse(model.Tags);
            await Repository.Update(file);

            return await GetById(file.Id);
        }

        /// <summary>
        /// set tags for file by id
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public async Task<string[]> SetTags(int fileId, string[] tags)
        {
            var tagService = new TagService(IdentityResolver, new TagRepository(DbContext), Mapper);

            var fileTags = await tagService.UpdateFileTags(fileId, tags);

            return fileTags.Select(ft => ft.Tag.Name).ToArray();
        }

        /// <summary>
        /// add file record
        /// </summary>
        /// <remarks>
        /// this should only be called when uploading with the exception of unit tests
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<FileDetail> Add(FileCreate model)
        {
            var hasBucketAccess = await HasBucketAccessType(model.BucketId, BucketAccessType.Owner, BucketAccessType.Manager, BucketAccessType.Member);

            if (!hasBucketAccess)
                throw new EntityPermissionException("Action requires elevated permission.");

            var file = await Add(model, Identity.Id.ToLower());

            return Map<FileDetail>(file);
        }

        async Task<File> Add(FileCreate model, string createdById)
        {
            var created = DateTime.UtcNow;

            var file = model.FileId.HasValue
                ? await DbContext.Files
                    .Include(f => f.FileVersions)
                    .SingleOrDefaultAsync(f => f.Id == model.FileId)
                : new File
                {
                    GlobalId = string.IsNullOrWhiteSpace(model.GlobalId) ? Guid.NewGuid().ToString().ToLower() : model.GlobalId,
                    BucketId = model.BucketId,
                    Name = model.Name,
                    Description = model.Description,
                    CurrentVersionNumber = 0,
                    Created = created
                };

            var version = new FileVersion
            {
                Name = model.Name,
                IsCurrent = true,
                Status = UploadStatus.Initialized,
                Created = created,
                CreatedById = createdById
            };

            file.FileVersions.Add(version);

            return await Repository.Add(file);
        }

        /// <summary>
        /// delete file by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FileStorageResult> Delete(int id)
        {
            var model = await GetById(id);

            if (model == null)
                throw new EntityNotFoundException("File not found.");

            if (!(await HasBucketAccessType(model.BucketId, BucketAccessType.Owner, BucketAccessType.Manager)))
                throw new EntityPermissionException("Action requires elevated permission.");

            var result = await StorageProvider.Delete(model);

            if (result.Type == FileStorageResultType.DeleteComplete)
            {
                var file = await DbContext.Files.Include(f => f.FileVersions).SingleOrDefaultAsync(f => f.Id == model.Id);
                DbContext.FileVersions.RemoveRange(file.FileVersions);
                DbContext.Files.Remove(file);
                await DbContext.SaveChangesAsync();
            }

            return result;
        }

        /// <summary>
        /// upload form files to the specified bucket
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="formFiles"></param>
        /// <param name="fileToVersion"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FileStorageResult>> Upload(BucketDetail bucket, ICollection<IFormFile> formFiles, File fileToVersion = null)
        {
            if (!(await HasBucketAccessType(bucket.Id, BucketAccessType.Owner, BucketAccessType.Manager, BucketAccessType.Member)))
                throw new EntityPermissionException("Action requires elevated permission.");

            var results = new List<FileStorageResult>();
            foreach (var formFile in formFiles)
            {
                FileStorageResult result = null;
                FileVersion current = null;
                try
                {
                    var fileId = fileToVersion == null ? (int?)null : fileToVersion.Id;
                    var fileDetail = await Add(new FileCreate { BucketId = bucket.Id, Name = formFile.FileName, FileId = fileId });
                    current = await DbContext.FileVersions
                        .Include(fv => fv.File)
                        .SingleOrDefaultAsync(fv => fv.IsCurrent && fv.FileId == fileDetail.Id);

                    result = await StorageProvider.Save(formFile, bucket);

                    if (result.Type == FileStorageResultType.UploadComplete)
                    {
                        UpdateFileVersion(formFile.FileName, formFile.ContentType, formFile.Length, result.GlobalId, result.Path, current, fileDetail.Id);
                    }
                    else
                    {
                        throw result.Exception;
                    }
                }
                catch (Exception ex)
                {
                    result.Exception = ex;
                    if (current != null)
                    {
                        current.Status = UploadStatus.Failed;
                    }
                }

                await DbContext.SaveChangesAsync();

                if (current != null)
                {
                    result.File = Mapper.Map<FileDetail>(current.File);
                }

                results.Add(result);
            }

            return results;
        }

        private void UpdateFileVersion(string fileName, string contentType, long length, string globalId, string path, FileVersion current, int fileId)
        {
            current.Name = fileName;
            current.GlobalId = globalId;
            current.Status = UploadStatus.Uploaded;
            current.Path = path;
            current.ContentType = contentType;
            current.Length = length;
            current.Extension = IO.Path.GetExtension(fileName).Replace(".", "");

            var previous = DbContext.FileVersions.Where(v => v.Id != current.Id && v.IsCurrent && v.FileId == fileId).ToList();

            current.File.CurrentVersionNumber = previous.Count() + 1;
            current.VersionNumber = current.File.CurrentVersionNumber;

            foreach (var v in previous)
            {
                v.IsCurrent = false;
            }

            current.IsCurrent = true;
        }

        /// <summary>
        /// gets a physically stored file by id as a PhysicalFileResult to return from the API
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PhysicalFileResult> GetFileResult(int id)
        {
            return await GetFileResult(await GetById(id));
        }

        /// <summary>
        /// gets a physically stored file by global id as a PhysicalFileResult to return from the API
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public async Task<PhysicalFileResult> GetFileResult(string globalId)
        {
            return await GetFileResult(await GetByGlobalId(globalId));
        }

        /// <summary>
        /// get file as file result
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        async Task<PhysicalFileResult> GetFileResult(FileDetail file)
        {
            if (file == null)
                throw new IO.FileNotFoundException("File not found.");

            var fileVersion = await DbContext.FileVersions.SingleOrDefaultAsync(fv => fv.FileId == file.Id && fv.IsCurrent);
            return new PhysicalFileResult(IO.Path.Combine(HostingEnvironment.ContentRootPath, fileVersion.Path), fileVersion.ContentType);
        }

        /// <summary>
        /// loop through all folders in local path and process any seed.json files present
        /// </summary>
        public void ImportSeedFiles()
        {
            if (StorageProvider is StorageLocalProvider local)
            {
                var seedFiles = IO.Directory.EnumerateFiles(local.StorageLocalOptions.Path, "seed.json", IO.SearchOption.AllDirectories);

                foreach (var seed in seedFiles)
                {
                    var dir = IO.Path.GetDirectoryName(seed);

                    var text = IO.File.ReadAllText(seed);

                    var model = JsonConvert.DeserializeObject<BucketImport>(text);

                    if (!string.IsNullOrWhiteSpace(model.AccountGlobalId))
                    {
                        var account = GetOrCreateAccount(model.AccountGlobalId, model.AccountName, model.AccountIsApplication, model.AccountIsUploadOwner);

                        var results = new ImportFileResults
                        {
                            AccountGlobalId = account.GlobalId,
                            AccountName = account.Name,
                            Seed = model
                        };

                        var files = IO.Directory.EnumerateFiles(dir);

                        var paths = files.Where(f => f != seed && !f.Contains("results-")).ToList();

                        foreach (var path in paths)
                        {
                            var parts = path.Split(IO.Path.DirectorySeparatorChar);

                            var bucketGlobalId = parts[1];
                            var fileVersionGlobalId = parts[2];
                            var bucket = GetOrCreateBucket(bucketGlobalId, model.BucketName, account);

                            if (string.IsNullOrWhiteSpace(results.BucketGlobalId))
                            {
                                results.BucketGlobalId = bucket.GlobalId;
                                results.BucketName = bucket.Name;
                            }

                            var fileGlobalId = Guid.NewGuid().ToString().ToLower();
                            var fileImport = model.Files.FirstOrDefault(f => f.FileVersionGlobalId == fileVersionGlobalId);

                            if (fileImport != null)
                            {
                                fileGlobalId = fileImport.FileGlobalId;
                            }

                            results.Results.Add(ImportFile(path, fileVersionGlobalId, fileVersionGlobalId, fileGlobalId, account, bucket).Result);
                        }

                        IO.File.WriteAllText(
                            IO.Path.Combine(dir, string.Format("results-{0}.json", DateTime.UtcNow.ToString("yyyyMMddhhmmss"))),
                            JsonConvert.SerializeObject(results));

                        // delete seed.json file once processed
                        IO.File.Delete(seed);
                    }

                }
            }
        }

        Account GetOrCreateAccount(string accountGlobalId, string accountName, bool accountIsApplication, bool accountIsUploadOwner)
        {
            var account = DbContext.Accounts.SingleOrDefault(a => a.GlobalId.ToLower() == accountGlobalId.ToLower());

            if (account == null)
            {
                account = new Account
                {
                    GlobalId = accountGlobalId.ToLower(),
                    Name = accountName,
                    IsApplication = accountIsApplication,
                    IsUploadOwner = accountIsUploadOwner
                };

                DbContext.Accounts.Add(account);
                DbContext.SaveChanges();
            }

            return account;
        }

        Bucket GetOrCreateBucket(string bucketGlobalId, string bucketName, Account account)
        {
            var bucket = DbContext.Buckets.SingleOrDefault(b => b.GlobalId.ToLower() == bucketGlobalId.ToLower());

            if (bucket == null)
            {
                bucket = new Bucket
                {
                    GlobalId = bucketGlobalId.ToLower(),
                    Name = string.IsNullOrWhiteSpace(bucketName) ? bucketGlobalId : bucketName,
                    CreatedById = account.GlobalId,
                    BucketSharingType = BucketSharingType.Public
                };

                DbContext.Buckets.Add(bucket);
                DbContext.SaveChanges();
            }

            var bucketAccount = DbContext.BucketAccounts.SingleOrDefault(b => b.BucketId == bucket.Id && b.AccountId.ToLower() == account.GlobalId.ToLower());

            if (bucketAccount == null)
            {
                bucketAccount = new BucketAccount
                {
                    AccountId = account.GlobalId,
                    BucketAccessType = BucketAccessType.Owner,
                    BucketId = bucket.Id
                };

                DbContext.BucketAccounts.Add(bucketAccount);
                DbContext.SaveChanges();
            }

            return bucket;
        }

        FileMeta GetFileMeta(string path)
        {
            var filePath = path;

            var fileInfo = new IO.FileInfo(filePath);

            //UpdateFileNameAsGuid(ref filePath, ref fileInfo);

            byte[] fileData = IO.File.ReadAllBytes(filePath);
            var fileType = fileData.GetFileType();

            return new FileMeta
            {
                ImportPath = filePath,
                Name = fileInfo.Name.Contains(".") ? fileInfo.Name : string.Format("{0}.{1}", fileInfo.Name, fileType.Extension),
                ContentType = fileType.Mime,
                Length = fileInfo.Length
            };
        }

        string UpdateFileNameAsGuid(string filePath, IO.FileInfo fileInfo)
        {
            Guid fileVersionGlobalId = Guid.NewGuid();

            string fileNameWithoutExtension = fileInfo.Name.Contains('.')
                ? fileInfo.Name.Split('.')[0]
                : fileInfo.Name;

            if (!Guid.TryParse(fileNameWithoutExtension, out fileVersionGlobalId))
            {
                // if file version is not a GUID rename the file
                var newFilePath = filePath.Replace(fileInfo.Name, fileVersionGlobalId.ToString());
                IO.File.Move(filePath, newFilePath);
                filePath = newFilePath;
                //fileInfo = new IO.FileInfo(filePath);

                return newFilePath;
            }

            return filePath;
        }

        /// <summary>
        /// import file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="fileVersionGlobalId"></param>
        /// <param name="fileGlobalId"></param>
        /// <param name="account"></param>
        /// <param name="bucket"></param>
        /// <returns></returns>
        public async Task<ImportFileResult> ImportFile(string path, string name, string fileVersionGlobalId, string fileGlobalId, Account account, Bucket bucket)
        {
            var result = new ImportFileResult
            {
                Path = path
            };

            try
            {
                if (account == null)
                    throw new ArgumentNullException("account");

                if (bucket == null)
                    throw new ArgumentNullException("bucket");

                var fileVersion = await DbContext.FileVersions
                    .Include(fv => fv.File)
                    .OrderByDescending(fv => fv.VersionNumber)
                    .FirstOrDefaultAsync(fv => fv.GlobalId.ToLower() == fileVersionGlobalId.ToLower());

                var meta = GetFileMeta(path);

                result.ImportPath = meta.ImportPath;

                File file = null;
                var description = "Imported " + DateTime.UtcNow.ToString();

                if (fileVersion == null)
                {
                    file = Add(new FileCreate { GlobalId = fileGlobalId, BucketId = bucket.Id, Name = meta.Name, Description = description }, account.GlobalId).Result;
                    fileVersion = file.FileVersions.First();
                    result.IsNew = true;
                }
                else
                {
                    file = fileVersion.File;
                    file.Name = string.Format("{0}.{1}", file.GlobalId, fileVersion.Extension);
                    file.Description = description;
                    file.BucketId = bucket.Id;
                }

                UpdateFileVersion(meta.Name, meta.ContentType, meta.Length, fileVersionGlobalId, path, fileVersion, file.Id);
                DbContext.SaveChanges();

                result.FileName = file.Name;
                result.FileGlobalId = file.GlobalId;
                result.FileVersionContentType = fileVersion.ContentType;
                result.FileVersionLength = fileVersion.Length;

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }
    }
}

