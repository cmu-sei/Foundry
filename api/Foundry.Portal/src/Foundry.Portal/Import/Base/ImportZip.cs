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
using Newtonsoft.Json;
using ServiceStack.Text;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Foundry.Portal.ViewModels.File;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Foundry.Portal.Import
{
    public class ImportZip : ImportBase, IImport
    {
        string Token { get; }
        string BucketsUrl { get; }

        public ImportZip(string token, ContentOptions contentOptions, ContentService contentService, PlaylistService playlistService)
            : base(contentOptions, contentService, playlistService)
        {
            Token = token;
            BucketsUrl = _contentService.Options.Buckets.Url;
        }

        public override async Task<IEnumerable<ContentExport>> Convert(IFormFile file)
        {
            var contents = new List<ContentExport>();
            var logoUrls = new Dictionary<string, string>();

            using (var archive = new ZipArchive(file.OpenReadStream()))
            {
                var entries = archive.Entries;

                var csv = archive.Entries.SingleOrDefault(e => e.Name == "import.csv");

                if (csv == null)
                {
                    throw new InvalidOperationException("Import content template was not found.");
                }
                else
                {
                    using (var stream = csv.Open())
                    {
                        contents = CsvSerializer.DeserializeFromStream<List<ContentExport>>(stream);
                    }

                    if (contents.Any())
                    {
                        foreach (var content in contents)
                        {
                            content.LogoUrl = await SetLogoUrl(logoUrls, archive, content.GlobalId, content.LogoUrl);
                            content.PublisherLogoUrl = await SetLogoUrl(logoUrls, archive, content.PublisherGlobalId, content.PublisherLogoUrl);
                            content.PlaylistLogoUrl = await SetLogoUrl(logoUrls, archive, content.PlaylistGlobalId, content.PlaylistLogoUrl);
                        }
                    }
                }
            }

            return contents;
        }

        /// <summary>
        /// minimizes uploading the same logo for the same object
        /// </summary>
        /// <param name="logoUrls"></param>
        /// <param name="archive"></param>
        /// <param name="globalId"></param>
        /// <param name="logoUrl"></param>
        /// <returns></returns>
        async Task<string> SetLogoUrl(Dictionary<string, string> logoUrls, ZipArchive archive, string globalId, string logoUrl)
        {
            string savedLogoUrl = null;

            // if the url was never embedded as a file
            if (logoUrl.ToLower().StartsWith("http"))
            {
                savedLogoUrl = logoUrl;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(globalId))
                {
                    // if the logo url was previously streamed return the new url
                    if (logoUrls.ContainsKey(globalId))
                    {
                        savedLogoUrl = logoUrls[globalId];
                    }
                    else
                    {
                        var entry = archive.GetEntry(logoUrl);

                        if (entry != null)
                        {
                            // upload new logo
                            using (var client = new HttpClient())
                            {
                                client.SetBearerToken(Token);
                                using (var content = new MultipartFormDataContent())
                                {
                                    content.Add(new StreamContent(entry.Open())
                                    {
                                        Headers = { ContentLength = entry.Length, ContentType = new MediaTypeHeaderValue(GetMimeType(entry.Name)) }
                                    }, "files", entry.Name);

                                    var response = await client.PostAsync(BucketsUrl + "/api/upload", content);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var jsonString = await response.Content.ReadAsStringAsync();
                                        var results = JsonConvert.DeserializeObject<FileStorageResult[]>(jsonString);

                                        if (results.Any())
                                        {
                                            var url = BucketsUrl + results[0].File.Url;
                                            logoUrls.Add(globalId, url);
                                            savedLogoUrl = url;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(savedLogoUrl))
            {
                savedLogoUrl = _contentOptions.ImportExportFallbackLogoUrl;
            }

            return savedLogoUrl;
        }
    }
}

