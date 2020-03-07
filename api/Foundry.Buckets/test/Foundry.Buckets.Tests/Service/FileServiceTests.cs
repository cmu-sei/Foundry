/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Http;
using Moq;
using Foundry.Buckets.Data;
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.Identity;
using Foundry.Buckets.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Foundry.Buckets.Tests.Service
{
    public class FileServiceTests : ServiceTests
    {
        static Mock<IFormFile> MockFile(string name, string content = null)
        {
            var mockFile = new Mock<IFormFile>();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(content ?? name);
            writer.Flush();
            stream.Position = 0;
            mockFile.Setup(_ => _.OpenReadStream()).Returns(stream);
            mockFile.Setup(_ => _.FileName).Returns(name);
            mockFile.Setup(_ => _.Length).Returns(stream.Length);
            return mockFile;
        }

        [Fact]
        public async Task Upload_UploadFileToSourceBucketComplete()
        {
            using (var context = new TestContext<BucketsDbContext>(
                (opt) => { return new BucketsDbContext(opt); },
                (ctx) =>
                {
                    var source = CreateClientIdentity(ctx);

                    InitializeProfileIdentity(ctx, source.GlobalId);
                }))
            {
                var bucketService = CreateBucketService(context);
                var fileService = CreateFileService(context);
                var mockFile = MockFile("Upload_UploadFileComplete.pdf");

                var files = new List<IFormFile>() { mockFile.Object };
                var bucket = await bucketService.GetBucketForRequest();
                var results = (await fileService.Upload(bucket, files)).ToList();

                var f = files[0];
                var r = results[0];

                Assert.Equal(FileStorageResultType.UploadComplete, r.Type);
                Assert.Equal(f.FileName, r.FileName);
                Assert.Equal(f.Length, r.File.Length);
                Assert.Equal(f.ContentType, r.File.ContentType);
                Assert.Equal(bucket.Id, r.File.BucketId);
            }
        }

        [Fact]
        public async Task Upload_UploadFileToDefaultBucketComplete()
        {
            using (var context = new TestContext<BucketsDbContext>(
                (opt) => { return new BucketsDbContext(opt); },
                (ctx) => {
                    InitializeProfileIdentity(ctx);
                }))
            {
                var bucketService = CreateBucketService(context);
                var fileService = CreateFileService(context);
                var mockFile = MockFile("Upload_UploadFileComplete.pdf");

                var files = new List<IFormFile>() { mockFile.Object };
                var bucket = await bucketService.GetDefaultBucket();
                var results = (await fileService.Upload(bucket, files)).ToList();

                var f = files[0];
                var r = results[0];

                Assert.Equal(FileStorageResultType.UploadComplete, r.Type);
                Assert.Equal(f.FileName, r.FileName);
                Assert.Equal(f.Length, r.File.Length);
                Assert.Equal(f.ContentType, r.File.ContentType);
                Assert.Equal(bucket.Id, r.File.BucketId);
            }
        }

        [Fact]
        public async Task Upload_UploadMultipleFilesComplete()
        {
            using (var context = new TestContext<BucketsDbContext>(
                (opt) => { return new BucketsDbContext(opt); },
                (ctx) => { InitializeProfileIdentity(ctx); }))
            {
                var bucketService = CreateBucketService(context);
                var fileService = CreateFileService(context);

                var files = new List<IFormFile>()
                {
                    MockFile("Upload_UploadMultipleFilesComplete1.pdf").Object,
                    MockFile("Upload_UploadMultipleFilesComplete2.pdf").Object,
                    MockFile("Upload_UploadMultipleFilesComplete3.pdf").Object,
                    MockFile("Upload_UploadMultipleFilesComplete4.pdf").Object,
                    MockFile("Upload_UploadMultipleFilesComplete5.pdf").Object
                };
                var bucket = await bucketService.GetDefaultBucket();
                var results = (await fileService.Upload(bucket, files)).ToList();

                for (int i = 0; i < results.Count(); i++)
                {
                    var f = files[i];
                    var r = results[i];

                    Assert.Equal(FileStorageResultType.UploadComplete, r.Type);
                    Assert.Equal(f.FileName, r.FileName);
                    Assert.Equal(f.Length, r.File.Length);
                    Assert.Equal(f.ContentType, r.File.ContentType);
                    Assert.Equal(bucket.Id, r.File.BucketId);
                }
            }
        }

        [Fact]
        public async Task Delete_DeleteComplete()
        {
            using (var context = new TestContext<BucketsDbContext>(
                (opt) => { return new BucketsDbContext(opt); },
                (ctx) => { InitializeProfileIdentity(ctx); }))
            {
                var bucketService = CreateBucketService(context);
                var fileService = CreateFileService(context);
                var mockFile = MockFile("Upload_UploadComplete.pdf");

                var files = new List<IFormFile>() { mockFile.Object };
                var bucket = await bucketService.GetDefaultBucket();
                var uploadResults = (await fileService.Upload(bucket, files)).ToList();

                Assert.Equal(FileStorageResultType.UploadComplete, uploadResults[0].Type);

                var deleteResult = await fileService.Delete(uploadResults[0].File.Id);

                Assert.Equal(FileStorageResultType.DeleteComplete, deleteResult.Type);
            }
        }

        [Fact]
        public async Task Update_FileTags()
        {
            var globalId = Guid.NewGuid().ToString().ToLower();

            using (var context = new TestContext<BucketsDbContext>(
                (opt) => { return new BucketsDbContext(opt); },
                (ctx) =>
                {
                    var account = new Account
                    {
                        GlobalId = globalId,
                        Name = "Update_FileTags"
                    };

                    ctx.DbContext.Accounts.Add(account);
                    ctx.DbContext.SaveChanges();

                    ctx.Identity = new ProfileIdentity { Id = account.GlobalId, Name = account.Name, Permissions = new string[] { }, ClientId = DefaultClientId, Subject = account.GlobalId };
                }))
            {
                var fileService = CreateFileService(context);
                var bucketService = CreateBucketService(context);

                var bucket = await bucketService.GetDefaultBucket();

                var file = await fileService.Add(new FileCreate { BucketId = bucket.Id, Name = "Update_FileTags" });

                Assert.Empty(file.Tags);

                var updated = await fileService.Update(new FileUpdate { Id = file.Id, BucketId = bucket.Id, Tags = new string[] { "tag-1", "tag-2" } });

                Assert.Equal(2, updated.Tags.Count);
            }
        }
    }
}

