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
using Foundry.Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Foundry.Portal.Export
{
    public abstract class ExportZip : ExportBase, IExport
    {
        public ExportZip(ContentOptions contentOptions, IMapper mapper)
            : base(contentOptions, mapper) { }

        /// <summary>
        /// avoids duplicate entries for the same globalId
        /// </summary>
        /// <param name="logoUrls"></param>
        /// <param name="archive"></param>
        /// <param name="client"></param>
        /// <param name="globalId"></param>
        /// <param name="logoUrl"></param>
        /// <returns></returns>
        async Task<string> SetLogoUrl(Dictionary<string, string> logoUrls, ZipArchive archive, HttpClient client, string globalId, string logoUrl)
        {
            if (!string.IsNullOrWhiteSpace(globalId) && logoUrls.ContainsKey(globalId))
            {
                return logoUrls[globalId];
            }

            var value = await CreateImageEntry(archive, client, logoUrl);

            if (!string.IsNullOrWhiteSpace(value))
            {
                if (!string.IsNullOrWhiteSpace(globalId))
                {
                    logoUrls.Add(globalId, value);
                }
            }

            return value;
        }

        public override async Task<ExportResult> Export(int[] ids)
        {
            var result = new ExportResult();
            var logoUrls = new Dictionary<string, string>();

            var collection = await GetAllByIds(ids);

            try
            {
                var zipStream = new MemoryStream();

                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    using (var client = new HttpClient())
                    {
                        foreach (var content in collection)
                        {
                            content.LogoUrl = await SetLogoUrl(logoUrls, archive, client, content.GlobalId, content.LogoUrl);
                            content.PublisherLogoUrl = await SetLogoUrl(logoUrls, archive, client, content.PublisherGlobalId, content.PublisherLogoUrl);
                            content.PlaylistLogoUrl = await SetLogoUrl(logoUrls, archive, client, content.PlaylistGlobalId, content.PlaylistLogoUrl);
                        }
                    }

                    var csvEntry = archive.CreateEntry("import.csv", CompressionLevel.Optimal);
                    using (var writer = new BinaryWriter(csvEntry.Open()))
                    {
                        var csv = ConvertToBytes(collection);
                        writer.Write(csv, 0, csv.Length);
                        writer.Close();
                    }
                }

                await zipStream.FlushAsync();
                zipStream.Position = 0;
                result.Bytes = zipStream.ToArray();
                result.Message = string.Format("Exported {0} content.", collection.Count());
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
            }

            return result;
        }
    }
}
