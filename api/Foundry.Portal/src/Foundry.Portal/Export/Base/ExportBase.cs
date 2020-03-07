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
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Portal.Export
{
    public abstract class ExportBase : IExport
    {
        public IMapper Mapper { get; }
        public ContentOptions ContentOptions { get; }

        public ExportBase(ContentOptions contentOptions, IMapper mapper)
        {
            ContentOptions = contentOptions ?? throw new ArgumentNullException(nameof(contentOptions));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public abstract Task<ExportResult> Export(int[] ids);

        public abstract Task<IEnumerable<ContentExport>> GetAllByIds(int[] ids);

        public async Task<string> CreateImageEntry(ZipArchive archive, HttpClient client, string logoUrl)
        {
            var savedLogoUrl = logoUrl;

            if (string.IsNullOrWhiteSpace(savedLogoUrl))
            {
                savedLogoUrl = ContentOptions.ImportExportFallbackLogoUrl;
            }
            else
            {
                var entryName = Guid.NewGuid().ToString().ToLower();

                try
                {
                    using (var result = await client.GetAsync(savedLogoUrl))
                    {
                        if (result.IsSuccessStatusCode)
                        {
                            using (var fileStream = await result.Content.ReadAsStreamAsync())
                            {
                                var imageEntry = archive.CreateEntry(entryName, CompressionLevel.Fastest);

                                using (var stream = imageEntry.Open())
                                {
                                    await fileStream.CopyToAsync(stream);
                                }

                                savedLogoUrl = entryName;
                            }
                        }
                    }
                }
                catch
                {
                    savedLogoUrl = ContentOptions.ImportExportFallbackLogoUrl;
                }
            }

            return savedLogoUrl;
        }

        public byte[] ConvertToBytes<T>(IEnumerable<T> collection)
        {
            var value = ServiceStack.StringExtensions.ToCsv(collection);

            return Encoding.UTF8.GetBytes(value.ToString());
        }
    }
}
